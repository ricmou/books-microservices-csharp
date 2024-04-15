using APICategories.Domain.Books;
using APICategories.Domain.Categories;
using APICategories.Infraestructure.Books;
using APICategories.Infraestructure.Categories;
using Microsoft.EntityFrameworkCore;

namespace APICategories.Infraestructure
{
    public class APICategoriesDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        
        public DbSet<Category> Categories { get; set; }


        public APICategoriesDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryEntityTypeConfiguration());
        }
    }
}