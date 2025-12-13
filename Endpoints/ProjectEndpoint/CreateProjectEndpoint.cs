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
    public class CreateProjectEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<CreateProjectRequest,
        Results<Ok<GenericProjectResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Post("/projects/create");
        }

        public override async Task<Results<Ok<GenericProjectResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
            ExecuteAsync(CreateProjectRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            var normalizedName = request.Name.Trim();
            var normalizedDescription = request.Description.Trim();
            var normalizedCompany = request.Company.Trim();
            var normalizedInstagramUrl = request.InstagramUrl.Trim();
            var normalizedFacebookUrl = request.FacebookUrl.Trim();
            var normalizedLinkedinUrl = request.LinkedinUrl.Trim();
            var normalizedTwitterUrl = request.TwitterUrl.Trim();

            if (string.IsNullOrWhiteSpace(normalizedName) 
            || string.IsNullOrWhiteSpace(normalizedDescription) 
            || string.IsNullOrWhiteSpace(normalizedCompany)
            || string.IsNullOrWhiteSpace(normalizedInstagramUrl)
            || string.IsNullOrWhiteSpace(normalizedFacebookUrl)
            || string.IsNullOrWhiteSpace(normalizedLinkedinUrl)
            || string.IsNullOrWhiteSpace(normalizedTwitterUrl)
            )
            {
                return TypedResults.BadRequest("Todos los campos son requeridos.");
            }

            var existingProject = await dbContext.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Name == normalizedName && p.Company == normalizedCompany, ct);

            if (existingProject != null)
            {
                return TypedResults.Conflict($"El proyecto '{normalizedName}' de la compañía '{normalizedCompany}' ya existe.");
            }

            var newProject = new Project
            {
                Name = normalizedName,
                Description = normalizedDescription,
                Company = normalizedCompany,
                Network = new Network
                {
                    Instagram = normalizedInstagramUrl,
                    Facebook = normalizedFacebookUrl,
                    LinkedIn = normalizedLinkedinUrl,
                    Twitter = normalizedTwitterUrl
                },
            };

            await dbContext.Projects.AddAsync(newProject, ct);
            await dbContext.SaveChangesAsync(ct);

            var response = new GenericProjectResponse
            {
                Id = newProject.Id,
                Name = newProject.Name,
                Description = newProject.Description,
                Company = newProject.Company,
                CreatedAt = newProject.CreatedAt,
                Network = new NetworkResponseForProject
                {
                    Id = newProject.Network.Id,
                    Instagram = newProject.Network.Instagram,
                    Facebook = newProject.Network.Facebook,
                    LinkedIn = newProject.Network.LinkedIn,
                    Twitter = newProject.Network.Twitter
                }
            };

            return TypedResults.Ok(response);
        }
    }
}
