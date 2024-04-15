using APIPublisher.Domain.Publishers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APIPublisher.Infraestructure.Publishers;

internal class PublisherEntityTypeConfiguration : IEntityTypeConfiguration<Publisher>
{
    public void Configure(EntityTypeBuilder<Publisher> builder)
    {
        builder.ToTable("Publishers", SchemaNames.APIPublisher);
        builder.HasKey(publisher => publisher.Id);
        builder.Property(publisher => publisher.Name)
            .HasColumnName("Name")
            .IsRequired();
        builder.OwnsOne(publisher => publisher.Country, Country =>
        {
            Country.Property(property => property.Name)
                .HasColumnName("Country")
                .IsRequired();
        }).Navigation(publisher => publisher.Country).IsRequired();
    }
}