using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookManagement2.Models.Entities
{

    [Table("Category")]
    public class Category
    {
        [Key]
        public int categoryId { get; set; }
        [Required]
        public string? categoryName { get; set; }

    }
}
