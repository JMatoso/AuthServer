namespace jwt_identity_api
{
    public static class JwtSettings
    {
        public static string SecretKey = "tKE+pMd2rQAHBbOjXWTZqacLJRLqlrnTzZdmKRJEXLjtiGOnFY3w+vuUxPSgLdMFbbVXxPrFWNUd/yQyG5PsEg==";
        public static string AudienceToken = "https://localhost:44381";
        public static string IssuerToken = "https://localhost:44381";
        public static int ExpireMinutes = 60;
    }
}