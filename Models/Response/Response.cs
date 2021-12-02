namespace jwt_identity_api.Models.Response
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public bool Successful { get;set; }
        public string? Token { get; set; }
        public Dictionary<object, object>? Errors { get; set; } = new();
    }

    public static class ResponseProvider
    {
        public static Response Set(int statusCode, string message, bool successful = true, Dictionary<object, object>? errors = null, string? token = null)
        {
            return new Response()
            {
                StatusCode = statusCode,
                Message = message,
                Successful = successful,
                Errors = errors,
                Token = token
            };
        }
    }
}