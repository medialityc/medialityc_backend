using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.RoleProfileEndpoint.RoleProfileRequest;
using Medialityc.Endpoints.RoleProfileEndpoint.RoleProfileResponse;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.RoleProfileEndpoint
{
    public class UpdateRoleProfileEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<UpdateRoleProfileRequest,
        Results<Ok<GenericRoleProfileResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Put("/role-profiles/update");
            AllowAnonymous();
        }

        public override async Task<Results<Ok<GenericRoleProfileResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
            ExecuteAsync(UpdateRoleProfileRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            var roleProfile = await dbContext.RoleProfiles
                .FirstOrDefaultAsync(rp => rp.Id == request.Id, ct);

            if (roleProfile == null)
            {
                return TypedResults.BadRequest($"El rol con ID '{request.Id}' no existe.");
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var normalizedName = request.Name.Trim();

                var duplicateRole = await dbContext.RoleProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(rp => rp.Id != request.Id && rp.Name == normalizedName, ct);

                if (duplicateRole != null)
                {
                    return TypedResults.Conflict($"El rol con el nombre '{normalizedName}' ya existe.");
                }

                roleProfile.Name = normalizedName;
            }

            await dbContext.SaveChangesAsync(ct);

            var response = new GenericRoleProfileResponse
            {
                Id = roleProfile.Id,
                Name = roleProfile.Name
            };

            return TypedResults.Ok(response);
        }
    }
}
