using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.ProjectEndpoint.ProjectRequest;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Medialityc.Endpoints.ProjectEndpoint
{
    public class DeleteProjectEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<DeleteProjectRequest,
        Results<Ok<string>, Conflict<string>, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Delete("/projects/delete");
        }

        public override async Task<Results<Ok<string>, Conflict<string>, UnauthorizedHttpResult>>
            ExecuteAsync(DeleteProjectRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            var project = await dbContext.Projects.FindAsync(new object[] { request.Id }, ct);

            if (project == null)
            {
                return TypedResults.Conflict($"El proyecto con ID '{request.Id}' no existe.");
            }

            dbContext.Projects.Remove(project);
            await dbContext.SaveChangesAsync(ct);

            return TypedResults.Ok($"Proyecto con ID '{request.Id}' eliminado exitosamente.");
        }
    }
}
