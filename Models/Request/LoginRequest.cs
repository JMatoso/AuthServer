using System.ComponentModel.DataAnnotations;

namespace jwt_identity_api.Models.Request
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Campo obrigatório.")]
        public string? User { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}