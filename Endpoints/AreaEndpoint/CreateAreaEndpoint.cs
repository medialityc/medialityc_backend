using FastEndpoints;
using Medialityc.Data;
using Medialityc.Data.Models;
using Medialityc.Endpoints.AreaEndpoint.AreaRequest;
using Medialityc.Endpoints.AreaEndpoint.AreaResponse;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.AreaEndpoint
{
    public class CreateAreaEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<CreateAreaRequest,
        Results<Ok<GenericAreaResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Post("/areas/create");
        }

        public override async Task<Results<Ok<GenericAreaResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
            ExecuteAsync(CreateAreaRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            var normalizedName = request.Name.Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                return TypedResults.BadRequest("El nombre del área es requerido.");
            }

            var existingArea = await dbContext.Areas
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Name == normalizedName, ct);

            if (existingArea != null)
            {
                return TypedResults.Conflict($"El área con el nombre '{normalizedName}' ya existe.");
            }

            var newArea = new Area
            {
                Name = normalizedName
            };

            await dbContext.Areas.AddAsync(newArea, ct);
            await dbContext.SaveChangesAsync(ct);

            var response = new GenericAreaResponse
            {
                Id = newArea.Id,
                Name = newArea.Name
            };

            return TypedResults.Ok(response);
        }
    }
}
