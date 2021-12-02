#nullable disable

using Microsoft.AspNetCore.Identity;

namespace jwt_identity_api.Extensions
{
    public static class GeneralExtensions
    {
        public static string GenerateCode()
        {
            var rnd = new Random();
            return rnd.Next(1000000, 9999999).ToString();
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