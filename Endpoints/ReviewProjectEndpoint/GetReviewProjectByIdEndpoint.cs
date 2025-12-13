using System;
using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.ReviewProjectEndpoint.ReviewProjectRequest;
using Medialityc.Endpoints.ReviewProjectEndpoint.ReviewProjectResponse;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.ReviewProjectEndpoint
{
    public class GetReviewProjectByIdEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<GetReviewProjectByIdRequest,
        Results<Ok<GetReviewProjectByIdResponse>, NotFound<string>, ProblemHttpResult, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Get("/review-projects/{Id}");
            AllowAnonymous();
            Description(x => x
                .WithName("GetReviewProjectById")
                .WithSummary("Obtiene una reseña de proyecto por ID")
                .Produces<GetReviewProjectByIdResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status404NotFound));
        }

        public override async Task<Results<Ok<GetReviewProjectByIdResponse>, NotFound<string>, ProblemHttpResult, UnauthorizedHttpResult>>
            ExecuteAsync(GetReviewProjectByIdRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

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
