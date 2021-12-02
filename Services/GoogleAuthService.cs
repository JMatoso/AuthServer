namespace jwt_identity_api.Services
{
    public class GoogleAuthService
    {
        //grant_type authorization_code
        private const string TokenValidationUrl = "https://www.googleapis.com/oauth2/v4/token?client_id={0}&client_secret={1}&redirect_uri={2}&grant_type={3}&code={4}";        
    }
}