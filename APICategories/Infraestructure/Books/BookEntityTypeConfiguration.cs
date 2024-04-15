using APICategories.Domain.Books;
using APICategories.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APICategories.Infraestructure.Books
{
    internal class BookEntityTypeConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            // cf. https://www.entityframeworktutorial.net/efcore/fluent-api-in-entity-framework-core.aspx
            
            builder.ToTable("Books", SchemaNames.APICategories);
            builder.HasKey(book => book.Id);
            builder.HasMany<Category>(book => book.Categories).WithMany(category => category.Books);
            //builder.Property<bool>("_active").HasColumnName("Active");
        }
    }
}