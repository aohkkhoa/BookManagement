using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookManagement2.Models.Entities
{
    [Table("User")]
    public class User
    {
        [Key]
        public int userId { get; set; }
        public string userName { get; set; }
        public byte[] PasswordSalt { get; set; }
        public byte[] PasswordHash { get; set; }
        public string address { get; set; }

        public string phone { get; set; }
    }
}
