using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Medialityc.Utils.Options;

namespace Medialityc.Services.MinioService
{
    public class MinioBlobServices : IBlobServices
    {
        private readonly string _bucketName;
        private readonly IMinioClient _minioClient;
        private readonly string _endpoint;


        public MinioBlobServices(IOptions<MinioOptions> options)
        {
            _bucketName = options.Value.Bucket;
            _endpoint = options.Value.Endpoint;
            
            Console.WriteLine($"🔧 Configurando MinIO:");
            Console.WriteLine($"   - Endpoint: {options.Value.Endpoint}");
            Console.WriteLine($"   - Bucket: {options.Value.Bucket}");
            Console.WriteLine($"   - UseSSL: {options.Value.UseSSL}");
            Console.WriteLine($"   - AccessKey: {options.Value.AccessKey?.Substring(0, Math.Min(8, options.Value.AccessKey?.Length ?? 0))}...");
            
            _minioClient = new MinioClient()
                .WithEndpoint(options.Value.Endpoint)
                .WithCredentials(options.Value.AccessKey, options.Value.SecretKey)
                .WithSSL(options.Value.UseSSL)
                .Build();
        }

        private async Task CreateBucketIfNotExistsAsync(CancellationToken ct)
        {
            try
            {
                if (!await BucketExistsAsync(ct))
                {
                    var makeBucketArgs = new MakeBucketArgs()
                        .WithBucket(_bucketName);
                    await _minioClient.MakeBucketAsync(makeBucketArgs, ct);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creando el bucket: {ex.Message}");
                throw;
            }
        }

        private async Task<bool> BucketExistsAsync(CancellationToken ct)
        {
            try
            {
                var buckets = await _minioClient.ListBucketsAsync(ct);
                // Access the Buckets property of ListAllMyBucketsResult to use LINQ's Any method
                return buckets.Buckets.Any(b => b.Name == _bucketName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error verificando el bucket: {ex.Message}");
                return false;
            }
        }

        public async Task<string> PresignedGetUrl(string objPath, CancellationToken ct)
        {
            var args = new PresignedGetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objPath)
                .WithExpiry(60 * 60 * 24 * 7);
            return await _minioClient.PresignedGetObjectAsync(args);
        }

        public async Task<string> UploadBlob(IFormFile file, string? previousUrl, CancellationToken ct)
        {
            try
            {
                // Validar el archivo
                if (file == null || file.Length == 0)
                    throw new ArgumentException("Archivo inválido");

                // Validar extensiones permitidas
                var ext = Path.GetExtension(file.FileName).ToLower();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".webp" };
                if (!allowedExtensions.Contains(ext))
                    throw new InvalidOperationException($"Extensión de archivo no permitida: {ext}. Extensiones permitidas: {string.Join(", ", allowedExtensions)}");

                Console.WriteLine($"📤 Iniciando subida de archivo: {file.FileName} ({file.Length} bytes)");

                // Verificar conexión a MinIO antes de proceder
                var connectionValid = await ValidateConnection(ct);
                if (!connectionValid)
                {
                    throw new Exception("No se puede conectar a MinIO. Verifica la configuración.");
                }

                await CreateBucketIfNotExistsAsync(ct);

                // Generar un ID único para el archivo
                var fileId = Guid.NewGuid().ToString();
                var objectPath = $"images/{fileId}{ext}";

                Console.WriteLine($"🎯 Subiendo a: {objectPath}");

                // Eliminar el archivo anterior si existe
                if (!string.IsNullOrEmpty(previousUrl))
                {
                    try
                    {
                        await DeleteBlob(previousUrl, ct);
                        Console.WriteLine($"🗑️ Archivo anterior eliminado: {previousUrl}");
                    }
                    catch (Exception deleteEx)
                    {
                        Console.WriteLine($"⚠️ Error eliminando archivo anterior (continuando): {deleteEx.Message}");
                    }
                }

                // Subir el nuevo archivo
                await using var fileStream = file.OpenReadStream();
                var uploadArgs = new PutObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(objectPath)
                    .WithStreamData(fileStream)
                    .WithObjectSize(file.Length)
                    .WithContentType(file.ContentType ?? "application/octet-stream");

                var res = await _minioClient.PutObjectAsync(uploadArgs, ct);
                Console.WriteLine($"✅ Subida exitosa: {objectPath}");

                // VERIFICAR que el archivo existe después de subirlo
                var exists = await ValidateBlobExistance(objectPath, ct);
                Console.WriteLine($"🔍 Archivo existe después de subir: {exists}");

                if (!exists)
                {
                    throw new Exception("El archivo no se guardó correctamente en MinIO");
                }

                return objectPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error detallado en subida:");
                Console.WriteLine($"   - Archivo: {file?.FileName}");
                Console.WriteLine($"   - Tamaño: {file?.Length} bytes");
                Console.WriteLine($"   - ContentType: {file?.ContentType}");
                Console.WriteLine($"   - Error: {ex.Message}");
                Console.WriteLine($"   - StackTrace: {ex.StackTrace}");
                
                // Si es un error de XML, probablemente es un problema de conectividad o configuración
                if (ex.Message.Contains("XML") || ex.Message.Contains("document"))
                {
                    throw new Exception($"Error de conectividad con MinIO. Verifica la configuración del endpoint y credenciales. Error original: {ex.Message}");
                }
                
                throw;
            }
        }

        public Task<string> UploadBlobByUrl(string fileUrl, string? previousUrl, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteBlob(string url, CancellationToken ct)
        {
            var args = new RemoveObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(url);

            await _minioClient.RemoveObjectAsync(args, ct);
        }

        public async Task<bool> ValidateBlobExistance(string url, CancellationToken ct)
        {
            try
            {
                var statObjectArgs = new StatObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(url);

                await _minioClient.StatObjectAsync(statObjectArgs, ct);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ValidateConnection(CancellationToken ct = default)
        {
            try
            {
                Console.WriteLine("🔄 Validando conexión a MinIO...");

                // Listar todos los buckets para validar la conexión
                var buckets = await _minioClient.ListBucketsAsync(ct);

                Console.WriteLine($"✅ Conexión a MinIO exitosa");
                Console.WriteLine($"📦 Buckets disponibles: {buckets.Buckets.Count}");

                foreach (var bucket in buckets.Buckets)
                {
                    Console.WriteLine($"   - {bucket.Name} (Creado: {bucket.CreationDate})");
                }

                // Verificar si el bucket configurado existe
                var targetBucketExists = buckets.Buckets.Any(b => b.Name == _bucketName);
                if (targetBucketExists)
                {
                    Console.WriteLine($"✅ Bucket configurado '{_bucketName}' encontrado");
                }
                else
                {
                    Console.WriteLine($"⚠️ Bucket configurado '{_bucketName}' NO encontrado");
                    Console.WriteLine(
                        $"   Buckets disponibles: {string.Join(", ", buckets.Buckets.Select(b => b.Name))}");
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error conectando a MinIO: {ex.Message}");
                Console.WriteLine($"   Endpoint: {_minioClient.Config.Endpoint}");
                Console.WriteLine($"   Bucket configurado: {_bucketName}");
                return false;
            }
        }
    }
}