using FastEndpoints;
using Medialityc.Data;
using Medialityc.Data.Models;
using Medialityc.Endpoints.RoleProfileEndpoint.RoleProfileRequest;
using Medialityc.Endpoints.RoleProfileEndpoint.RoleProfileResponse;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.RoleProfileEndpoint
{
    public class CreateRoleProfileEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<CreateRoleProfileRequest,
        Results<Ok<GenericRoleProfileResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Post("/role-profiles/create");
            AllowAnonymous();
        }

        public override async Task<Results<Ok<GenericRoleProfileResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
            ExecuteAsync(CreateRoleProfileRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            var normalizedName = request.Name.Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return TypedResults.BadRequest("El nombre del rol es requerido.");
            }

            var existingRole = await dbContext.RoleProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(rp => rp.Name == normalizedName, ct);

            if (existingRole != null)
            {
                return TypedResults.Conflict($"El rol con el nombre '{normalizedName}' ya existe.");
            }

            var newRoleProfile = new RoleProfile
            {
                Name = normalizedName
            };

            await dbContext.RoleProfiles.AddAsync(newRoleProfile, ct);
            await dbContext.SaveChangesAsync(ct);

            var response = new GenericRoleProfileResponse
            {
                Id = newRoleProfile.Id,
                Name = newRoleProfile.Name
            };

            return TypedResults.Ok(response);
        }
    }
}
