namespace jwt_identity_api.Models.Response
{
    public class AuthResponse
    {
        public int StatusCode { get; set; }
        public TokenResponse? Token { get; set; }
        public bool Successful { get; set; }
        public string? Message { get; set; }
        public Dictionary<object, object>? Errors { get; set; } = new();
    }

    public static class AuthResponseProvider
    {
        public static AuthResponse Set(string? message, int statusCode, bool successful = true, Dictionary<object, object>? errors = null, TokenResponse? tokenResponse = null)
        {
            return new AuthResponse()
            {
                StatusCode = statusCode,
                Token = tokenResponse,
                Successful = successful,
                Message = message,
                Errors = errors
            };
        }
    }
}