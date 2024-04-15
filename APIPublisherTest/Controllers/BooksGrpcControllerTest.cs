using APIPublisher;
using APIPublisher.Controllers;
using APIPublisher.Domain.Books;
using APIPublisher.Domain.Publishers;
using APIPublisher.Services;
using APIPublisherTest.Helpers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace APIPublisherTest.Controllers;

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
        BooksDto = new BooksDto("978-0000000001", new PublisherDto("AWE", "Addison Wesley", "US"));

        List<BooksDto> listDto = new List<BooksDto>();
        listDto.Add(BooksDto);

        BooksDto2 = new BooksDto("978-0000000002", new PublisherDto("AWE", "Addison Wesley", "US"));

        CreatingBooksDto = new CreatingBooksDto("978-0000000001", "AWE");
        CreatingBooksDto2 = new CreatingBooksDto("978-0000000002", "AWE");
        CreatingBooksDto3 = new CreatingBooksDto("978-0000000003", "AWE");

        var BookService = new Mock<IBooksService>();
        BookService.Setup(bk => bk.GetAllAsync().Result).Returns(listDto);
        BookService.Setup(bk => bk.GetAllFromPublisherAsync(new PublisherId("AWE")).Result).Returns(listDto);
        BookService.Setup(bk => bk.GetAllFromPublisherAsync(new PublisherId("ORE")).Result)
            .Returns<List<BooksDto>>(null);
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

        Boc = new BooksGrpcController(logger.Object, BookService.Object);
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
        var bok = new BookGrpcDto
        {
            Id = "978-0000000001",
            Publisher = new PublisherGrpcDto
            {
                PublisherId = "AWE",
                Name = "Addison Wesley",
                Country = "US"
            }
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
    public async void TestGetByPublisherSuccess()
    {
        var bok = new BookGrpcDto
        {
            Id = "978-0000000001",
            Publisher = new PublisherGrpcDto
            {
                PublisherId = "AWE",
                Name = "Addison Wesley",
                Country = "US"
            }
        };
        List<BookGrpcDto> listBok = new List<BookGrpcDto>();
        listBok.Add(bok);
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        var responseStream = new TestServerStreamWriter<BookGrpcDto>(callContext);

        using var call =
            Boc.GetAllBooksFromPublisher(new RequestWithPublisherId { Id = "AWE" }, responseStream, callContext);

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
    public async void TestGetByPublisherFail()
    {
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        var responseStream = new TestServerStreamWriter<BookGrpcDto>(callContext);

        await Assert.ThrowsAsync<RpcException>(() =>
            Boc.GetAllBooksFromPublisher(new RequestWithPublisherId { Id = "ORE" }, responseStream, callContext));
    }

    [Fact]
    public async void TestGetGetByIdValid()
    {
        var bok = new BookGrpcDto
        {
            Id = "978-0000000001",
            Publisher = new PublisherGrpcDto
            {
                PublisherId = "AWE",
                Name = "Addison Wesley",
                Country = "US"
            }
        };

        //GRPC setup
        var callContext = TestServerCallContext.Create();

        var response = await Boc.GetBookByISBN(new RequestWithISBN { Id = "978-0000000001" }, callContext);

        Assert.Equal(JsonConvert.SerializeObject(bok), JsonConvert.SerializeObject(response));
    }

    [Fact]
    public async void TestGetGetByIdInValid()
    {
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>(() =>
            Boc.GetBookByISBN(new RequestWithISBN { Id = "978-0000000009" }, callContext));
    }

    [Fact]
    public async void TestCreateSuccess()
    {
        var bok = new CreatingBooksGrpcDto
        {
            Id = "978-0000000001",
            PublisherId = "AWE"
        };

        var bokNormal = new BookGrpcDto
        {
            Id = "978-0000000001",
            Publisher = new PublisherGrpcDto
            {
                PublisherId = "AWE",
                Name = "Addison Wesley",
                Country = "US"
            }
        };

        var callContext = TestServerCallContext.Create();
        var response = await Boc.AddNewBook(bok, callContext);

        Assert.Equal(JsonConvert.SerializeObject(bokNormal), JsonConvert.SerializeObject(response));
    }

    [Fact]
    public async void TestCreateNull()
    {
        var bok = new CreatingBooksGrpcDto
        {
            Id = "978-0000000002",
            PublisherId = "AWE"
        };
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>(() => Boc.AddNewBook(bok, callContext));
    }

    [Fact]
    public async void TestUpdateNullDto()
    {
        var bok = new CreatingBooksGrpcDto
        {
            Id = "978-0000000001",
            PublisherId = "AWE"
        };
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>(() => Boc.ModifyBook(bok, callContext));
    }

    [Fact]
    public async void TestUpdateSuccess()
    {
        var bok = new CreatingBooksGrpcDto
        {
            Id = "978-0000000002",
            PublisherId = "AWE"
        };

        var bokNormal = new BookGrpcDto
        {
            Id = "978-0000000002",
            Publisher = new PublisherGrpcDto
            {
                PublisherId = "AWE",
                Name = "Addison Wesley",
                Country = "US"
            }
        };

        var callContext = TestServerCallContext.Create();
        var response = await Boc.ModifyBook(bok, callContext);

        Assert.Equal(JsonConvert.SerializeObject(bokNormal), JsonConvert.SerializeObject(response));
    }

    [Fact]
    public async void TestDeleteNullDto()
    {
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>(() =>
            Boc.DeleteBook(new RequestWithISBN(new RequestWithISBN { Id = "978-0000000001" }), callContext));
    }

    [Fact]
    public async void TestDeleteSuccess()
    {
        var bokNormal = new BookGrpcDto
        {
            Id = "978-0000000002",
            Publisher = new PublisherGrpcDto
            {
                PublisherId = "AWE",
                Name = "Addison Wesley",
                Country = "US"
            }
        };
        var callContext = TestServerCallContext.Create();
        var response = await Boc.DeleteBook(new RequestWithISBN(new RequestWithISBN { Id = "978-0000000002" }),
            callContext);

        Assert.Equal(JsonConvert.SerializeObject(bokNormal), JsonConvert.SerializeObject(response));
    }
}