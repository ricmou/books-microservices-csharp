using System.Collections.Generic;
using System.Threading.Tasks;
using APICategories.Domain.Categories;

namespace APICategories.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();
    Task<CategoryDto> GetByIdAsync(CategoryId id);
    Task<CategoryDto> AddAsync(CreatingCategoryDto dto);
    Task<CategoryDto> UpdateAsync(CategoryDto dto);
    Task<CategoryDto> DeleteAsync(CategoryId id);
}