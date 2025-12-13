using System;
using System.Linq;
using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.WorkProfileEndpoint.WorkProfileRequest;
using Medialityc.Endpoints.WorkProfileEndpoint.WorkProfileResponse;
using Medialityc.Utils.Authentication;
using Medialityc.Utils.Commons;
using Medialityc.Utils.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.WorkProfileEndpoint
{
    public class GetAllWorkProfileEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<GetAllWorkProfileRequest,
        Results<Ok<PaginatedResponse<GetAllWorkProfileResponse>>, ProblemHttpResult, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Get("/work-profiles/all");
            AllowAnonymous();
            Description(x => x
                .WithName("GetAllWorkProfiles")
                .Produces<PaginatedResponse<GetAllWorkProfileResponse>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status500InternalServerError));
        }

        public override async Task<Results<Ok<PaginatedResponse<GetAllWorkProfileResponse>>, ProblemHttpResult, UnauthorizedHttpResult>>
            ExecuteAsync(GetAllWorkProfileRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            try
            {
                var workProfilesQuery = dbContext.WorkProfiles
                    .Include(wp => wp.Area)
                    .Include(wp => wp.Role)
                    .AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim().ToLowerInvariant();
                    workProfilesQuery = workProfilesQuery.Where(wp =>
                        wp.FirsName.ToLower().Contains(search) ||
                        wp.LastName.ToLower().Contains(search) ||
                        wp.Email.ToLower().Contains(search) ||
                        wp.GitHubProfile.ToLower().Contains(search));
                }

                var workProfilesList = await workProfilesQuery.ToListAsync(ct);

                var totalCount = workProfilesList.Count;
                var page = request.Page ?? 1;
                var pageSize = request.PageSize ?? 10;

                var orderedWorkProfiles = workProfilesList
                    .OrderByProperty(request.SortBy ?? "Id", request.IsDescending ?? false)
                    .PaginatePage(page, pageSize);

                var responseData = orderedWorkProfiles.Select(wp => new GetAllWorkProfileResponse
                {
                    Id = wp.Id,
                    FirsName = wp.FirsName,
                    LastName = wp.LastName,
                    AreaId = wp.Area.Id,
                    RoleProfileId = wp.Role.Id,
                    Email = wp.Email,
                    GitHubProfile = wp.GitHubProfile,
                    Image = wp.Image,
                    ReviewStars = wp.ReviewStars,
                    OverallReview = wp.OverallReview
                });

                var response = new PaginatedResponse<GetAllWorkProfileResponse>
                {
                    Data = responseData,
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount
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
