using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Books;
using APIAuthors.Domain.Shared;
using APIAuthors.Services;
using Moq;
using Newtonsoft.Json;

namespace APIAuthorsTest.Services;

public abstract class BookServiceTestSetup : IDisposable
{
    public BooksService Whs;

    public BookServiceTestSetup()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(unit => unit.CommitAsync().Result).Returns(0);
        
        var aut = new Author("RE1", "FirstName", "LastName", new DateOnly(1999,1,1), "DE");
        var aut2 = new Author("RE2", "FirstName", "LastName", new DateOnly(1999, 1, 1), "DE");

        var Book = new Book("978-0123456789");
        Book.AddAuthor(aut);
        var Book2 = new Book("978-1123456789");
        Book2.AddAuthor(aut2);
        List<Book> list = new List<Book>();
        list.Add(Book);
            
        var repo = new Mock<IBooksRepository>();
        repo.Setup(rep => rep.GetAllAsync().Result).Returns(list);
        repo.Setup(rep => rep.GetByAuthorIdAsync(new AuthorId("RE1")).Result).Returns(list);
        repo.Setup(rep => rep.GetByAuthorIdAsync(new AuthorId("RE3")).Result).Returns<List<Book>>(null);
        repo.Setup(rep => rep.GetByIdAsync(new BookId("978-0123456789")).Result).Returns(Book);
        repo.Setup(rep => rep.GetByIdAsync(new BookId("978-1234567890")).Result).Returns<Book>(null);
        repo.Setup(rep => rep.AddAsync(Book).Result).Returns(Book);
        repo.Setup(rep => rep.AddAsync(It.IsAny<Book>()).Result).Returns<Book>(null);
        
        var autRepo = new Mock<IAuthorsRepository>();
        autRepo.Setup(rep => rep.GetByIdAsync(new AuthorId("RE1")).Result).Returns(aut);
        autRepo.Setup(rep => rep.GetByIdAsync(new AuthorId("RE2")).Result).Returns(aut2);
        autRepo.Setup(rep => rep.GetByIdAsync(new AuthorId("RE3")).Result).Returns<Book>(null);

        Whs = new BooksService(unitOfWork.Object, repo.Object, autRepo.Object);
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
            var lstAut = new List<AuthorDto>();
            lstAut.Add(new AuthorDto("RE1", "FirstName", "LastName", "01/01/1999", "DE"));
            var bok = new BooksDto("978-0123456789", lstAut);
                
            var bdto = Whs.GetByIdAsync(new BookId("978-0123456789"));
            Assert.Equal(JsonConvert.SerializeObject(bok),JsonConvert.SerializeObject(bdto.Result));
        }
        
        [Fact]
        public void TestGetByIdAsyncInvalid()
        {
            var bdto = Whs.GetByIdAsync(new BookId("978-1234567890"));
            Assert.Null(bdto.Result);
        }

        [Fact]
        public void TestGetAllAsync()
        {
            var lstAut = new List<AuthorDto>();
            lstAut.Add(new AuthorDto("RE1", "FirstName", "LastName", "01/01/1999", "DE"));
            var bok = new BooksDto("978-0123456789", lstAut);
            var bdto = Whs.GetAllAsync();
            Assert.Equal(JsonConvert.SerializeObject(bok),JsonConvert.SerializeObject(bdto.Result[0]));
        }
        
        [Fact]
        public void TestDeleteAsyncValid()
        {
            var lstAut = new List<AuthorDto>();
            lstAut.Add(new AuthorDto("RE1", "FirstName", "LastName", "01/01/1999", "DE"));
            var bok = new BooksDto("978-0123456789", lstAut);
            var bdto = Whs.DeleteAsync(new BookId("978-0123456789"));
            Assert.Equal(JsonConvert.SerializeObject(bok),JsonConvert.SerializeObject(bdto.Result));
        }
        
        [Fact]
        public void TestDeleteAsyncInvalid()
        {
            var bdto = Whs.DeleteAsync(new BookId("978-1234567890"));
            Assert.Null(bdto.Result);
        }
        
        [Fact]
        public void TestUpdateAsyncValid()
        {
            var lstAut = new List<AuthorDto>();
            lstAut.Add(new AuthorDto("RE2", "FirstName", "LastName", "01/01/1999", "DE"));
            var bok = new BooksDto("978-0123456789", lstAut);
            var lstCreAut = new List<string>();
            lstCreAut.Add("RE2");
            var bdto = Whs.UpdateAsync(new CreatingBooksDto("978-0123456789", lstCreAut));
            Assert.Equal(JsonConvert.SerializeObject(bok),JsonConvert.SerializeObject(bdto.Result));
        }
        
        [Fact]
        public void TestUpdateAsyncInvalid()
        {
            var lstCreAut = new List<string>();
            lstCreAut.Add("RE1");
            var bdto = Whs.UpdateAsync(new CreatingBooksDto("978-1234567890", lstCreAut));
            Assert.Null(bdto.Result);
        }
        
        [Fact]
        public async void TestUpdateAsyncInvalidAuthor()
        {
            var lstCreAut = new List<string>();
            lstCreAut.Add("RE3");
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => Whs.UpdateAsync(new CreatingBooksDto("978-0123456789", lstCreAut)));
        }
        
        [Fact]
        public void TestAddAsyncValid()
        {
            var lstAut = new List<AuthorDto>();
            lstAut.Add(new AuthorDto("RE1", "FirstName", "LastName", "01/01/1999", "DE"));
            var bok = new BooksDto("978-0123456789", lstAut);
            var lstCreAut = new List<string>();
            lstCreAut.Add("RE1");
            var bdto = Whs.AddAsync(new CreatingBooksDto("978-0123456789", lstCreAut));
            Assert.Equal(JsonConvert.SerializeObject(bok),JsonConvert.SerializeObject(bdto.Result));
        }
        
        [Fact]
        public async void TestAddAsyncInvalidAuthor()
        {
            var lstCreAut = new List<string>();
            lstCreAut.Add("RE3");
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => Whs.AddAsync(new CreatingBooksDto("978-0123456789", lstCreAut)));
        }
        
        [Fact]
        public void TestGetAllOfAuthorAsync()
        {
            var lstAut = new List<AuthorDto>();
            lstAut.Add(new AuthorDto("RE1", "FirstName", "LastName", "01/01/1999", "DE"));
            var bok = new BooksDto("978-0123456789", lstAut);
            var bdto = Whs.GetByAuthorIdAsync(new AuthorId("RE1"));
            Assert.Equal(JsonConvert.SerializeObject(bok),JsonConvert.SerializeObject(bdto.Result[0]));
        }
        
        [Fact]
        public async void TestGetAllOfAuthorInvalid()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => Whs.GetByAuthorIdAsync(new AuthorId("RE3")));
        }
}