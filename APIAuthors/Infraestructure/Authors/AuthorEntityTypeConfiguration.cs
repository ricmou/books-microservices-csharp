using APIAuthors.Domain.Authors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APIAuthors.Infraestructure.Authors
{
    internal class AuthorEntityTypeConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable("Authors", SchemaNames.APIAuthors);
            builder.HasKey(author => author.Id);
            builder.OwnsOne(author => author.Name, Name =>
            {
                Name.Property(property => property.FirstName)
                    .HasColumnName("FirstName")
                    .IsRequired();
                Name.Property(property => property.LastName)
                    .HasColumnName("LastName")
                    .IsRequired();
            }).Navigation(author => author.Name).IsRequired();
            builder.Property(author => author.BirthDate)
                .HasColumnName("BirthDate")
                .IsRequired();
            builder.OwnsOne(author => author.Country, Country =>
            {
                Country.Property(property => property.Name)
                    .HasColumnName("Country")
                    .IsRequired();
            }).Navigation(author => author.Country).IsRequired();
        }
    }
}