#nullable disable

using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace jwt_identity_api.Extensions
{
    public static class GeneralExtensions
    {
        public static string TokenFromHeader(this HttpContext context) => context.Request.Headers["Authorization"].ToString().Split(" ")[1];

        public static string GetDataFromJwtToken(this HttpContext context, string type = JwtRegisteredClaimNames.Sub)
        {
            var token = TokenFromHeader(context);
            var jwtSecurityToken = (new JwtSecurityTokenHandler()).ReadJwtToken(token);
            var userId = jwtSecurityToken.Claims.Single(x => x.Type.Equals(type)).Value;
            
            return string.IsNullOrEmpty(userId) ? string.Empty : userId;
        }

        public static string GenerateCode()
        {
            var rnd = new Random();
            return rnd.Next(1000000, 9999999).ToString();
        }

        public static void SetHeaderResponseJson<T>(this HttpContext context, string key, object data)
        {
            context.Response.Headers[key] = JsonConvert.SerializeObject((T)data);
        }

        public static Dictionary<object, object> SetErrors(IEnumerable<Microsoft.AspNetCore.Mvc.ModelBinding.ModelError> errors)
        {
            var errorList = new Dictionary<object, object>();

            foreach (var err in errors) errorList[key: err.Exception.Source] = err.ErrorMessage;
            return errorList;
        }

        public static Dictionary<object, object> SetErrors(IEnumerable<IdentityError> errors)
        {
            var errorList = new Dictionary<object, object>();

            foreach (var err in errors) errorList[key: err.Code] = err.Description;
            return errorList;
        }
    }
}