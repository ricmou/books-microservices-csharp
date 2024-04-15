using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Shared;
using APIAuthors.Services;
using Moq;
using Newtonsoft.Json;

namespace APIAuthorsTest.Services;

public abstract class AuthorServiceTestSetup : IDisposable
{
    public AuthorsService Whs;

    public AuthorServiceTestSetup()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(unit => unit.CommitAsync().Result).Returns(0);

        var author = new Author("RE1", "FirstName", "LastName", new DateOnly(1999,1,1), "DE");
        var author2 =
            new Author("RE2", "FirstName", "LastName", new DateOnly(1999, 1, 1), "DE");
        List<Author> list = new List<Author>();
        list.Add(author);
            
        var repo = new Mock<IAuthorsRepository>();
        repo.Setup(rep => rep.GetAllAsync().Result).Returns(list);
        repo.Setup(rep => rep.GetByIdAsync(new AuthorId("RE1")).Result).Returns(author);
        repo.Setup(rep => rep.GetByIdAsync(new AuthorId("RE2")).Result).Returns<Author>(null);
        repo.Setup(rep => rep.AddAsync(author).Result).Returns(author);
        repo.Setup(rep => rep.AddAsync(It.IsAny<Author>()).Result).Returns<Author>(null);

        Whs = new AuthorsService(unitOfWork.Object, repo.Object);
    }
        
    public void Dispose()
    {

    }
}

public class AuthorServiceTest : AuthorServiceTestSetup
{
    [Fact]
        public void TestGetByIdAsyncValid()
        {
            var aut = new AuthorDto("RE1", "FirstName", "LastName", "01/01/1999", "DE");
            var adto = Whs.GetByIdAsync(new AuthorId("RE1"));
            Assert.Equal(JsonConvert.SerializeObject(aut),JsonConvert.SerializeObject(adto.Result));
        }
        
        [Fact]
        public void TestGetByIdAsyncInvalid()
        {
            var adto = Whs.GetByIdAsync(new AuthorId("RE2"));
            Assert.Null(adto.Result);
        }

        [Fact]
        public void TestGetAllAsync()
        {
            var aut = new AuthorDto("RE1", "FirstName", "LastName", "01/01/1999", "DE");
            var adto = Whs.GetAllAsync();
            Assert.Equal(JsonConvert.SerializeObject(aut),JsonConvert.SerializeObject(adto.Result[0]));
        }
        
        [Fact]
        public void TestDeleteAsyncValid()
        {
            var aut = new AuthorDto("RE1", "FirstName", "LastName", "01/01/1999", "DE");
            var adto = Whs.DeleteAsync(new AuthorId("RE1"));
            Assert.Equal(JsonConvert.SerializeObject(aut),JsonConvert.SerializeObject(adto.Result));
        }
        
        [Fact]
        public void TestDeleteAsyncInvalid()
        {
            var adto = Whs.DeleteAsync(new AuthorId("RE2"));
            Assert.Null(adto.Result);
        }
        
        [Fact]
        public void TestUpdateAsyncValid()
        {
            var aut = new AuthorDto("RE1", "FirstName1", "LastName1", "02/03/1999", "ZA");
            var adto = Whs.UpdateAsync(new AuthorDto("RE1", "FirstName1", "LastName1", "02/03/1999", "ZA"));
            Assert.Equal(JsonConvert.SerializeObject(aut),JsonConvert.SerializeObject(adto.Result));
        }
        
        [Fact]
        public void TestUpdateAsyncInvalid()
        {
            var adto = Whs.UpdateAsync(new AuthorDto("RE2", "FirstName", "LastName", "01/01/1999", "DE"));
            Assert.Null(adto.Result);
        }
        
        [Fact]
        public void TestAddAsyncValid()
        {
            var createaut = Whs.AddAsync(new CreatingAuthorsDto("RE1", "FirstName", "LastName", new DateOnly(1999, 1, 1).ToString(), "DE"));
            var resaut = new AuthorDto("RE1", "FirstName", "LastName", "01/01/1999", "DE");
            Assert.Equal(JsonConvert.SerializeObject(resaut),JsonConvert.SerializeObject(createaut.Result));
        }
}