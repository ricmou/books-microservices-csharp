using APIPublisher.Domain.Books;
using APIPublisher.Domain.Publishers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APIPublisher.Infraestructure.Books
{
    internal class BookEntityTypeConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            // cf. https://www.entityframeworktutorial.net/efcore/fluent-api-in-entity-framework-core.aspx
            
            builder.ToTable("Books", SchemaNames.APIPublisher);
            builder.HasKey(book => book.Id);
            builder.HasOne<Publisher>(book => book.Publisher)
                .WithMany(publisher => publisher.Books);
            //builder.Property<bool>("_active").HasColumnName("Active");
        }
    }
}