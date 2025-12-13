using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.ReviewProjectEndpoint.ReviewProjectRequest;
using Medialityc.Endpoints.ReviewProjectEndpoint.ReviewProjectResponse;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.ReviewProjectEndpoint
{
    public class UpdateReviewProjectEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<UpdateReviewProjectRequest,
        Results<Ok<GenericReviewProjectResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Put("/review-projects/update");
            AllowAnonymous();
        }

        public override async Task<Results<Ok<GenericReviewProjectResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
            ExecuteAsync(UpdateReviewProjectRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            var reviewProject = await dbContext.ReviewProjects
                .FirstOrDefaultAsync(rp => rp.Id == request.Id, ct);

            if (reviewProject == null)
            {
                return TypedResults.BadRequest($"La reseña de proyecto con ID '{request.Id}' no existe.");
            }

            var targetProjectId = request.ProjectId ?? reviewProject.ProjectId;
            var targetWorkProfileId = request.WorkProfileId ?? reviewProject.WorkProfileId;

            if (request.ProjectId.HasValue)
            {
                var projectExists = await dbContext.Projects
                    .AsNoTracking()
                    .AnyAsync(p => p.Id == request.ProjectId.Value, ct);

                if (!projectExists)
                {
                    return TypedResults.BadRequest($"El proyecto con ID '{request.ProjectId.Value}' no existe.");
                }

                reviewProject.ProjectId = request.ProjectId.Value;
            }

            if (request.WorkProfileId.HasValue)
            {
                var workProfileExists = await dbContext.WorkProfiles
                    .AsNoTracking()
                    .AnyAsync(w => w.Id == request.WorkProfileId.Value, ct);

                if (!workProfileExists)
                {
                    return TypedResults.BadRequest($"El perfil de trabajo con ID '{request.WorkProfileId.Value}' no existe.");
                }

                reviewProject.WorkProfileId = request.WorkProfileId.Value;
            }

            var duplicateReview = await dbContext.ReviewProjects
                .AsNoTracking()
                .FirstOrDefaultAsync(rp => rp.Id != request.Id && rp.ProjectId == targetProjectId && rp.WorkProfileId == targetWorkProfileId, ct);

            if (duplicateReview != null)
            {
                return TypedResults.Conflict("Ya existe una reseña para este proyecto y perfil de trabajo.");
            }

            if (!string.IsNullOrWhiteSpace(request.SpecificReview))
            {
                reviewProject.SpecificReview = request.SpecificReview.Trim();
            }

            if (request.PerformanceEvaluation.HasValue)
            {
                reviewProject.PerformanceEvaluation = request.PerformanceEvaluation.Value;
            }

            await dbContext.SaveChangesAsync(ct);

            var response = new GenericReviewProjectResponse
            {
                Id = reviewProject.Id,
                ProjectId = reviewProject.ProjectId,
                WorkProfileId = reviewProject.WorkProfileId,
                SpecificReview = reviewProject.SpecificReview,
                PerformanceEvaluation = reviewProject.PerformanceEvaluation
            };

            return TypedResults.Ok(response);
        }
    }
}
