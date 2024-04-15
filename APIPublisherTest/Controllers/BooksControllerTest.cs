using APIPublisher.Controllers;
using APIPublisher.Domain.Books;
using APIPublisher.Domain.Publishers;
using APIPublisher.Domain.Shared;
using APIPublisher.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace APIPublisherTest.Controllers;

public abstract class BookControllerTestSetup : IDisposable
{
    protected BooksController Auc;

    protected BooksDto BooksDto;
    protected BooksDto BooksDto2;
    protected CreatingBooksDto CreatingBooksDto;
    protected CreatingBooksDto CreatingBooksDto2;
    protected CreatingBooksDto CreatingBooksDto3;

    public BookControllerTestSetup()
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
        BookService.Setup(bk => bk.GetAllFromPublisherAsync(new PublisherId("ORE")).Result).Returns<List<BooksDto>>(null);
        BookService.Setup(bk => bk.GetByIdAsync(new BookId("978-0000000001")).Result).Returns(BooksDto);
        BookService.Setup(bk => bk.GetByIdAsync(new BookId("978-0000000009")).Result).Returns<BooksDto>(null);
        BookService.Setup(bk => bk.AddAsync(CreatingBooksDto).Result).Returns(BooksDto);
        BookService.Setup(bk => bk.UpdateAsync(CreatingBooksDto).Result).Returns<BooksDto>(null);
        BookService.Setup(bk => bk.UpdateAsync(CreatingBooksDto2)).ReturnsAsync(BooksDto2);
        BookService.Setup(bk => bk.UpdateAsync(CreatingBooksDto3).Result)
            .Throws(new BusinessRuleValidationException("Whatever"));
        BookService.Setup(bk => bk.DeleteAsync(new BookId("978-0000000001")).Result).Returns<BooksDto>(null);
        BookService.Setup(bk => bk.DeleteAsync(new BookId("978-0000000002")).Result).Returns(BooksDto2);
        BookService.Setup(bk => bk.DeleteAsync(new BookId("978-0000000003")).Result)
            .Throws(new BusinessRuleValidationException("Whatever"));
        
        Auc = new BooksController(BookService.Object);
    }

    public void Dispose()
    {
    }
}

public class BooksControllerTest : BookControllerTestSetup
{
    [Fact]
    public async void TestGetAllAsync()
    {
        var bok = new BooksDto("978-0000000001", new PublisherDto("AWE", "Addison Wesley", "US"));
        List<BooksDto> listBok = new List<BooksDto>();
        listBok.Add(bok);
        var pdto = await Auc.GetAll();
        var result = pdto.Value;
        Assert.Equal(JsonConvert.SerializeObject(listBok), JsonConvert.SerializeObject(result));
    }
    
    [Fact]
    public async void TestGetByPublisherSuccess()
    {
        var bok = new BooksDto("978-0000000001", new PublisherDto("AWE", "Addison Wesley", "US"));
        List<BooksDto> listBok = new List<BooksDto>();
        listBok.Add(bok);
        var pdto = await Auc.GetAllFromPublisher("AWE");
        var result = pdto.Value;
        Assert.Equal(JsonConvert.SerializeObject(listBok), JsonConvert.SerializeObject(result));
    }
    
    [Fact]
    public async void TestGetByPublisherFail()
    {
        var pdto = await Auc.GetAllFromPublisher("ORE");

        var actionResult = Assert.IsType<ActionResult<IEnumerable<BooksDto>>>(pdto);
        Assert.Null(actionResult.Value);
    }

    [Fact]
    public async void TestGetGetByIdValid()
    {
        var pdto = await Auc.GetGetById("978-0000000001");
        var result = pdto.Value;

        Assert.Equal(JsonConvert.SerializeObject(BooksDto), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestGetGetByIdInValid()
    {
        var pdto = await Auc.GetGetById("978-0000000009");

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(pdto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestCreate()
    {
        var pdto = await Auc.Create(CreatingBooksDto);
        var result = pdto.Value;

        Assert.Equal(JsonConvert.SerializeObject(BooksDto), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestUpdateMismatchId()
    {
        var pdto = await Auc.Update("978-0000000002", CreatingBooksDto);

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(pdto);
        Assert.IsType<BadRequestResult>(actionResult.Result);
    }

    [Fact]
    public async void TestUpdateNullDto()
    {
        var pdto = await Auc.Update("978-0000000001", CreatingBooksDto);

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(pdto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestUpdateSuccess()
    {
        var pdto = await Auc.Update("978-0000000002", CreatingBooksDto2);

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(pdto);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = pdto.Result as OkObjectResult;
        var actualValue = returnValue?.Value;


        Assert.Equal(JsonConvert.SerializeObject(BooksDto2), JsonConvert.SerializeObject(actualValue));
    }

    [Fact]
    public async void TestUpdateBadReq()
    {
        var pdto = await Auc.Update("978-0000000003", CreatingBooksDto3);

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(pdto);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }

    [Fact]
    public async void TestDeleteNullDto()
    {
        var pdto = await Auc.HardDelete("978-0000000001");

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(pdto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestDeleteSuccess()
    {
        var pdto = await Auc.HardDelete("978-0000000002");

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(pdto);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = pdto.Result as OkObjectResult;
        var actualValue = returnValue?.Value;


        Assert.Equal(JsonConvert.SerializeObject(BooksDto2), JsonConvert.SerializeObject(actualValue));
    }

    [Fact]
    public async void TestDeleteBadReq()
    {
        var pdto = await Auc.HardDelete("978-0000000003");

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(pdto);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }
}
