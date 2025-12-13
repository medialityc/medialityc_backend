using System;
using System.Linq;
using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.ReviewProjectEndpoint.ReviewProjectRequest;
using Medialityc.Endpoints.ReviewProjectEndpoint.ReviewProjectResponse;
using Medialityc.Utils.Commons;
using Medialityc.Utils.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.ReviewProjectEndpoint
{
    public class GetAllReviewProjectLandingEndpoint(IMedialitycDbContext dbContext) : Endpoint<GetAllReviewProjectRequest,
        Results<Ok<PaginatedResponse<GetAllReviewProjectResponse>>, ProblemHttpResult>>
    {
        public override void Configure()
        {
            Get("/review-projects/landing/all");
            AllowAnonymous();
            Description(x => x
                .WithName("GetAllReviewProjectsLanding")
                .Produces<PaginatedResponse<GetAllReviewProjectResponse>>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status500InternalServerError));
        }

        public override async Task<Results<Ok<PaginatedResponse<GetAllReviewProjectResponse>>, ProblemHttpResult>>
            ExecuteAsync(GetAllReviewProjectRequest request, CancellationToken ct)
        {
            try
            {
                var reviewsQuery = dbContext.ReviewProjects.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(request.Search))
                {
                    var search = request.Search.Trim().ToLowerInvariant();
                    reviewsQuery = reviewsQuery.Where(rp =>
                        rp.SpecificReview.ToLower().Contains(search) ||
                        rp.ProjectId.ToString().Contains(search) ||
                        rp.WorkProfileId.ToString().Contains(search));
                }

                var reviewsList = await reviewsQuery.ToListAsync(ct);

                var totalCount = reviewsList.Count;
                var page = request.Page ?? 1;
                var pageSize = request.PageSize ?? 10;

                var orderedReviews = reviewsList
                    .OrderByProperty(request.SortBy ?? "Id", request.IsDescending ?? false)
                    .PaginatePage(page, pageSize);

                var responseData = orderedReviews.Select(rp => new GetAllReviewProjectResponse
                {
                    Id = rp.Id,
                    ProjectId = rp.ProjectId,
                    WorkProfileId = rp.WorkProfileId,
                    SpecificReview = rp.SpecificReview,
                    PerformanceEvaluation = rp.PerformanceEvaluation
                });

                var response = new PaginatedResponse<GetAllReviewProjectResponse>
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
