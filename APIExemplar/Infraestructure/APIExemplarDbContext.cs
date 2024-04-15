using System;
using APIExemplar.Domain.Exemplars;
using APIExemplar.Infraestructure.Exemplars;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace APIExemplar.Infraestructure
{
    public class APIExemplarDbContext : DbContext
    {
        public DbSet<Exemplar> Exemplars { get; set; }
        
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateOnly>()
                .HaveConversion<DateOnlyConverter>()
                .HaveColumnType("date");
        }


        public APIExemplarDbContext(DbContextOptions options) : base(options)
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
            modelBuilder.ApplyConfiguration(new ExemplarEntityTypeConfiguration());
        }
    }
}