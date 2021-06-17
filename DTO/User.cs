using System.ComponentModel.DataAnnotations;

namespace jwt_identity_api.DTO
{
    public class User
    {
        [Required]
        [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$")]
        public string FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}