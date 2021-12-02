#nullable disable

using System.ComponentModel.DataAnnotations;

namespace jwt_identity_api.Models.Request
{
    public class ChangePasswordRequest
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}