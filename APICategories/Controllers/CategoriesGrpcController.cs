using System.Threading.Tasks;
using APICategories.Domain.Categories;
using APICategories.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace APICategories.Controllers;

public class CategoriesGrpcController : APICategoriesCategoryGRPC.APICategoriesCategoryGRPCBase
{
    private readonly ILogger<CategoriesGrpcController> _logger;
    private readonly ICategoryService _service;

    public CategoriesGrpcController(ILogger<CategoriesGrpcController> logger, ICategoryService service)
    {
        _logger = logger;
        _service = service;
    }
    
    public override async Task<CategoryGrpcDto> GetCategoryByID(RequestWithCategoryId request, ServerCallContext context)
    {
        var category = await this._service.GetByIdAsync(new CategoryId(request.Id));

        if (category == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.Id }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "No data Found"), metadata);
        }

        return new CategoryGrpcDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name
        };
    }

    public override async Task GetAllCategories(Empty request, IServerStreamWriter<CategoryGrpcDto> responseStream, ServerCallContext context)
    {
        var lstAllCategory = await _service.GetAllAsync();

        if (lstAllCategory == null || lstAllCategory.Count < 1)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "No data Found"));
        }
        
        foreach (var category in lstAllCategory)
        {
            await responseStream.WriteAsync(new CategoryGrpcDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name
            });
        }
    }
    
    

    public override async Task<CategoryGrpcDto> AddNewCategory(CreatingCategoryGrpcDto request, ServerCallContext context)
    {
        var category = await _service.AddAsync(new CreatingCategoryDto(request.CategoryId, request.Name));

        if (category == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.CategoryId }
            };
            throw new RpcException(new Status(StatusCode.Aborted, "Failed to add category"), metadata);
        }

        /*var categoryFetch = await _service.GetByIdAsync(new CategoryId(request.CategoryId));

        if (categoryFetch == null)
        {
            var metadata = new Metadata
            {
                { "ISBN", request.CategoryId }
            };
            throw new RpcException(new Status(StatusCode.Aborted, "Failed to add category"), metadata);
        }*/

        return new CategoryGrpcDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name
        };
    }

    public override async Task<CategoryGrpcDto> ModifyCategory(CategoryGrpcDto request, ServerCallContext context)
    {
        var category = await _service.UpdateAsync(new CategoryDto(request.CategoryId, request.Name));
        
        if (category == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.CategoryId }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "Could not find category"), metadata);
        }
        
        return new CategoryGrpcDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name
        };
    }

    public override async Task<CategoryGrpcDto> DeleteCategory(RequestWithCategoryId request, ServerCallContext context)
    {
        var category = await _service.DeleteAsync(new CategoryId(request.Id));
        
        if (category == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.Id }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "Could not find category"), metadata);
        }

        return new CategoryGrpcDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name
        };
    }
}