namespace jwt_identity_api.Options
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string AudienceToken { get; set; }
        public string IssuerToken { get; set; }
        public int ExpireMinutes { get; set; }
    }
}