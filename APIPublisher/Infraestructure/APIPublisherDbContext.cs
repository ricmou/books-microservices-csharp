using APIPublisher.Domain.Books;
using APIPublisher.Domain.Publishers;
using APIPublisher.Infraestructure.Books;
using APIPublisher.Infraestructure.Publishers;
using Microsoft.EntityFrameworkCore;

namespace APIPublisher.Infraestructure
{
    public class APIPublisherDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Publisher> Publishers { get; set; }


        public APIPublisherDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PublisherEntityTypeConfiguration());
        }
    }
}