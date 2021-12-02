using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;

namespace jwt_identity_api.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetDomainPath(this HttpContext httpContext)
        {
            return $"{httpContext.Request.Host}/";
        }

        public static string TokenFromHeader(this HttpContext context) => context.Request.Headers["Authorization"].ToString().Split(" ")[1];

        public static string GetDataFromJwtToken(this HttpContext context, string type = JwtRegisteredClaimNames.Sub)
        {
            var token = TokenFromHeader(context);
            var jwtSecurityToken = (new JwtSecurityTokenHandler()).ReadJwtToken(token);
            var userId = jwtSecurityToken.Claims.Single(x => x.Type.Equals(type)).Value;
            
            return string.IsNullOrEmpty(userId) ? string.Empty : userId;
        }

        public static void SetHeaderResponseJson<T>(this HttpContext context, string key, object data)
        {
            context.Response.Headers[key] = JsonConvert.SerializeObject((T)data);
        }
    }
}