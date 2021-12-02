namespace jwt_identity_api.Options
{
    public class JwtOptions
    {
        public string SecretKey { get; set; } = string.Empty;
        public string? AudienceToken { get; set; }
        public string? IssuerToken { get; set; }
        public int ExpireMinutes { get; set; }
        public DateTime TokenLifeTime { get; set; }
    }
}
