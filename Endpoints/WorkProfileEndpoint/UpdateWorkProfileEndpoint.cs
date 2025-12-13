using FastEndpoints;
using Medialityc.Data;
using Medialityc.Endpoints.WorkProfileEndpoint.WorkProfileRequest;
using Medialityc.Endpoints.WorkProfileEndpoint.WorkProfileResponse;
using Medialityc.Utils.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Medialityc.Endpoints.WorkProfileEndpoint
{
    public class UpdateWorkProfileEndpoint(IMedialitycDbContext dbContext, IAuthService authService) : Endpoint<UpdateWorkProfileRequest,
        Results<Ok<GenericWorkProfileResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
    {
        public override void Configure()
        {
            Put("/work-profiles/update");
            AllowAnonymous();
        }

        public override async Task<Results<Ok<GenericWorkProfileResponse>, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult>>
            ExecuteAsync(UpdateWorkProfileRequest request, CancellationToken ct)
        {
            if (!authService.ValidateRequest(HttpContext))
            {
                return TypedResults.Unauthorized();
            }

            var workProfile = await dbContext.WorkProfiles
                .Include(wp => wp.Area)
                .Include(wp => wp.Role)
                .FirstOrDefaultAsync(wp => wp.Id == request.Id, ct);

            if (workProfile == null)
            {
                return TypedResults.BadRequest($"El perfil con ID '{request.Id}' no existe.");
            }

            if (!string.IsNullOrWhiteSpace(request.FirsName))
            {
                workProfile.FirsName = request.FirsName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(request.LastName))
            {
                workProfile.LastName = request.LastName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var normalizedEmail = request.Email.Trim();
                var duplicateEmail = await dbContext.WorkProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(wp => wp.Id != request.Id && wp.Email == normalizedEmail, ct);

                if (duplicateEmail != null)
                {
                    return TypedResults.Conflict($"Ya existe un perfil con el correo '{normalizedEmail}'.");
                }

                workProfile.Email = normalizedEmail;
            }

            if (!string.IsNullOrWhiteSpace(request.GitHubProfile))
            {
                workProfile.GitHubProfile = request.GitHubProfile.Trim();
            }

            if (!string.IsNullOrWhiteSpace(request.Image))
            {
                workProfile.Image = request.Image.Trim();
            }

            if (request.ReviewStars.HasValue)
            {
                workProfile.ReviewStars = request.ReviewStars.Value;
            }

            if (!string.IsNullOrWhiteSpace(request.OverallReview))
            {
                workProfile.OverallReview = request.OverallReview.Trim();
            }

            if (request.AreaId.HasValue)
            {
                var area = await dbContext.Areas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == request.AreaId.Value, ct);

                if (area == null)
                {
                    return TypedResults.BadRequest($"El área con ID '{request.AreaId.Value}' no existe.");
                }

                workProfile.Area = area;
            }

            if (request.RoleProfileId.HasValue)
            {
                var roleProfile = await dbContext.RoleProfiles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(rp => rp.Id == request.RoleProfileId.Value, ct);

                if (roleProfile == null)
                {
                    return TypedResults.BadRequest($"El rol con ID '{request.RoleProfileId.Value}' no existe.");
                }

                workProfile.Role = roleProfile;
            }

            await dbContext.SaveChangesAsync(ct);

            var response = new GenericWorkProfileResponse
            {
                Id = workProfile.Id,
                FirsName = workProfile.FirsName,
                LastName = workProfile.LastName,
                AreaId = workProfile.Area.Id,
                RoleProfileId = workProfile.Role.Id,
                Email = workProfile.Email,
                GitHubProfile = workProfile.GitHubProfile,
                Image = workProfile.Image,
                ReviewStars = workProfile.ReviewStars,
                OverallReview = workProfile.OverallReview
            };

            return TypedResults.Ok(response);
        }
    }
}
