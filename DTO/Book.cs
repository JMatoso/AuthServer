using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace jwt_identity_api.DTO
{
    [Table("Books")]
    public class Book
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$")]
        [Required]
        public string Author { get; set; }

        [Range(1500, 5000)]
        public int Year { get; set; }
    }
}