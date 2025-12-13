using System;
using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.RoleProfileEndpoint.RoleProfileRequest;
using Medialityc.Endpoints.RoleProfileEndpoint.RoleProfileResponse;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.RoleProfileEndpoint
{
    public class GetRoleProfileByIdLandingEndpoint(IMedialitycDbContext dbContext) : Endpoint<GetRoleProfileByIdRequest,
        Results<Ok<GetRoleProfileByIdResponse>, NotFound<string>, ProblemHttpResult>>
    {
        public override void Configure()
        {
            Get("/role-profiles/landing/{Id}");
            AllowAnonymous();
            Description(x => x
                .WithName("GetRoleProfileByIdLanding")
                .WithSummary("Obtiene un rol para landing por ID")
                .Produces<GetRoleProfileByIdResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status404NotFound));
        }

        public override async Task<Results<Ok<GetRoleProfileByIdResponse>, NotFound<string>, ProblemHttpResult>>
            ExecuteAsync(GetRoleProfileByIdRequest request, CancellationToken ct)
        {
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
