using APICategories.Controllers;
using APICategories.Domain.Categories;
using APICategories.Domain.Shared;
using APICategories.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace APICategoriesTest.Controllers;

public abstract class CategoryControllerTestSetup : IDisposable
{
    protected CategoriesController Auc;

    protected CategoryDto CategoryDto;
    protected CategoryDto CategoryDto2;
    protected CategoryDto CategoryDto3;
    protected CreatingCategoryDto CreatingCategoryDto;

    public CategoryControllerTestSetup()
    {
        CategoryDto = new CategoryDto("RE1", "CatName");
        List<CategoryDto> listDto = new List<CategoryDto>();
        listDto.Add(CategoryDto);

        CategoryDto2 = new CategoryDto("RE2", "CatName2");

        CategoryDto3 = new CategoryDto("RE3", "CatName3");
        
        CreatingCategoryDto = new CreatingCategoryDto("RE1", "CatName");
        
        var CategoryService = new Mock<ICategoryService>();
        CategoryService.Setup(cat => cat.GetAllAsync().Result).Returns(listDto);
        CategoryService.Setup(cat => cat.GetByIdAsync(new CategoryId("RE1")).Result).Returns(CategoryDto);
        CategoryService.Setup(cat => cat.GetByIdAsync(new CategoryId("RE9")).Result).Returns<CategoryDto>(null);
        CategoryService.Setup(cat => cat.AddAsync(CreatingCategoryDto).Result).Returns(CategoryDto);
        CategoryService.Setup(cat => cat.UpdateAsync(CategoryDto).Result).Returns<CategoryDto>(null);
        CategoryService.Setup(cat => cat.UpdateAsync(CategoryDto2)).ReturnsAsync(CategoryDto2);
        CategoryService.Setup(cat => cat.UpdateAsync(CategoryDto3).Result)
            .Throws(new BusinessRuleValidationException("Whatever"));
        CategoryService.Setup(cat => cat.DeleteAsync(new CategoryId("RE1")).Result).Returns<CategoryDto>(null);
        CategoryService.Setup(cat => cat.DeleteAsync(new CategoryId("RE2")).Result).Returns(CategoryDto2);
        CategoryService.Setup(cat => cat.DeleteAsync(new CategoryId("RE3")).Result)
            .Throws(new BusinessRuleValidationException("Whatever"));
        
        Auc = new CategoriesController(CategoryService.Object);
    }

    public void Dispose()
    {
    }
}

public class CategoriesControllerTest : CategoryControllerTestSetup
{
    [Fact]
    public async void TestGetAllAsync()
    {
        var cat = new CategoryDto("RE1", "CatName");
        List<CategoryDto> listCat = new List<CategoryDto>();
        listCat.Add(cat);
        var adto = await Auc.GetAll();
        var result = adto.Value;
        Assert.Equal(JsonConvert.SerializeObject(listCat), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestGetGetByIdValid()
    {
        var adto = await Auc.GetGetById("RE1");
        var result = adto.Value;

        Assert.Equal(JsonConvert.SerializeObject(CategoryDto), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestGetGetByIdInValid()
    {
        var adto = await Auc.GetGetById("RE9");

        var actionResult = Assert.IsType<ActionResult<CategoryDto>>(adto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }
    
    [Fact]
    public async void TestCreate()
    {
        var adto = await Auc.Create(CreatingCategoryDto);
        var result = adto.Value;

        Assert.Equal(JsonConvert.SerializeObject(CategoryDto), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestUpdateMismatchId()
    {
        var adto = await Auc.Update("NO1", CategoryDto);

        var actionResult = Assert.IsType<ActionResult<CategoryDto>>(adto);
        Assert.IsType<BadRequestResult>(actionResult.Result);
    }

    [Fact]
    public async void TestUpdateNullDto()
    {
        var adto = await Auc.Update("RE1", CategoryDto);

        var actionResult = Assert.IsType<ActionResult<CategoryDto>>(adto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestUpdateSuccess()
    {
        var adto = await Auc.Update("RE2", CategoryDto2);

        var actionResult = Assert.IsType<ActionResult<CategoryDto>>(adto);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = adto.Result as OkObjectResult;
        var actualValue = returnValue?.Value;


        Assert.Equal(JsonConvert.SerializeObject(CategoryDto2), JsonConvert.SerializeObject(actualValue));
    }

    [Fact]
    public async void TestUpdateBadReq()
    {
        var adto = await Auc.Update("RE3", CategoryDto3);

        var actionResult = Assert.IsType<ActionResult<CategoryDto>>(adto);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }

    [Fact]
    public async void TestDeleteNullDto()
    {
        var adto = await Auc.HardDelete("RE1");

        var actionResult = Assert.IsType<ActionResult<CategoryDto>>(adto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestDeleteSuccess()
    {
        var adto = await Auc.HardDelete("RE2");

        var actionResult = Assert.IsType<ActionResult<CategoryDto>>(adto);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = adto.Result as OkObjectResult;
        var actualValue = returnValue?.Value;


        Assert.Equal(JsonConvert.SerializeObject(CategoryDto2), JsonConvert.SerializeObject(actualValue));
    }

    [Fact]
    public async void TestDeleteBadReq()
    {
        var adto = await Auc.HardDelete("RE3");

        var actionResult = Assert.IsType<ActionResult<CategoryDto>>(adto);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }
}