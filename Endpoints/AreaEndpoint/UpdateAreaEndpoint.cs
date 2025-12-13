using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.AreaEndpoint.AreaRequest;
using Medialityc.Endpoints.AreaEndpoint.AreaResponse;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.AreaEndpoint
{
    public class UpdateAreaEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<UpdateAreaRequest,
        Results<Ok<GenericAreaResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Put("/areas/update");
        }

        public override async Task<Results<Ok<GenericAreaResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
            ExecuteAsync(UpdateAreaRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            var area = await dbContext.Areas
                .FirstOrDefaultAsync(a => a.Id == request.Id, ct);

            if (area == null)
            {
                return TypedResults.BadRequest($"El área con ID '{request.Id}' no existe.");
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var normalizedName = request.Name.Trim();

                var duplicatedArea = await dbContext.Areas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id != request.Id && a.Name == normalizedName, ct);

                if (duplicatedArea != null)
                {
                    return TypedResults.Conflict($"El área con el nombre '{normalizedName}' ya existe.");
                }

                area.Name = normalizedName;
            }

            await dbContext.SaveChangesAsync(ct);

            var response = new GenericAreaResponse
            {
                Id = area.Id,
                Name = area.Name
            };

            return TypedResults.Ok(response);
        }
    }
}
