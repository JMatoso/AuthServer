namespace jwt_identity_api.Models
{
    public class Result
    {
        public int? Code { get; set; }
        public object? Content { get; set; }
        public Dictionary<object, object>? Errors { get; set; } 
        public bool Successful { get; set; }
        public DateTime TimeStamp { get; set; } = DateTime.Now;
    }

    public static class ResultProvider
    {
        public static Result Set(int? code, object? content, bool sucessful, Dictionary<object, object>? errors = null)
        {
            return new Result
            {
                Code = code,
                Content = content,
                Errors = errors,
                Successful = sucessful
            };
        }
    }
}