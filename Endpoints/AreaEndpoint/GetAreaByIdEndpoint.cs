using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.AreaEndpoint.AreaRequest;
using Medialityc.Endpoints.AreaEndpoint.AreaResponse;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.AreaEndpoint
{
    public class GetAreaByIdEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<GetAreaByIdRequest,
        Results<Ok<GetAreaByIdResponse>, NotFound<string>, ProblemHttpResult, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Get("/areas/{Id}");
            Description(x => x
                .WithName("GetAreaById")
                .WithSummary("Obtiene un área por ID")
                .Produces<GetAreaByIdResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status404NotFound));
        }

        public override async Task<Results<Ok<GetAreaByIdResponse>, NotFound<string>, ProblemHttpResult, UnauthorizedHttpResult>>
            ExecuteAsync(GetAreaByIdRequest req, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            try
            {
                var area = await dbContext.Areas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == req.Id, ct);

                if (area == null)
                {
                    return TypedResults.NotFound($"Área con ID {req.Id} no encontrada.");
                }

                var response = new GetAreaByIdResponse
                {
                    Id = area.Id,
                    Name = area.Name
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
