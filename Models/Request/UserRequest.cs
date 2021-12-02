using System.ComponentModel.DataAnnotations;

namespace jwt_identity_api.Models.Request
{
    public class UserRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        [StringLength(50, ErrorMessage = "Até 50 carácters.")]
        public string? Nome { get; set; }

        [Phone(ErrorMessage = "Insira um número válido.")]
        [Required(ErrorMessage = "Campo obrigatório.")]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        [EmailAddress(ErrorMessage = "Insira um e-mail válido.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Campo obrigatório.")]
        public string? Role { get; set; }
        
        public string? Picture { get; set; }
        
        [Required(ErrorMessage = "Campo obrigatório.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}