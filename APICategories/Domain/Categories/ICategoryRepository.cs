using APICategories.Domain.Shared;

namespace APICategories.Domain.Categories;

public interface ICategoryRepository : IRepository<Category, CategoryId>
{
    
}