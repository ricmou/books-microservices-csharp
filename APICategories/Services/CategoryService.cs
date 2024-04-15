using System.Collections.Generic;
using System.Threading.Tasks;
using APICategories.Domain.Categories;
using APICategories.Domain.Shared;

namespace APICategories.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryRepository _repo;

    public CategoryService(IUnitOfWork unitOfWork, ICategoryRepository repo)
    {
        _unitOfWork = unitOfWork;
        _repo = repo;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var list = await this._repo.GetAllAsync();

        List<CategoryDto> listDto = list.ConvertAll<CategoryDto>(category => new CategoryDto(category.Id.AsString(), category.Name));

        return listDto;
    }

    public async Task<CategoryDto> GetByIdAsync(CategoryId id)
    {
        var category = await this._repo.GetByIdAsync(id);
            
        if(category == null)
            return null;

        return new CategoryDto(category.Id.AsString(), category.Name);
    }

    public async Task<CategoryDto> AddAsync(CreatingCategoryDto dto)
    {
        var category = new Category(dto.CategoryId, dto.Name);
        
        await this._repo.AddAsync(category);

        await this._unitOfWork.CommitAsync();
        
        return new CategoryDto(category.Id.AsString(), category.Name);
    }

    public async Task<CategoryDto> UpdateAsync(CategoryDto dto)
    {
        var category = await this._repo.GetByIdAsync(new CategoryId(dto.CategoryId)); 

        if (category == null)
            return null;   

        // change all field
        category.ChangeName(dto.Name);

        await this._unitOfWork.CommitAsync();

        return new CategoryDto(category.Id.AsString(), category.Name);
    }

    public async Task<CategoryDto> DeleteAsync(CategoryId id)
    {
        var category = await this._repo.GetByIdAsync(id);

        if (category == null)
            return null;

        this._repo.Remove(category);
        await this._unitOfWork.CommitAsync();

        return new CategoryDto(category.Id.AsString(), category.Name);
    }
}