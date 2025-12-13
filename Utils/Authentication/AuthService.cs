using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;

namespace Medialityc.Utils.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly AuthSettings _settings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IOptions<AuthSettings> settings, ILogger<AuthService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public string GenerateTokenLogin(string username, string password)
        {
            if (!IsValidCredentials(username, password))
            {
                _logger.LogWarning("Intento de autenticación fallido para el usuario {Username}.", username);
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_settings.TokenExpirationMinutes),
                Audience = _settings.Audience,
                Issuer = _settings.Issuer,
                SigningCredentials = credentials
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }

        public bool ValidateUser(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var handler = new JwtSecurityTokenHandler();

            try
            {
                handler.ValidateToken(token, GetValidationParameters(), out _);
                return true;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Token inválido.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al validar el token.");
                return false;
            }
        }

        public bool ValidateRequest(HttpContext context)
        {
            if (context?.Request == null)
                return false;

            if (!context.Request.Headers.TryGetValue("Authorization", out var rawHeader))
                return false;

            var token = rawHeader.ToString();

            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = token[7..].Trim();
            }

            if (string.IsNullOrWhiteSpace(token))
                return false;

            return ValidateUser(token);
        }

        private bool IsValidCredentials(string username, string password) =>
            string.Equals(username, _settings.Username, StringComparison.OrdinalIgnoreCase) &&
            password == _settings.Password;

        private TokenValidationParameters GetValidationParameters() => new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = _settings.Issuer,
            ValidAudience = _settings.Audience,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    }
}
