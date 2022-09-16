using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookManagement2.Models.Entities
{
    [Table("Book")]
    public class Book

    {
        [Key]
        public int bookId { get; set; }
        [Required]
        public string title { get; set; }
        public int categoryId { get; set; }
    }
}
