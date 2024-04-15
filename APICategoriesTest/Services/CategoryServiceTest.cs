using APICategories.Domain.Categories;
using APICategories.Domain.Shared;
using APICategories.Services;
using Moq;
using Newtonsoft.Json;

namespace APICategoriesTest.Services;

public abstract class CategoryServiceTestSetup : IDisposable
{
    public CategoryService Whs;

    public CategoryServiceTestSetup()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(unit => unit.CommitAsync().Result).Returns(0);

        var Category = new Category("RE1", "CatName");
        var Category2 = new Category("RE2", "CatName");
        List<Category> list = new List<Category>();
        list.Add(Category);
            
        var repo = new Mock<ICategoryRepository>();
        repo.Setup(rep => rep.GetAllAsync().Result).Returns(list);
        repo.Setup(rep => rep.GetByIdAsync(new CategoryId("RE1")).Result).Returns(Category);
        repo.Setup(rep => rep.GetByIdAsync(new CategoryId("RE2")).Result).Returns<Category>(null);
        repo.Setup(rep => rep.AddAsync(Category).Result).Returns(Category);
        repo.Setup(rep => rep.AddAsync(It.IsAny<Category>()).Result).Returns<Category>(null);

        Whs = new CategoryService(unitOfWork.Object, repo.Object);
    }
        
    public void Dispose()
    {

    }
}

public class CategoryServiceTest : CategoryServiceTestSetup
{
    [Fact]
        public void TestGetByIdAsyncValid()
        {
            var cat = new CategoryDto("RE1", "CatName");
            var cdto = Whs.GetByIdAsync(new CategoryId("RE1"));
            Assert.Equal(JsonConvert.SerializeObject(cat),JsonConvert.SerializeObject(cdto.Result));
        }
        
        [Fact]
        public void TestGetByIdAsyncInvalid()
        {
            var cdto = Whs.GetByIdAsync(new CategoryId("RE2"));
            Assert.Null(cdto.Result);
        }

        [Fact]
        public void TestGetAllAsync()
        {
            var cat = new CategoryDto("RE1", "CatName");
            var cdto = Whs.GetAllAsync();
            Assert.Equal(JsonConvert.SerializeObject(cat),JsonConvert.SerializeObject(cdto.Result[0]));
        }
        
        [Fact]
        public void TestDeleteAsyncValid()
        {
            var cat = new CategoryDto("RE1", "CatName");
            var cdto = Whs.DeleteAsync(new CategoryId("RE1"));
            Assert.Equal(JsonConvert.SerializeObject(cat),JsonConvert.SerializeObject(cdto.Result));
        }
        
        [Fact]
        public void TestDeleteAsyncInvalid()
        {
            var cdto = Whs.DeleteAsync(new CategoryId("RE2"));
            Assert.Null(cdto.Result);
        }
        
        [Fact]
        public void TestUpdateAsyncValid()
        {
            var cat = new CategoryDto("RE1", "CatName2");
            var cdto = Whs.UpdateAsync(new CategoryDto("RE1", "CatName2"));
            Assert.Equal(JsonConvert.SerializeObject(cat),JsonConvert.SerializeObject(cdto.Result));
        }
        
        [Fact]
        public void TestUpdateAsyncInvalid()
        {
            var cdto = Whs.UpdateAsync(new CategoryDto("RE2", "CatName"));
            Assert.Null(cdto.Result);
        }
        
        [Fact]
        public void TestAddAsyncValid()
        {
            var createcat = Whs.AddAsync(new CreatingCategoryDto("RE1", "CatName"));
            var rescat = new CategoryDto("RE1", "CatName");
            Assert.Equal(JsonConvert.SerializeObject(rescat),JsonConvert.SerializeObject(createcat.Result));
        }
}