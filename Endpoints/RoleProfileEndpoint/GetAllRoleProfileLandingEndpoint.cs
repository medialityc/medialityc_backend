using System;
using System.Linq;
using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.RoleProfileEndpoint.RoleProfileRequest;
using Medialityc.Endpoints.RoleProfileEndpoint.RoleProfileResponse;
using Medialityc.Utils.Commons;
using Medialityc.Utils.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.RoleProfileEndpoint
{
    public class GetAllRoleProfileLandingEndpoint(IMedialitycDbContext dbContext) : Endpoint<GetAllRoleProfileRequest,
        Results<Ok<PaginatedResponse<GetAllRoleProfileResponse>>, ProblemHttpResult>>
    {
        public override void Configure()
        {
            Get("/role-profiles/landing/all");
            AllowAnonymous();
            Description(x => x
                .WithName("GetAllRoleProfilesLanding")
                .Produces<PaginatedResponse<GetAllRoleProfileResponse>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status500InternalServerError));
        }

        public override async Task<Results<Ok<PaginatedResponse<GetAllRoleProfileResponse>>, ProblemHttpResult>>
            ExecuteAsync(GetAllRoleProfileRequest request, CancellationToken ct)
        {
            try
            {
                var rolesQuery = dbContext.RoleProfiles.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim().ToLowerInvariant();
                    rolesQuery = rolesQuery.Where(rp => rp.Name.ToLower().Contains(search));
                }

                var rolesList = await rolesQuery.ToListAsync(ct);

                var totalCount = rolesList.Count;
                var page = request.Page ?? 1;
                var pageSize = request.PageSize ?? 10;

                var orderedRoles = rolesList
                    .OrderByProperty(request.SortBy ?? "Id", request.IsDescending ?? false)
                    .PaginatePage(page, pageSize);

                var responseData = orderedRoles.Select(rp => new GetAllRoleProfileResponse
                {
                    Id = rp.Id,
                    Name = rp.Name
                });

                var response = new PaginatedResponse<GetAllRoleProfileResponse>
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
