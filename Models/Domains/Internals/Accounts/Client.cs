using System.ComponentModel.DataAnnotations;

namespace jwt_identity_api.Models.Domains.Internals.Accounts
{
    public class Client
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid? UserId { get; set; }
        
        public string? Genre { get; set; }

    }
}