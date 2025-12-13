using System;
using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.ReviewProjectEndpoint.ReviewProjectRequest;
using Medialityc.Endpoints.ReviewProjectEndpoint.ReviewProjectResponse;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.ReviewProjectEndpoint
{
    public class GetReviewProjectByIdLandingEndpoint(IMedialitycDbContext dbContext) : Endpoint<GetReviewProjectByIdRequest,
        Results<Ok<GetReviewProjectByIdResponse>, NotFound<string>, ProblemHttpResult>>
    {
        public override void Configure()
        {
            Get("/review-projects/landing/{Id}");
            AllowAnonymous();
            Description(x => x
                .WithName("GetReviewProjectByIdLanding")
                .WithSummary("Obtiene una reseña de proyecto para landing por ID")
                .Produces<GetReviewProjectByIdResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status404NotFound));
        }

        public override async Task<Results<Ok<GetReviewProjectByIdResponse>, NotFound<string>, ProblemHttpResult>>
            ExecuteAsync(GetReviewProjectByIdRequest request, CancellationToken ct)
        {
            try
            {
                var reviewProject = await dbContext.ReviewProjects
                    .AsNoTracking()
                    .FirstOrDefaultAsync(rp => rp.Id == request.Id, ct);

                if (reviewProject == null)
                {
                    return TypedResults.NotFound($"Reseña de proyecto con ID {request.Id} no encontrada.");
                }

                var response = new GetReviewProjectByIdResponse
                {
                    Id = reviewProject.Id,
                    ProjectId = reviewProject.ProjectId,
                    WorkProfileId = reviewProject.WorkProfileId,
                    SpecificReview = reviewProject.SpecificReview,
                    PerformanceEvaluation = reviewProject.PerformanceEvaluation
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
