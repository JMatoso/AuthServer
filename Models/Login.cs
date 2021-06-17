using System.ComponentModel.DataAnnotations;

namespace jwt_identity_api.Models
{
    public class Login
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}