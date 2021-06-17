using System;

namespace jwt_identity_api.Models
{
    public class TokenReturned
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}