using APICategories.Domain.Categories;
using APICategories.Infraestructure.Shared;

namespace APICategories.Infraestructure.Categories;

public class CategoryRepository : BaseRepository<Category, CategoryId>, ICategoryRepository
{
    private APICategoriesDbContext _context;

    public CategoryRepository(APICategoriesDbContext context) : base(context.Categories)
    {
        _context = context;
    }
}