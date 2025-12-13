using System;
using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.WorkProfileEndpoint.WorkProfileRequest;
using Medialityc.Endpoints.WorkProfileEndpoint.WorkProfileResponse;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.WorkProfileEndpoint
{
    public class GetWorkProfileByIdEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<GetWorkProfileByIdRequest,
        Results<Ok<GetWorkProfileByIdResponse>, NotFound<string>, ProblemHttpResult, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Get("/work-profiles/{Id}");
            AllowAnonymous();
            Description(x => x
                .WithName("GetWorkProfileById")
                .WithSummary("Obtiene un perfil de trabajo por ID")
                .Produces<GetWorkProfileByIdResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status404NotFound));
        }

        public override async Task<Results<Ok<GetWorkProfileByIdResponse>, NotFound<string>, ProblemHttpResult, UnauthorizedHttpResult>>
            ExecuteAsync(GetWorkProfileByIdRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            try
            {
                var workProfile = await dbContext.WorkProfiles
                    .Include(wp => wp.Area)
                    .Include(wp => wp.Role)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(wp => wp.Id == request.Id, ct);

                if (workProfile == null)
                {
                    return TypedResults.NotFound($"Perfil con ID {request.Id} no encontrado.");
                }

                var response = new GetWorkProfileByIdResponse
                {
                    Id = workProfile.Id,
                    FirsName = workProfile.FirsName,
                    LastName = workProfile.LastName,
                    AreaId = workProfile.Area.Id,
                    RoleProfileId = workProfile.Role.Id,
                    Email = workProfile.Email,
                    GitHubProfile = workProfile.GitHubProfile,
                    Image = workProfile.Image,
                    ReviewStars = workProfile.ReviewStars,
                    OverallReview = workProfile.OverallReview
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
