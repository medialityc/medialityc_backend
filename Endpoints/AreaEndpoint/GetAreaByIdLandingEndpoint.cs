using System;
using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.AreaEndpoint.AreaRequest;
using Medialityc.Endpoints.AreaEndpoint.AreaResponse;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.AreaEndpoint
{
    public class GetAreaByIdLandingEndpoint(IMedialitycDbContext dbContext) : Endpoint<GetAreaByIdRequest,
        Results<Ok<GetAreaByIdResponse>, NotFound<string>, ProblemHttpResult>>
    {
        public override void Configure()
        {
            Get("/areas/landing/{Id}");
            AllowAnonymous();
            Description(x => x
                .WithName("GetAreaByIdLanding")
                .WithSummary("Obtiene un área para landing por ID")
                .Produces<GetAreaByIdResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .Produces<string>(StatusCodes.Status404NotFound));
        }

        public override async Task<Results<Ok<GetAreaByIdResponse>, NotFound<string>, ProblemHttpResult>>
            ExecuteAsync(GetAreaByIdRequest request, CancellationToken ct)
        {
            try
            {
                var area = await dbContext.Areas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == request.Id, ct);

                if (area == null)
                {
                    return TypedResults.NotFound($"Área con ID {request.Id} no encontrada.");
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
