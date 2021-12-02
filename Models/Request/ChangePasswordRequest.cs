#nullable disable

using System.ComponentModel.DataAnnotations;

namespace jwt_identity_api.Models.Request
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Campo obrigatório.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}