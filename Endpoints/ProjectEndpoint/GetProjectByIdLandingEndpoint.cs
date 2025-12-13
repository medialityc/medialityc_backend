using System;
using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.ProjectEndpoint.ProjectRequest;
using Medialityc.Endpoints.ProjectEndpoint.ProjectResponse;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.ProjectEndpoint
{
    public class GetProjectByIdLandingEndpoint(IMedialitycDbContext dbContext) : Endpoint<GetProjectByIdRequest,
        Results<Ok<GetProjectByIdResponse>, NotFound<string>, ProblemHttpResult>>
    {
        public override void Configure()
        {
            Get("/projects/landing/{Id}");
            AllowAnonymous();
            Description(x => x
                .WithName("GetProjectByIdLanding")
                .WithSummary("Obtiene un proyecto para landing por ID")
                .Produces<GetProjectByIdResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status404NotFound));
        }

        public override async Task<Results<Ok<GetProjectByIdResponse>, NotFound<string>, ProblemHttpResult>>
            ExecuteAsync(GetProjectByIdRequest request, CancellationToken ct)
        {
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
