namespace APICategories.Domain.Categories;

public class CreatingCategoryDto
{
    public string CategoryId { get; set; }
    
    public string Name { get; set; }

    public CreatingCategoryDto(string categoryId, string name)
    {
        CategoryId = categoryId;
        Name = name;
    }
}