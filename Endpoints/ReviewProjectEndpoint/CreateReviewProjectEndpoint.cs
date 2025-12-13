using FastEndpoints;
using Medialityc.Data;
using Medialityc.Data.Models;
using Medialityc.Endpoints.ReviewProjectEndpoint.ReviewProjectRequest;
using Medialityc.Endpoints.ReviewProjectEndpoint.ReviewProjectResponse;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.ReviewProjectEndpoint
{
    public class CreateReviewProjectEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<CreateReviewProjectRequest,
        Results<Ok<GenericReviewProjectResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Post("/review-projects/create");
            AllowAnonymous();
        }

        public override async Task<Results<Ok<GenericReviewProjectResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
            ExecuteAsync(CreateReviewProjectRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            var normalizedReview = request.SpecificReview.Trim();

            if (string.IsNullOrWhiteSpace(normalizedReview))
            {
                return TypedResults.BadRequest("La reseña específica es requerida.");
            }

            var projectExists = await dbContext.Projects
                .AsNoTracking()
                .AnyAsync(p => p.Id == request.ProjectId, ct);

            if (!projectExists)
            {
                return TypedResults.BadRequest($"El proyecto con ID '{request.ProjectId}' no existe.");
            }

            var workProfileExists = await dbContext.WorkProfiles
                .AsNoTracking()
                .AnyAsync(w => w.Id == request.WorkProfileId, ct);

            if (!workProfileExists)
            {
                return TypedResults.BadRequest($"El perfil de trabajo con ID '{request.WorkProfileId}' no existe.");
            }

            var duplicateReview = await dbContext.ReviewProjects
                .AsNoTracking()
                .FirstOrDefaultAsync(rp => rp.ProjectId == request.ProjectId && rp.WorkProfileId == request.WorkProfileId, ct);

            if (duplicateReview != null)
            {
                return TypedResults.Conflict("Ya existe una reseña para este proyecto y perfil de trabajo.");
            }

            var newReview = new ReviewProject
            {
                ProjectId = request.ProjectId,
                WorkProfileId = request.WorkProfileId,
                SpecificReview = normalizedReview,
                PerformanceEvaluation = request.PerformanceEvaluation
            };

            await dbContext.ReviewProjects.AddAsync(newReview, ct);
            await dbContext.SaveChangesAsync(ct);

            var response = new GenericReviewProjectResponse
            {
                Id = newReview.Id,
                ProjectId = newReview.ProjectId,
                WorkProfileId = newReview.WorkProfileId,
                SpecificReview = newReview.SpecificReview,
                PerformanceEvaluation = newReview.PerformanceEvaluation
            };

            return TypedResults.Ok(response);
        }
    }
}
