using APIAuthors.Controllers;
using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Books;
using APIAuthors.Domain.Shared;
using APIAuthors.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace APIAuthorsTest.Controllers;

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
        var lstAut = new List<AuthorDto>();
        lstAut.Add(new AuthorDto("RE1", "FirstName", "LastName", "01/01/1999", "DE"));
        var bok = new BooksDto("978-0000000001", lstAut);
        List<BooksDto> listBok = new List<BooksDto>();
        listBok.Add(bok);
        var adto = await Auc.GetAll();
        var result = adto.Value;
        Assert.Equal(JsonConvert.SerializeObject(listBok), JsonConvert.SerializeObject(result));
    }
    
    [Fact]
    public async void TestGetByAuthorSuccess()
    {
        var lstAut = new List<AuthorDto>();
        lstAut.Add(new AuthorDto("RE1", "FirstName", "LastName", "01/01/1999", "DE"));
        var bok = new BooksDto("978-0000000001", lstAut);
        List<BooksDto> listBok = new List<BooksDto>();
        listBok.Add(bok);
        var adto = await Auc.GetAllFromAuthor("RE1");
        var result = adto.Value;
        Assert.Equal(JsonConvert.SerializeObject(listBok), JsonConvert.SerializeObject(result));
    }
    
    [Fact]
    public async void TestGetByAuthorFail()
    {
        var adto = await Auc.GetAllFromAuthor("RE2");

        var actionResult = Assert.IsType<ActionResult<IEnumerable<BooksDto>>>(adto);
        Assert.Null(actionResult.Value);
    }

    [Fact]
    public async void TestGetGetByIdValid()
    {
        var adto = await Auc.GetGetById("978-0000000001");
        var result = adto.Value;

        Assert.Equal(JsonConvert.SerializeObject(BooksDto), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestGetGetByIdInValid()
    {
        var adto = await Auc.GetGetById("978-0000000009");

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(adto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestCreate()
    {
        var adto = await Auc.Create(CreatingBooksDto);
        var result = adto.Value;

        Assert.Equal(JsonConvert.SerializeObject(BooksDto), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestUpdateMismatchId()
    {
        var adto = await Auc.Update("978-0000000002", CreatingBooksDto);

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(adto);
        Assert.IsType<BadRequestResult>(actionResult.Result);
    }

    [Fact]
    public async void TestUpdateNullDto()
    {
        var adto = await Auc.Update("978-0000000001", CreatingBooksDto);

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(adto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestUpdateSuccess()
    {
        var adto = await Auc.Update("978-0000000002", CreatingBooksDto2);

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(adto);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = adto.Result as OkObjectResult;
        var actualValue = returnValue?.Value;


        Assert.Equal(JsonConvert.SerializeObject(BooksDto2), JsonConvert.SerializeObject(actualValue));
    }

    [Fact]
    public async void TestUpdateBadReq()
    {
        var adto = await Auc.Update("978-0000000003", CreatingBooksDto3);

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(adto);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }

    [Fact]
    public async void TestDeleteNullDto()
    {
        var adto = await Auc.HardDelete("978-0000000001");

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(adto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestDeleteSuccess()
    {
        var adto = await Auc.HardDelete("978-0000000002");

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(adto);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = adto.Result as OkObjectResult;
        var actualValue = returnValue?.Value;


        Assert.Equal(JsonConvert.SerializeObject(BooksDto2), JsonConvert.SerializeObject(actualValue));
    }

    [Fact]
    public async void TestDeleteBadReq()
    {
        var adto = await Auc.HardDelete("978-0000000003");

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(adto);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }
}
