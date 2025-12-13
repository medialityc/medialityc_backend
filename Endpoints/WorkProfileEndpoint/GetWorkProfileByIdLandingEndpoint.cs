using System;
using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.WorkProfileEndpoint.WorkProfileRequest;
using Medialityc.Endpoints.WorkProfileEndpoint.WorkProfileResponse;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.WorkProfileEndpoint
{
    public class GetWorkProfileByIdLandingEndpoint(IMedialitycDbContext dbContext) : Endpoint<GetWorkProfileByIdRequest,
        Results<Ok<GetWorkProfileByIdResponse>, NotFound<string>, ProblemHttpResult>>
    {
        public override void Configure()
        {
            Get("/work-profiles/landing/{Id}");
            AllowAnonymous();
            Description(x => x
                .WithName("GetWorkProfileByIdLanding")
                .WithSummary("Obtiene un perfil de trabajo para landing por ID")
                .Produces<GetWorkProfileByIdResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status404NotFound));
        }

        public override async Task<Results<Ok<GetWorkProfileByIdResponse>, NotFound<string>, ProblemHttpResult>>
            ExecuteAsync(GetWorkProfileByIdRequest request, CancellationToken ct)
        {
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
