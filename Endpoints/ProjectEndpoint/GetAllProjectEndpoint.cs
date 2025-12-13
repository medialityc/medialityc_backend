using System;
using System.Linq;
using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.ProjectEndpoint.ProjectRequest;
using Medialityc.Endpoints.ProjectEndpoint.ProjectResponse;
using Medialityc.Utils.Authentication;
using Medialityc.Utils.Commons;
using Medialityc.Utils.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.ProjectEndpoint
{
    public class GetAllProjectEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<GetAllProjectRequest,
        Results<Ok<PaginatedResponse<GetAllProjectResponse>>, ProblemHttpResult, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Get("/projects/all");
            Description(x => x
                .WithName("GetAllProjects")
                .Produces<PaginatedResponse<GetAllProjectResponse>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status500InternalServerError));
        }

        public override async Task<Results<Ok<PaginatedResponse<GetAllProjectResponse>>, ProblemHttpResult, UnauthorizedHttpResult>>
            ExecuteAsync(GetAllProjectRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            try
            {
                var projectsQuery = dbContext.Projects
                    .Include(p => p.Network)
                    .AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim().ToLowerInvariant();
                    projectsQuery = projectsQuery.Where(p =>
                        p.Name.ToLower().Contains(search) ||
                        p.Description.ToLower().Contains(search) ||
                        p.Company.ToLower().Contains(search));
                }

                var projectsList = await projectsQuery.ToListAsync(ct);

                var totalCount = projectsList.Count;
                var page = request.Page ?? 1;
                var pageSize = request.PageSize ?? 10;

                var orderedProjects = projectsList
                    .OrderByProperty(request.SortBy ?? "Id", request.IsDescending ?? false)
                    .PaginatePage(page, pageSize);

                var responseData = orderedProjects.Select(p => new GetAllProjectResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Company = p.Company,
                    CreatedAt = p.CreatedAt,
                    Network = new NetworkResponseForProject
                    {
                        Id = p.Network?.Id ?? 0,
                        Instagram = p.Network?.Instagram ?? string.Empty,
                        Facebook = p.Network?.Facebook ?? string.Empty,
                        LinkedIn = p.Network?.LinkedIn ?? string.Empty,
                        Twitter = p.Network?.Twitter ?? string.Empty
                    }
                });

                var response = new PaginatedResponse<GetAllProjectResponse>
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
