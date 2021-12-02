using jwt_identity_api.Domains.Externals.Contracts;
using jwt_identity_api.Options;
using Newtonsoft.Json;

namespace jwt_identity_api.Services
{
    public interface IFacebookAuthService
    {
        Task<FacebookUserInfoResult?> GetUserInfoAsync(string accessToken);
        Task<FacebookTokenValidationResult?> ValidateAccessTokenAsync(string accessToken);
    }

    public class FacebookAuthService : IFacebookAuthService
    {
        private readonly FacebookOptions _facebookOption;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string TokenValidationUrl = "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}|{2}";
        private const string UserInfoUrl = "https://graph.facebook.com/me?fields=first_name,last_name,picture,email&access_token={0}";

        public FacebookAuthService(IHttpClientFactory httpClientFactory, FacebookOptions facebookOption)
        {
            _httpClientFactory = httpClientFactory;
            _facebookOption = facebookOption;
        }

        public async Task<FacebookUserInfoResult?> GetUserInfoAsync(string accessToken)
        {
            var formattedUrl = string.Format(UserInfoUrl, accessToken);
            var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);

            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookUserInfoResult>(response);
        }

        public async Task<FacebookTokenValidationResult?> ValidateAccessTokenAsync(string accessToken)
        {
            var formattedUrl = string
                .Format(TokenValidationUrl, accessToken, _facebookOption.AppId, _facebookOption.AppSecret);

            var result = await _httpClientFactory.CreateClient().GetAsync(formattedUrl);

            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookTokenValidationResult>(response);
        }
    }
}