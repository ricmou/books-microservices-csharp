using APICategories.Domain.Books;
using APICategories.Domain.Categories;
using APICategories.Domain.Shared;
using APICategories.Services;
using Moq;
using Newtonsoft.Json;

namespace APICategoriesTest.Services;

public abstract class BookServiceTestSetup : IDisposable
{
    public BooksService Bks;

    public BookServiceTestSetup()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(unit => unit.CommitAsync().Result).Returns(0);
        
        var aut = new Category("RE1", "CatName");
        var aut2 = new Category("RE2", "CatName");

        var Book = new Book("978-0123456789");
        Book.AddCategory(aut);
        var Book2 = new Book("978-1123456789");
        Book2.AddCategory(aut2);
        List<Book> list = new List<Book>();
        list.Add(Book);
            
        var repo = new Mock<IBooksRepository>();
        repo.Setup(rep => rep.GetAllAsync().Result).Returns(list);
        repo.Setup(rep => rep.GetByIdAsync(new BookId("978-0123456789")).Result).Returns(Book);
        repo.Setup(rep => rep.GetByIdAsync(new BookId("978-1234567890")).Result).Returns<Book>(null);
        repo.Setup(rep => rep.AddAsync(Book).Result).Returns(Book);
        repo.Setup(rep => rep.AddAsync(It.IsAny<Book>()).Result).Returns<Book>(null);
        
        var autRepo = new Mock<ICategoryRepository>();
        autRepo.Setup(rep => rep.GetByIdAsync(new CategoryId("RE1")).Result).Returns(aut);
        autRepo.Setup(rep => rep.GetByIdAsync(new CategoryId("RE2")).Result).Returns(aut2);
        autRepo.Setup(rep => rep.GetByIdAsync(new CategoryId("RE3")).Result).Returns<Book>(null);

        Bks = new BooksService(unitOfWork.Object, repo.Object, autRepo.Object);
    }
        
    public void Dispose()
    {

    }
}

public class BookServiceTest : BookServiceTestSetup
{
        [Fact]
        public void TestGetByIdAsyncValid()
        {
            var lstAut = new List<CategoryDto>();
            lstAut.Add(new CategoryDto("RE1", "CatName"));
            var bok = new BooksDto("978-0123456789", lstAut);
                
            var bdto = Bks.GetByIdAsync(new BookId("978-0123456789"));
            Assert.Equal(JsonConvert.SerializeObject(bok),JsonConvert.SerializeObject(bdto.Result));
        }
        
        [Fact]
        public void TestGetByIdAsyncInvalid()
        {
            var bdto = Bks.GetByIdAsync(new BookId("978-1234567890"));
            Assert.Null(bdto.Result);
        }

        [Fact]
        public void TestGetAllAsync()
        {
            var lstAut = new List<CategoryDto>();
            lstAut.Add(new CategoryDto("RE1", "CatName"));
            var bok = new BooksDto("978-0123456789", lstAut);
            var bdto = Bks.GetAllAsync();
            Assert.Equal(JsonConvert.SerializeObject(bok),JsonConvert.SerializeObject(bdto.Result[0]));
        }
        
        [Fact]
        public void TestDeleteAsyncValid()
        {
            var lstAut = new List<CategoryDto>();
            lstAut.Add(new CategoryDto("RE1", "CatName"));
            var bok = new BooksDto("978-0123456789", lstAut);
            var bdto = Bks.DeleteAsync(new BookId("978-0123456789"));
            Assert.Equal(JsonConvert.SerializeObject(bok),JsonConvert.SerializeObject(bdto.Result));
        }
        
        [Fact]
        public void TestDeleteAsyncInvalid()
        {
            var bdto = Bks.DeleteAsync(new BookId("978-1234567890"));
            Assert.Null(bdto.Result);
        }
        
        [Fact]
        public void TestUpdateAsyncValid()
        {
            var lstAut = new List<CategoryDto>();
            lstAut.Add(new CategoryDto("RE2", "CatName"));;
            var bok = new BooksDto("978-0123456789", lstAut);
            var lstCreAut = new List<string>();
            lstCreAut.Add("RE2");
            var bdto = Bks.UpdateAsync(new CreatingBooksDto("978-0123456789", lstCreAut));
            Assert.Equal(JsonConvert.SerializeObject(bok),JsonConvert.SerializeObject(bdto.Result));
        }
        
        [Fact]
        public void TestUpdateAsyncInvalid()
        {
            var lstCreAut = new List<string>();
            lstCreAut.Add("RE1");
            var bdto = Bks.UpdateAsync(new CreatingBooksDto("978-1234567890", lstCreAut));
            Assert.Null(bdto.Result);
        }
        
        [Fact]
        public async void TestUpdateAsyncInvalidCategory()
        {
            var lstCreAut = new List<string>();
            lstCreAut.Add("RE3");
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => Bks.UpdateAsync(new CreatingBooksDto("978-0123456789", lstCreAut)));
        }
        
        [Fact]
        public void TestAddAsyncValid()
        {
            var lstAut = new List<CategoryDto>();
            lstAut.Add(new CategoryDto("RE1", "CatName"));
            var bok = new BooksDto("978-0123456789", lstAut);
            var lstCreAut = new List<string>();
            lstCreAut.Add("RE1");
            var bdto = Bks.AddAsync(new CreatingBooksDto("978-0123456789", lstCreAut));
            Assert.Equal(JsonConvert.SerializeObject(bok),JsonConvert.SerializeObject(bdto.Result));
        }
        
        [Fact]
        public async void TestAddAsyncInvalidCategory()
        {
            var lstCreAut = new List<string>();
            lstCreAut.Add("RE3");
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => Bks.AddAsync(new CreatingBooksDto("978-0123456789", lstCreAut)));
        }
}