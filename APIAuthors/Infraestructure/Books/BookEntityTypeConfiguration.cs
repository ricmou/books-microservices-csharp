using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APIAuthors.Infraestructure.Books
{
    internal class BookEntityTypeConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            // cf. https://www.entityframeworktutorial.net/efcore/fluent-api-in-entity-framework-core.aspx
            
            builder.ToTable("Books", SchemaNames.APIAuthors);
            builder.HasKey(book => book.Id);
            builder.HasMany<Author>(book => book.Authors).WithMany(author => author.Books);
            //builder.Property<bool>("_active").HasColumnName("Active");
        }
    }
}