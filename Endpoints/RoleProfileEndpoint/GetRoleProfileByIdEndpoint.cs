using System;
using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.RoleProfileEndpoint.RoleProfileRequest;
using Medialityc.Endpoints.RoleProfileEndpoint.RoleProfileResponse;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.RoleProfileEndpoint
{
    public class GetRoleProfileByIdEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<GetRoleProfileByIdRequest,
        Results<Ok<GetRoleProfileByIdResponse>, NotFound<string>, ProblemHttpResult, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Get("/role-profiles/{Id}");
            AllowAnonymous();
            Description(x => x
                .WithName("GetRoleProfileById")
                .WithSummary("Obtiene un rol por ID")
                .Produces<GetRoleProfileByIdResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status404NotFound));
        }

        public override async Task<Results<Ok<GetRoleProfileByIdResponse>, NotFound<string>, ProblemHttpResult, UnauthorizedHttpResult>>
            ExecuteAsync(GetRoleProfileByIdRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            try
            {
                var roleProfile = await dbContext.RoleProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(rp => rp.Id == request.Id, ct);

                if (roleProfile == null)
                {
                    return TypedResults.NotFound($"Rol con ID {request.Id} no encontrado.");
                }

                var response = new GetRoleProfileByIdResponse
                {
                    Id = roleProfile.Id,
                    Name = roleProfile.Name
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
