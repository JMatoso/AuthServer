using System.ComponentModel.DataAnnotations;

namespace jwt_identity_api.Models.Request
{
    public class UpdateUserRequest
    {
        [Required]
        [StringLength(60)]
        public string? Name { get; set; }

        [Phone]
        [Required]
        public string? PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}