using System.Collections.Generic;
using APICategories.Domain.Books;
using APICategories.Domain.Shared;

namespace APICategories.Domain.Categories;

public class Category : Entity<CategoryId>, IAggregateRoot
{
    public string Name { get; private set; }
    
    public List<Book> Books { get; private set; }

    private Category()
    {
    }

    public Category(string categoryId, string categoryName)
    {
        this.Id = new CategoryId(categoryId);
        if (IsValidName(categoryName))
        {
            this.Name = categoryName;
        }
        else
        {
            throw new BusinessRuleValidationException(
                "Category Names must have at least a character and no more than 50");
        }
        Books = new List<Book>();
    }
    
    private bool IsValidName(string name)
    {
        return (!string.IsNullOrEmpty(name) && name.Length <= 50);
    }

    public void ChangeName(string name)
    {
        if (IsValidName(name))
        {
            this.Name = name;
        }
        else
        {
            throw new BusinessRuleValidationException(
                "Category Names must have at least a character and no more than 50");
        }
    }
}