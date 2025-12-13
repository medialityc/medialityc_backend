using System.Text;
using Medialityc.Data;
using Medialityc.Utils.Authentication;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MedialitycDbContext>((sp,options) =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("stringConnection"));
    });

// Registrar la interfaz del DbContext
builder.Services.AddScoped<IMedialitycDbContext>(provider => 
    provider.GetRequiredService<MedialitycDbContext>());

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDevelopment", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("Authentication"));

var authSettings = builder.Configuration
    .GetSection("Authentication")
    .Get<AuthSettings>() ?? throw new InvalidOperationException("La sección 'Authentication' no está configurada correctamente en appsettings.");

builder.Services.AddSingleton<IAuthService, AuthService>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.SecretKey)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = authSettings.Issuer,
            ValidAudience = authSettings.Audience,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services
    .AddAuthorization()
    .AddFastEndpoints()
    .AddSwaggerGen();


builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Medialityc API";
        s.Version = "v1";
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
    app.UseCors("AllowDevelopment");
}
else
{
    app.UseCors("AllowAllOrigins");
}

// Only use HTTPS redirection in development
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerGen();

app.UseFastEndpoints(c => {
    c.Endpoints.RoutePrefix = "suntravel";
    c.Serializer.Options.DefaultIgnoreCondition = 
        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// Inicialización de la base de datos y Minio
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MedialitycDbContext>();
    
    //dbContext.Database.EnsureDeleted();
    dbContext.Database.Migrate();


   
}

app.Run();