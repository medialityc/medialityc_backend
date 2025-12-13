using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.RoleProfileEndpoint.RoleProfileRequest;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Medialityc.Endpoints.RoleProfileEndpoint
{
    public class DeleteRoleProfileEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<DeleteRoleProfileRequest,
        Results<Ok<string>, Conflict<string>, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Delete("/role-profiles/delete");
            AllowAnonymous();
        }

        public override async Task<Results<Ok<string>, Conflict<string>, UnauthorizedHttpResult>>
            ExecuteAsync(DeleteRoleProfileRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            var roleProfile = await dbContext.RoleProfiles.FindAsync(new object[] { request.Id }, ct);

            if (roleProfile == null)
            {
                return TypedResults.Conflict($"El rol con ID '{request.Id}' no existe.");
            }

            dbContext.RoleProfiles.Remove(roleProfile);
            await dbContext.SaveChangesAsync(ct);

            return TypedResults.Ok($"Rol con ID '{request.Id}' eliminado exitosamente.");
        }
    }
}
