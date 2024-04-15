namespace APICategories.Domain.Categories;

public class CategoryDto
{
    public string CategoryId { get; set; }
    
    public string Name { get; set; }

    public CategoryDto(string categoryId, string name)
    {
        CategoryId = categoryId;
        Name = name;
    }
}