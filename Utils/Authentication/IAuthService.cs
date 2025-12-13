using Microsoft.AspNetCore.Http;

namespace Medialityc.Utils.Authentication
{
    public interface IAuthService
    {
        string GenerateTokenLogin(string username, string password);
        bool ValidateUser(string token);
        bool ValidateRequest(HttpContext context);
    }
}
