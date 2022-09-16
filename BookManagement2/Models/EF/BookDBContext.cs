using BookManagement2.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookManagement2.Models.EF
{
    public class BookDBContext : DbContext
    {
        protected readonly IConfiguration _Configuration;

        public BookDBContext(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server with connection string from app settings
            options.UseSqlServer(_Configuration.GetConnectionString("MyDB"));
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Logins { get; set; }



    }
}
