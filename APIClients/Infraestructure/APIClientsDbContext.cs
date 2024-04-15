using APIClients.Domain.Clients;
using APIClients.Infraestructure.Books;
using Microsoft.EntityFrameworkCore;

namespace APIClients.Infraestructure
{
    public class APIClientsDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }


        public APIClientsDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ClientEntityTypeConfiguration());
        }
    }
}