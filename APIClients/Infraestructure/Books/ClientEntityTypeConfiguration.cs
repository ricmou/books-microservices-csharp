using APIClients.Domain.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APIClients.Infraestructure.Books
{
    internal class ClientEntityTypeConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            // cf. https://www.entityframeworktutorial.net/efcore/fluent-api-in-entity-framework-core.aspx
            
            builder.ToTable("Clients", SchemaNames.APIClients);
            builder.HasKey(client => client.Id);
            builder.Property(client => client.Name)
                .HasColumnName("Name")
                .IsRequired();
            builder.OwnsOne(client => client.Address, Address =>
            {
                Address.Property(address => address.Street)
                    .HasColumnName("Street")
                    .IsRequired();
                Address.Property(address => address.Local)
                    .HasColumnName("Local")
                    .IsRequired();
                Address.Property(address => address.PostalCode)
                    .HasColumnName("PostalCode")
                    .IsRequired();
                Address.OwnsOne(address => address.Country, Country =>
                {
                    Country.Property(property => property.Name)
                        .HasColumnName("Country")
                        .IsRequired();
                }).Navigation(address => address.Country).IsRequired();
            }).Navigation(client => client.Address).IsRequired();
        }
    }
}