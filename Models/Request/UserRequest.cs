using System.ComponentModel.DataAnnotations;

namespace jwt_identity_api.Models.Request
{
    public class UserRequest
    {
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [Phone]
        [Required]
        public string? PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}