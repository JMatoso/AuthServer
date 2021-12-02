#nullable disable

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jwt_identity_api.DTO
{
    [Table("Recoveries")]
    public class Recovery
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public bool IsValid { get; set; }

        [DataType(DataType.Password)]
        public string Code { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime ExpireTime { get; set; }
    }
}