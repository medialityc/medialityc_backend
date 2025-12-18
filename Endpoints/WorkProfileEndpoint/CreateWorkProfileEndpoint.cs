using FastEndpoints;
using Medialityc.Data;
using Medialityc.Data.Models;
using Medialityc.Endpoints.WorkProfileEndpoint.WorkProfileRequest;
using Medialityc.Endpoints.WorkProfileEndpoint.WorkProfileResponse;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Medialityc.Services.MinioService;
namespace Medialityc.Endpoints.WorkProfileEndpoint
{
    public class CreateWorkProfileEndpoint(IMedialitycDbContext dbContext, IAuthService authService, IBlobServices blobServices) : Endpoint<CreateWorkProfileRequest,
        Results<Ok<GenericWorkProfileResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Post("/work-profiles/create");
            AllowAnonymous();
        }

        public override async Task<Results<Ok<GenericWorkProfileResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
            ExecuteAsync(CreateWorkProfileRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            string objectPath = "";
            try {
                 objectPath = await blobServices.UploadBlob(request.Image, null, ct);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest($"Error al subir la imagen: {ex.Message}");
            }

            var normalizedFirstName = request.FirsName.Trim();
            var normalizedLastName = request.LastName.Trim();
            var normalizedEmail = request.Email.Trim();
            var normalizedGitHub = request.GitHubProfile.Trim();
            var normalizedOverallReview = request.OverallReview?.Trim();

            if (string.IsNullOrWhiteSpace(normalizedFirstName) || string.IsNullOrWhiteSpace(normalizedLastName) ||
                string.IsNullOrWhiteSpace(normalizedEmail) || string.IsNullOrWhiteSpace(normalizedGitHub))
            {
                return TypedResults.BadRequest("Todos los campos obligatorios deben ser proporcionados.");
            }

            var areaExists = await dbContext.Areas
                .AsNoTracking()
                .AnyAsync(a => a.Id == request.AreaId, ct);

            if (!areaExists)
            {
                return TypedResults.BadRequest($"El área con ID '{request.AreaId}' no existe.");
            }

            var roleProfileExists = await dbContext.RoleProfiles
                .AsNoTracking()
                .FirstAsync(rp => rp.Id == request.RoleProfileId, ct);

            if (roleProfileExists == null)
            {
                return TypedResults.BadRequest($"El rol con ID '{request.RoleProfileId}' no existe.");
            }

            var duplicateProfile = await dbContext.WorkProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(wp => wp.Email == normalizedEmail, ct);

            if (duplicateProfile != null)
            {
                return TypedResults.Conflict($"Ya existe un perfil con el correo '{normalizedEmail}'.");
            }

            var areaExist = await dbContext.Areas.FirstAsync(a => a.Id == request.AreaId, ct);
            if (areaExist == null)
            {
                return TypedResults.BadRequest($"El área con ID '{request.AreaId}' no existe.");
            }

            var newWorkProfile = new WorkProfile
            {
                FirsName = normalizedFirstName,
                LastName = normalizedLastName,
                Role = roleProfileExists ,
                Area = areaExist ,
                Email = normalizedEmail,
                GitHubProfile = normalizedGitHub,
                Image = objectPath ?? string.Empty,
                ReviewStars = request.ReviewStars,
                OverallReview = normalizedOverallReview ?? string.Empty
            };


            await dbContext.WorkProfiles.AddAsync(newWorkProfile, ct);
            await dbContext.SaveChangesAsync(ct);

            var responsePrincipalImageUrl = !string.IsNullOrEmpty(newWorkProfile.Image) 
                ? await blobServices.PresignedGetUrl(newWorkProfile.Image, ct) 
                : string.Empty;

            var response = new GenericWorkProfileResponse
            {
                Id = newWorkProfile.Id,
                FirsName = newWorkProfile.FirsName,
                LastName = newWorkProfile.LastName,
                AreaId = newWorkProfile.Area.Id,
                RoleProfileId = newWorkProfile.Role.Id,
                Email = newWorkProfile.Email,
                GitHubProfile = newWorkProfile.GitHubProfile,
                Image = newWorkProfile.Image,
                ReviewStars = newWorkProfile.ReviewStars,
                OverallReview = newWorkProfile.OverallReview
            };

            return TypedResults.Ok(response);
        }
    }
}
