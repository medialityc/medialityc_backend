using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.AreaEndpoint.AreaRequest;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Medialityc.Endpoints.AreaEndpoint
{
    public class DeleteAreaEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<DeleteAreaRequest,
        Results<Ok<string>, Conflict<string>, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Delete("/areas/delete");
        }

        public override async Task<Results<Ok<string>, Conflict<string>, UnauthorizedHttpResult>>
            ExecuteAsync(DeleteAreaRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            var area = await dbContext.Areas.FindAsync(new object[] { request.Id }, ct);

            if (area == null)
            {
                return TypedResults.Conflict($"El área con ID '{request.Id}' no existe.");
            }

            dbContext.Areas.Remove(area);
            await dbContext.SaveChangesAsync(ct);

            return TypedResults.Ok($"Área con ID '{request.Id}' eliminada exitosamente.");
        }
    }
}
