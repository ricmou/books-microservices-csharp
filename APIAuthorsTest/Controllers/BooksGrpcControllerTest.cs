using APIAuthors;
using APIAuthors.Controllers;
using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Books;
using APIAuthors.Services;
using APIAuthorsTest.Helpers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace APIAuthorsTest.Controllers;

public abstract class BookGrpcControllerTestSetup : IDisposable
{
    protected BooksGrpcController Boc;

    protected BooksDto BooksDto;
    protected BooksDto BooksDto2;
    protected CreatingBooksDto CreatingBooksDto;
    protected CreatingBooksDto CreatingBooksDto2;
    protected CreatingBooksDto CreatingBooksDto3;

    public BookGrpcControllerTestSetup()
    {
        var lstAut = new List<AuthorDto>();
        lstAut.Add(new AuthorDto("RE1", "FirstName", "LastName", "01/01/1999", "DE"));
        BooksDto = new BooksDto("978-0000000001", lstAut);

        List<BooksDto> listDto = new List<BooksDto>();
        listDto.Add(BooksDto);

        BooksDto2 = new BooksDto("978-0000000002", lstAut);

        var lstStr = new List<String>();
        lstStr.Add("RE1");
        CreatingBooksDto = new CreatingBooksDto("978-0000000001", lstStr);
        CreatingBooksDto2 = new CreatingBooksDto("978-0000000002", lstStr);
        CreatingBooksDto3 = new CreatingBooksDto("978-0000000003", lstStr);

        var BookService = new Mock<IBooksService>();
        BookService.Setup(bk => bk.GetAllAsync().Result).Returns(listDto);
        BookService.Setup(bk => bk.GetByAuthorIdAsync(new AuthorId("RE1")).Result).Returns(listDto);
        BookService.Setup(bk => bk.GetByAuthorIdAsync(new AuthorId("RE2")).Result).Returns<List<BooksDto>>(null);
        BookService.Setup(bk => bk.GetByIdAsync(new BookId("978-0000000001")).Result).Returns(BooksDto);
        BookService.Setup(bk => bk.GetByIdAsync(new BookId("978-0000000009")).Result).Returns<BooksDto>(null);
        BookService.Setup(bk => bk.AddAsync(It.Is<CreatingBooksDto>(i => 
            i.Id == CreatingBooksDto.Id)).Result).Returns(BooksDto);
        BookService.Setup(bk => bk.AddAsync(It.Is<CreatingBooksDto>(i => 
            i.Id == CreatingBooksDto2.Id)).Result).Returns<BookGrpcDto>(null);
        BookService.Setup(bk => bk.UpdateAsync(It.Is<CreatingBooksDto>(i => 
            i.Id == CreatingBooksDto.Id)).Result).Returns<BooksDto>(null);
        BookService.Setup(bk => bk.UpdateAsync(It.Is<CreatingBooksDto>(i => 
            i.Id == CreatingBooksDto2.Id)).Result).Returns(BooksDto2);
        BookService.Setup(bk => bk.DeleteAsync(new BookId("978-0000000001")).Result).Returns<BooksDto>(null);
        BookService.Setup(bk => bk.DeleteAsync(new BookId("978-0000000002")).Result).Returns(BooksDto2);

        var logger = new Mock<ILogger<BooksGrpcController>>();
        
        Boc = new BooksGrpcController(logger.Object ,BookService.Object);
    }

    public void Dispose()
    {
    }
}

public class BooksGrpcControllerTest : BookGrpcControllerTestSetup
{
    [Fact]
    public async void TestGetAllAsync()
    {
        var lstAut = new List<AuthorGrpcDto>();
        lstAut.Add(new AuthorGrpcDto
            {
                AuthorId = "RE1",
                FirstName = "FirstName",
                LastName = "LastName",
                BirthDate = "01/01/1999",
                Country = "DE"
            }
        );
        var bok = new BookGrpcDto
        {
            Id = "978-0000000001",
            Authors = {lstAut}
        };
        List<BookGrpcDto> listBok = new List<BookGrpcDto>();
        listBok.Add(bok);
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        var responseStream = new TestServerStreamWriter<BookGrpcDto>(callContext);

        using var call = Boc.GetAllBooks(new Empty(), responseStream, callContext);

        await call;
        responseStream.Complete();
        
        var allMessages = new List<BookGrpcDto>();
        await foreach (var message in responseStream.ReadAllAsync())
        {
            allMessages.Add(message);
        }
        
        Assert.Equal(JsonConvert.SerializeObject(listBok), JsonConvert.SerializeObject(allMessages));
    }

    [Fact]
    public async void TestGetByAuthorSuccess()
    {
        var lstAut = new List<AuthorGrpcDto>();
        lstAut.Add(new AuthorGrpcDto
        {
            AuthorId = "RE1",
            FirstName = "FirstName",
            LastName = "LastName",
            BirthDate = "01/01/1999",
            Country = "DE"
        });
        var bok = new BookGrpcDto{
            Id = "978-0000000001", 
            Authors = {lstAut}};
        List<BookGrpcDto> listBok = new List<BookGrpcDto>();
        listBok.Add(bok);
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        var responseStream = new TestServerStreamWriter<BookGrpcDto>(callContext);

        using var call = Boc.GetAllBooksFromAuthor(new RequestWithAuthorId{Id ="RE1"}, responseStream, callContext);

        await call;
        responseStream.Complete();
        
        var allMessages = new List<BookGrpcDto>();
        await foreach (var message in responseStream.ReadAllAsync())
        {
            allMessages.Add(message);
        }
        Assert.Equal(JsonConvert.SerializeObject(listBok), JsonConvert.SerializeObject(allMessages));
    }

    [Fact]
    public async void TestGetByAuthorFail()
    {
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        var responseStream = new TestServerStreamWriter<BookGrpcDto>(callContext);

        await Assert.ThrowsAsync<RpcException>(() =>Boc.GetAllBooksFromAuthor(new RequestWithAuthorId{Id ="RE2"}, responseStream, callContext));
        
    }

    [Fact]
    public async void TestGetGetByIdValid()
    {
        var lstAut = new List<AuthorGrpcDto>();
        lstAut.Add(new AuthorGrpcDto
            {
                AuthorId = "RE1",
                FirstName = "FirstName",
                LastName = "LastName",
                BirthDate = "01/01/1999",
                Country = "DE"
            }
        );
        var bok = new BookGrpcDto
        {
            Id = "978-0000000001",
            Authors = {lstAut}
        };

        //GRPC setup
        var callContext = TestServerCallContext.Create();

        var response = await Boc.GetBookByISBN(new RequestWithISBN{Id = "978-0000000001"}, callContext);

        Assert.Equal(JsonConvert.SerializeObject(bok), JsonConvert.SerializeObject(response));
    }

    [Fact]
    public async void TestGetGetByIdInValid()
    {
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>(() => Boc.GetBookByISBN(new RequestWithISBN{Id ="978-0000000009"}, callContext));
    }

    [Fact]
    public async void TestCreateSuccess()
    {
        var lstAut = new List<String>();
        lstAut.Add("RE1");
        var bok = new CreatingBooksGrpcDto
        {
            Id = "978-0000000001",
            Authors = {lstAut}
        };
        
        var lstAutNormal = new List<AuthorGrpcDto>();
        lstAutNormal.Add(new AuthorGrpcDto
            {
                AuthorId = "RE1",
                FirstName = "FirstName",
                LastName = "LastName",
                BirthDate = "01/01/1999",
                Country = "DE"
            }
        );
        var bokNormal = new BookGrpcDto
        {
            Id = "978-0000000001",
            Authors = {lstAutNormal}
        };
        
        var callContext = TestServerCallContext.Create();
        var response = await Boc.AddNewBook(bok, callContext);

        Assert.Equal(JsonConvert.SerializeObject(bokNormal), JsonConvert.SerializeObject(response));
    }
    
    [Fact]
    public async void TestCreateNull()
    {
        var lstAut = new List<String>();
        lstAut.Add("RE1");
        var bok = new CreatingBooksGrpcDto
        {
            Id = "978-0000000002",
            Authors = {lstAut}
        };
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>(() => Boc.AddNewBook(bok, callContext));
    }

    [Fact]
    public async void TestUpdateNullDto()
    {
        var lstAut = new List<String>();
        lstAut.Add("RE1");
        var bok = new CreatingBooksGrpcDto
        {
            Id = "978-0000000001",
            Authors = {lstAut}
        };
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>(() => Boc.ModifyBook(bok, callContext));
    }

    [Fact]
    public async void TestUpdateSuccess()
    {
        var lstAut = new List<String>();
        lstAut.Add("RE1");
        var bok = new CreatingBooksGrpcDto
        {
            Id = "978-0000000002",
            Authors = {lstAut}
        };
        
        var lstAutNormal = new List<AuthorGrpcDto>();
        lstAutNormal.Add(new AuthorGrpcDto
            {
                AuthorId = "RE1",
                FirstName = "FirstName",
                LastName = "LastName",
                BirthDate = "01/01/1999",
                Country = "DE"
            }
        );
        var bokNormal = new BookGrpcDto
        {
            Id = "978-0000000002",
            Authors = {lstAutNormal}
        };
        
        var callContext = TestServerCallContext.Create();
        var response = await Boc.ModifyBook(bok, callContext);

        Assert.Equal(JsonConvert.SerializeObject(bokNormal), JsonConvert.SerializeObject(response));
    }

    [Fact]
    public async void TestDeleteNullDto()
    {
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>(() => Boc.DeleteBook(new RequestWithISBN(new RequestWithISBN{ Id = "978-0000000001"}), callContext));
    }

    [Fact]
    public async void TestDeleteSuccess()
    {
        var lstAutNormal = new List<AuthorGrpcDto>();
        lstAutNormal.Add(new AuthorGrpcDto
            {
                AuthorId = "RE1",
                FirstName = "FirstName",
                LastName = "LastName",
                BirthDate = "01/01/1999",
                Country = "DE"
            }
        );
        var bokNormal = new BookGrpcDto
        {
            Id = "978-0000000002",
            Authors = {lstAutNormal}
        };
        var callContext = TestServerCallContext.Create();
        var response = await Boc.DeleteBook(new RequestWithISBN(new RequestWithISBN { Id = "978-0000000002" }), callContext);

        Assert.Equal(JsonConvert.SerializeObject(bokNormal), JsonConvert.SerializeObject(response));
    }
}