using APICategories.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace APICategories.Infraestructure.Categories;

internal class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // cf. https://www.entityframeworktutorial.net/efcore/fluent-api-in-entity-framework-core.aspx
        builder.ToTable("Categories", SchemaNames.APICategories);    
        builder.HasKey(cat => cat.Id);
        builder.Property(property => property.Name)
            .HasColumnName("Name")
            .IsRequired();
        /*builder.OwnsOne(cat => cat.CategoryName, CategoryName =>
        {
            CategoryName.Property(property => property.Name)
                .HasColumnName("CatName")
                .IsRequired();
        }).Navigation(cat => cat.CategoryName).IsRequired();*/
    }
}