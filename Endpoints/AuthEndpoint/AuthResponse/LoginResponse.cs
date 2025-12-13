namespace Medialityc.Endpoints.AuthEndpoint.AuthResponse
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string TokenType { get; set; } = string.Empty;
        public int ExpiresInMinutes { get; set; }
    }
}
