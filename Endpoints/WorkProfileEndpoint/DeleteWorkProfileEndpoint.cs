using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.WorkProfileEndpoint.WorkProfileRequest;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Medialityc.Endpoints.WorkProfileEndpoint
{
    public class DeleteWorkProfileEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<DeleteWorkProfileRequest,
        Results<Ok<string>, Conflict<string>, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Delete("/work-profiles/delete");
            AllowAnonymous();
        }

        public override async Task<Results<Ok<string>, Conflict<string>, UnauthorizedHttpResult>>
            ExecuteAsync(DeleteWorkProfileRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            var workProfile = await dbContext.WorkProfiles.FindAsync(new object[] { request.Id }, ct);

            if (workProfile == null)
            {
                return TypedResults.Conflict($"El perfil con ID '{request.Id}' no existe.");
            }

            dbContext.WorkProfiles.Remove(workProfile);
            await dbContext.SaveChangesAsync(ct);

            return TypedResults.Ok($"Perfil con ID '{request.Id}' eliminado exitosamente.");
        }
    }
}
