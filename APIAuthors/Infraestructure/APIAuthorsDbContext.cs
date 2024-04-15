using System;
using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Books;
using APIAuthors.Infraestructure.Authors;
using APIAuthors.Infraestructure.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace APIAuthors.Infraestructure
{
    public class ApiAuthorsDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateOnly>()
                .HaveConversion<DateOnlyConverter>()
                .HaveColumnType("date");
        }

        public ApiAuthorsDbContext(DbContextOptions options) : base(options)
        {
            
        }
        
        //Workaround for DateOnly not being supported natively
        //https://github.com/dotnet/efcore/issues/24507#issuecomment-891034323
        public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
        {
            public DateOnlyConverter() : base(
                d => d.ToDateTime(TimeOnly.MinValue),
                d => DateOnly.FromDateTime(d))
            { }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AuthorEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BookEntityTypeConfiguration());
        }
    }
}