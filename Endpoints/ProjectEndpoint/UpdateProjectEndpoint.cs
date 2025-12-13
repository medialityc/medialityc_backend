using FastEndpoints;
using Medialityc.Data;
using Medialityc.Data.Models;
using Medialityc.Endpoints.ProjectEndpoint.ProjectRequest;
using Medialityc.Endpoints.ProjectEndpoint.ProjectResponse;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.ProjectEndpoint
{
    public class UpdateProjectEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<UpdateProjectRequest,
        Results<Ok<GenericProjectResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Put("/projects/update");
        }

        public override async Task<Results<Ok<GenericProjectResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
            ExecuteAsync(UpdateProjectRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            var project = await dbContext.Projects
                .Include(p => p.Network)
                .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

            if (project == null)
            {
                return TypedResults.BadRequest($"El proyecto con ID '{request.Id}' no existe.");
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var normalizedName = request.Name.Trim();

                var duplicateProject = await dbContext.Projects
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id != request.Id && p.Name == normalizedName && p.Company == (request.Company ?? project.Company), ct);

                if (duplicateProject != null)
                {
                    return TypedResults.Conflict($"Ya existe un proyecto con el nombre '{normalizedName}' para la compañía '{duplicateProject.Company}'.");
                }

                project.Name = normalizedName;
            }

            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                project.Description = request.Description.Trim();
            }

            if (!string.IsNullOrWhiteSpace(request.Company))
            {
                var normalizedCompany = request.Company.Trim();

                var duplicateProject = await dbContext.Projects
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id != request.Id && p.Name == project.Name && p.Company == normalizedCompany, ct);

                if (duplicateProject != null)
                {
                    return TypedResults.Conflict($"Ya existe un proyecto con el nombre '{project.Name}' para la compañía '{normalizedCompany}'.");
                }

                project.Company = normalizedCompany;
            }

            if (request.InstagramUrl is not null)
            {
                var normalizedInstagram = request.InstagramUrl.Trim();

                if (string.IsNullOrWhiteSpace(normalizedInstagram))
                {
                    return TypedResults.BadRequest("La URL de Instagram es requerida.");
                }

                project.Network ??= new Network();
                project.Network.Instagram = normalizedInstagram;
            }

            if (request.FacebookUrl is not null)
            {
                var normalizedFacebook = request.FacebookUrl.Trim();

                if (string.IsNullOrWhiteSpace(normalizedFacebook))
                {
                    return TypedResults.BadRequest("La URL de Facebook es requerida.");
                }

                project.Network ??= new Network();
                project.Network.Facebook = normalizedFacebook;
            }

            if (request.LinkedinUrl is not null)
            {
                var normalizedLinkedIn = request.LinkedinUrl.Trim();

                if (string.IsNullOrWhiteSpace(normalizedLinkedIn))
                {
                    return TypedResults.BadRequest("La URL de LinkedIn es requerida.");
                }

                project.Network ??= new Network();
                project.Network.LinkedIn = normalizedLinkedIn;
            }

            if (request.TwitterUrl is not null)
            {
                var normalizedTwitter = request.TwitterUrl.Trim();

                if (string.IsNullOrWhiteSpace(normalizedTwitter))
                {
                    return TypedResults.BadRequest("La URL de Twitter es requerida.");
                }

                project.Network ??= new Network();
                project.Network.Twitter = normalizedTwitter;
            }

            await dbContext.SaveChangesAsync(ct);

            var response = new GenericProjectResponse
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Company = project.Company,
                CreatedAt = project.CreatedAt,
                Network = new NetworkResponseForProject
                {
                    Id = project.Network?.Id ?? 0,
                    Instagram = project.Network?.Instagram ?? string.Empty,
                    Facebook = project.Network?.Facebook ?? string.Empty,
                    LinkedIn = project.Network?.LinkedIn ?? string.Empty,
                    Twitter = project.Network?.Twitter ?? string.Empty
                }
            };

            return TypedResults.Ok(response);
        }
    }
}
