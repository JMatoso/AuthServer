using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace jwt_identity_api.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(60)]
        public string? Name { get; set; }
        [Required]
        public UserStatus UserStatus { get; set; }

        [Required]
        public string? Role { get; set; }

        public string? FacebookId { get; set; }

        public bool IsThirtyUser { get; set; }

        public bool HasPasswordChanged { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }
    }

    public enum UserStatus
    {
        Blocked = 1,
        Active = 2
    }
}