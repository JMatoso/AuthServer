namespace jwt_identity_api.Models.Response
{
    public class TokenResponse
    {
        public string? Token { get; set; }
        public string? Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}