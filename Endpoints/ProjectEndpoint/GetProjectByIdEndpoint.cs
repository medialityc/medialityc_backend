using System;
using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.ProjectEndpoint.ProjectRequest;
using Medialityc.Endpoints.ProjectEndpoint.ProjectResponse;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.ProjectEndpoint
{
    public class GetProjectByIdEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<GetProjectByIdRequest,
        Results<Ok<GetProjectByIdResponse>, NotFound<string>, ProblemHttpResult, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Get("/projects/{Id}");
            Description(x => x
                .WithName("GetProjectById")
                .WithSummary("Obtiene un proyecto por ID")
                .Produces<GetProjectByIdResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status404NotFound));
        }

        public override async Task<Results<Ok<GetProjectByIdResponse>, NotFound<string>, ProblemHttpResult, UnauthorizedHttpResult>>
            ExecuteAsync(GetProjectByIdRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            try
            {
                var project = await dbContext.Projects
                    .Include(p => p.Network)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

                if (project == null)
                {
                    return TypedResults.NotFound($"Proyecto con ID {request.Id} no encontrado.");
                }

                var response = new GetProjectByIdResponse
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
            catch (Exception ex)
            {
                return TypedResults.Problem(ex.Message);
            }
        }
    }
}
