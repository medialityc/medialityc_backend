using FastEndpoints;
using System.Linq;
using Medialityc.Data;
using Medialityc.Endpoints.AreaEndpoint.AreaRequest;
using Medialityc.Endpoints.AreaEndpoint.AreaResponse;
using Medialityc.Utils.Authentication;
using Medialityc.Utils.Commons;
using Medialityc.Utils.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.AreaEndpoint
{
    public class GetAllAreaEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<GetAllAreaRequest,
        Results<Ok<PaginatedResponse<GetAllAreaResponse>>, ProblemHttpResult, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Get("/areas/all");
            Description(x => x
                .WithName("GetAllAreas")
                .Produces<PaginatedResponse<GetAllAreaResponse>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status500InternalServerError));
        }

        public override async Task<Results<Ok<PaginatedResponse<GetAllAreaResponse>>, ProblemHttpResult, UnauthorizedHttpResult>>
            ExecuteAsync(GetAllAreaRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            try
            {
                var areasQuery = dbContext.Areas.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim().ToLowerInvariant();
                    areasQuery = areasQuery.Where(a => a.Name.ToLower().Contains(search));
                }

                var areasList = await areasQuery.ToListAsync(ct);

                var totalCount = areasList.Count;
                var page = request.Page ?? 1;
                var pageSize = request.PageSize ?? 10;

                var orderedAreas = areasList
                    .OrderByProperty(request.SortBy ?? "Id", request.IsDescending ?? false)
                    .PaginatePage(page, pageSize);

                var responseData = orderedAreas.Select(a => new GetAllAreaResponse
                {
                    Id = a.Id,
                    Name = a.Name
                });

                var response = new PaginatedResponse<GetAllAreaResponse>
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
