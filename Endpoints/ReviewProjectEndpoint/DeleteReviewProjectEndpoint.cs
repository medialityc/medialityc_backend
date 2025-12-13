using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.ReviewProjectEndpoint.ReviewProjectRequest;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Medialityc.Endpoints.ReviewProjectEndpoint
{
    public class DeleteReviewProjectEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<DeleteReviewProjectRequest,
        Results<Ok<string>, Conflict<string>, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Delete("/review-projects/delete");
            AllowAnonymous();
        }

        public override async Task<Results<Ok<string>, Conflict<string>, UnauthorizedHttpResult>>
            ExecuteAsync(DeleteReviewProjectRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            var reviewProject = await dbContext.ReviewProjects.FindAsync(new object[] { request.Id }, ct);

            if (reviewProject == null)
            {
                return TypedResults.Conflict($"La reseña de proyecto con ID '{request.Id}' no existe.");
            }

            dbContext.ReviewProjects.Remove(reviewProject);
            await dbContext.SaveChangesAsync(ct);

            return TypedResults.Ok($"Reseña de proyecto con ID '{request.Id}' eliminada exitosamente.");
        }
    }
}
