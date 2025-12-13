using Medialityc.Endpoints.AuthEndpoint.AuthRequest;
using Medialityc.Endpoints.AuthEndpoint.AuthResponse;
using Medialityc.Utils.Authentication;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace Medialityc.Endpoints.AuthEndpoint
{
    public class LoginEndpoint(IAuthService authService, IOptions<AuthSettings> options) :
        Endpoint<LoginRequest, Results<Ok<LoginResponse>, UnauthorizedHttpResult>>
    {
        private readonly AuthSettings _settings = options.Value;

        public override void Configure()
        {
            Post("/auth/login");
            AllowAnonymous();
        }

        public override async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult>>
            ExecuteAsync(LoginRequest request, CancellationToken ct)
        {
            await Task.Yield();

            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return TypedResults.Unauthorized();

            try
            {
                var token = authService.GenerateTokenLogin(request.Username, request.Password);
                var response = new LoginResponse
                {
                    Token = token,
                    TokenType = "Bearer",
                    ExpiresInMinutes = _settings.TokenExpirationMinutes
                };

                return TypedResults.Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return TypedResults.Unauthorized();
            }
        }
    }
}
