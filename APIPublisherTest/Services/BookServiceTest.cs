using APIPublisher.Domain.Books;
using APIPublisher.Domain.Publishers;
using APIPublisher.Domain.Shared;
using APIPublisher.Services;
using Moq;
using Newtonsoft.Json;

namespace APIPublisherTest.Services;

public abstract class BookServiceTestSetup : IDisposable
{
    public BooksService Bks;

    public BookServiceTestSetup()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(unit => unit.CommitAsync().Result).Returns(0);
        
        var pub = new Publisher("AWE", "Addison Wesley", "US");
        var pub2 = new Publisher("ORE", "O'Reilly", "GB");

        var Book = new Book("978-0123456789", pub);
        var Book2 = new Book("978-1123456789", pub2);
        List<Book> list = new List<Book>();
        list.Add(Book);
            
        var repo = new Mock<IBooksRepository>();
        repo.Setup(rep => rep.GetAllAsync().Result).Returns(list);
        repo.Setup(rep => rep.GetByPublisherIdAsync(new PublisherId("AWE")).Result).Returns(list);
        repo.Setup(rep => rep.GetByPublisherIdAsync(new PublisherId("LUL")).Result).Returns<List<Book>>(null);
        repo.Setup(rep => rep.GetByIdAsync(new BookId("978-0123456789")).Result).Returns(Book);
        repo.Setup(rep => rep.GetByIdAsync(new BookId("978-1234567890")).Result).Returns<Book>(null);
        repo.Setup(rep => rep.AddAsync(Book).Result).Returns(Book);
        repo.Setup(rep => rep.AddAsync(It.IsAny<Book>()).Result).Returns<Book>(null);
        
        var pubRepo = new Mock<IPublishersRepository>();
        pubRepo.Setup(rep => rep.GetByIdAsync(new PublisherId("AWE")).Result).Returns(pub);
        pubRepo.Setup(rep => rep.GetByIdAsync(new PublisherId("ORE")).Result).Returns(pub2);
        pubRepo.Setup(rep => rep.GetByIdAsync(new PublisherId("LUL")).Result).Returns<Book>(null);

        Bks = new BooksService(unitOfWork.Object, repo.Object, pubRepo.Object);
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
            var bok = new BooksDto("978-0123456789", new PublisherDto("AWE", "Addison Wesley", "US"));
                
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
            var bok = new BooksDto("978-0123456789", new PublisherDto("AWE", "Addison Wesley", "US"));
            var bdto = Bks.GetAllAsync();
            Assert.Equal(JsonConvert.SerializeObject(bok),JsonConvert.SerializeObject(bdto.Result[0]));
        }
        
        [Fact]
        public void TestDeleteAsyncValid()
        {
            var bok = new BooksDto("978-0123456789", new PublisherDto("AWE", "Addison Wesley", "US"));
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
            var bok = new BooksDto("978-0123456789", new PublisherDto("ORE", "O'Reilly", "GB"));
            var bdto = Bks.UpdateAsync(new CreatingBooksDto("978-0123456789", "ORE"));
            Assert.Equal(JsonConvert.SerializeObject(bok),JsonConvert.SerializeObject(bdto.Result));
        }
        
        [Fact]
        public void TestUpdateAsyncInvalid()
        {
            var bdto = Bks.UpdateAsync(new CreatingBooksDto("978-1234567890", "AWE"));
            Assert.Null(bdto.Result);
        }
        
        [Fact]
        public async void TestUpdateAsyncInvalidPublisher()
        {
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => Bks.UpdateAsync(new CreatingBooksDto("978-0123456789", "LUL")));
        }
        
        [Fact]
        public void TestAddAsyncValid()
        {
            var bok = new BooksDto("978-0123456789", new PublisherDto("AWE", "Addison Wesley", "US"));
            var bdto = Bks.AddAsync(new CreatingBooksDto("978-0123456789", "AWE"));
            Assert.Equal(JsonConvert.SerializeObject(bok),JsonConvert.SerializeObject(bdto.Result));
        }
        
        [Fact]
        public async void TestAddAsyncInvalidPublisher()
        {
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => Bks.AddAsync(new CreatingBooksDto("978-0123456789", "LUL")));
        }
        
        [Fact]
        public void TestGetAllOfPublisherAsync()
        {
            var bok = new BooksDto("978-0123456789", new PublisherDto("AWE", "Addison Wesley", "US"));
            var bdto = Bks.GetAllFromPublisherAsync(new PublisherId("AWE"));
            Assert.Equal(JsonConvert.SerializeObject(bok),JsonConvert.SerializeObject(bdto.Result[0]));
        }
        
        [Fact]
        public async void TestGetAllOfPublisherInvalid()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => Bks.GetAllFromPublisherAsync(new PublisherId("LUL")));
        }
}