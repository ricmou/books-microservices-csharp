using APICategories;
using APICategories.Controllers;
using APICategories.Domain.Categories;
using APICategories.Services;
using APICategoriesTest.Helpers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace APICategoriesTest.Controllers;

public abstract class CategoryGrpcControllerTestSetup : IDisposable
{
    protected CategoriesGrpcController Catc;

    protected CategoryDto CategoryDto;
    protected CategoryDto CategoryDto2;
    protected CreatingCategoryDto CreatingCategoryDto;
    protected CreatingCategoryDto CreatingCategoryDto2;

    public CategoryGrpcControllerTestSetup()
    {
        CategoryDto = new CategoryDto("RE1", "CatName");
        List<CategoryDto> listDto = new List<CategoryDto>();
        listDto.Add(CategoryDto);

        CategoryDto2 = new CategoryDto("RE2", "CatName2");

        CreatingCategoryDto = new CreatingCategoryDto("RE1", "CatName");
        
        CreatingCategoryDto2 = new CreatingCategoryDto("RE2", "CatName2");

        var CategoryService = new Mock<ICategoryService>();
        CategoryService.Setup(cat => cat.GetAllAsync().Result).Returns(listDto);
        CategoryService.Setup(cat => cat.GetByIdAsync(new CategoryId("RE1")).Result).Returns(CategoryDto);
        CategoryService.Setup(cat => cat.GetByIdAsync(new CategoryId("RE9")).Result).Returns<CategoryDto>(null);
        CategoryService.Setup(cat => cat.AddAsync(It.Is<CreatingCategoryDto>(i => 
            i.CategoryId == CreatingCategoryDto.CategoryId)).Result).Returns(CategoryDto);
        CategoryService.Setup(cat => cat.AddAsync(It.Is<CreatingCategoryDto>(i => 
            i.CategoryId == CreatingCategoryDto2.CategoryId)).Result).Returns<CategoryDto>(null);
        CategoryService.Setup(cat => cat.UpdateAsync(It.Is<CategoryDto>(i => 
            i.CategoryId == CategoryDto.CategoryId)).Result).Returns<CategoryDto>(null);
        CategoryService.Setup(cat => cat.UpdateAsync(It.Is<CategoryDto>(i => 
            i.CategoryId == CategoryDto2.CategoryId)).Result).Returns(CategoryDto2);
        CategoryService.Setup(cat => cat.DeleteAsync(new CategoryId("RE1")).Result).Returns<CategoryDto>(null);
        CategoryService.Setup(cat => cat.DeleteAsync(new CategoryId("RE2")).Result).Returns(CategoryDto2);

        var logger = new Mock<ILogger<CategoriesGrpcController>>();
        
        Catc = new CategoriesGrpcController(logger.Object, CategoryService.Object);
    }

    public void Dispose()
    {
    }
}

public class CategoriesGrpcControllerTest : CategoryGrpcControllerTestSetup
{
    [Fact]
    public async void TestGetAllAsync()
    {
        var cat = new CategoryGrpcDto{
           CategoryId = "RE1", 
           Name = "CatName"
        };
        List<CategoryGrpcDto> listCat = new List<CategoryGrpcDto>();
        listCat.Add(cat);
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        var responseStream = new TestServerStreamWriter<CategoryGrpcDto>(callContext);

        using var call = Catc.GetAllCategories(new Empty(), responseStream, callContext);

        await call;
        responseStream.Complete();
        
        var allMessages = new List<CategoryGrpcDto>();
        await foreach (var message in responseStream.ReadAllAsync())
        {
            allMessages.Add(message);
        }
        
        Assert.Equal(JsonConvert.SerializeObject(listCat), JsonConvert.SerializeObject(allMessages));
    }

    [Fact]
    public async void TestGetGetByIdValid()
    {
        var cat = new CategoryGrpcDto{
            CategoryId = "RE1", 
            Name = "CatName"
        };

        //GRPC setup
        var callContext = TestServerCallContext.Create();

        var response = await Catc.GetCategoryByID(new RequestWithCategoryId{Id = "RE1"}, callContext);
        

        Assert.Equal(JsonConvert.SerializeObject(response), JsonConvert.SerializeObject(cat));
    }

    [Fact]
    public async Task TestGetGetByIdInValid()
    {
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>( () => Catc.GetCategoryByID(new RequestWithCategoryId{Id = "RE9"}, callContext));
        
    }
    
    [Fact]
    public async void TestCreate()
    {
        var cat = new CategoryGrpcDto{
            CategoryId = "RE1", 
            Name = "CatName"
        };

        //GRPC setup
        var callContext = TestServerCallContext.Create();
        
        var response = await Catc.AddNewCategory(new CreatingCategoryGrpcDto
        {
            CategoryId = CreatingCategoryDto.CategoryId,
            Name = CreatingCategoryDto.Name
        }, callContext);

        Assert.Equal(JsonConvert.SerializeObject(cat), JsonConvert.SerializeObject(response));
    }
    
    [Fact]
    public async Task TestCreateInvalid()
    {
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>( () => Catc.AddNewCategory(new CreatingCategoryGrpcDto
        {
            CategoryId = CreatingCategoryDto2.CategoryId,
            Name = CreatingCategoryDto2.Name
        }, callContext));
        
    }


    [Fact]
    public async void TestUpdateNullDto()
    {
        var cat = new CategoryGrpcDto{
            CategoryId = "RE1", 
            Name = "CatName"
        };
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        
        await Assert.ThrowsAsync<RpcException>( () => Catc.ModifyCategory(cat, callContext));
    }

    [Fact]
    public async void TestUpdateSuccess()
    {
        var cat = new CategoryGrpcDto{
            CategoryId = CategoryDto2.CategoryId, 
            Name = CategoryDto2.Name

        };
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        
        var adto = await Catc.ModifyCategory(cat, callContext);
        
        Assert.Equal(JsonConvert.SerializeObject(adto), JsonConvert.SerializeObject(cat));
    }

    [Fact]
    public async void TestDeleteNullDto()
    {
        //GRPC setup
        var callContext = TestServerCallContext.Create();

        await Assert.ThrowsAsync<RpcException>( () => Catc.DeleteCategory(new RequestWithCategoryId{Id ="RE1"}, callContext));
    }

    [Fact]
    public async void TestDeleteSuccess()
    {
        var cat = new CategoryGrpcDto{
            CategoryId = CategoryDto2.CategoryId, 
            Name = CategoryDto2.Name

        };
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();

        var adto = await Catc.DeleteCategory(new RequestWithCategoryId { Id = "RE2" }, callContext);
        
        Assert.Equal(JsonConvert.SerializeObject(cat), JsonConvert.SerializeObject(adto));
    }
    
}