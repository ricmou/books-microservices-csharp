using APICategories.Controllers;
using APICategories.Domain.Books;
using APICategories.Domain.Categories;
using APICategories.Domain.Shared;
using APICategories.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace APICategoriesTest.Controllers;

public abstract class BookControllerTestSetup : IDisposable
{
    protected BooksController Cac;

    protected BooksDto BooksDto;
    protected BooksDto BooksDto2;
    protected CreatingBooksDto CreatingBooksDto;
    protected CreatingBooksDto CreatingBooksDto2;
    protected CreatingBooksDto CreatingBooksDto3;

    public BookControllerTestSetup()
    {
        var lstCat = new List<CategoryDto>();
        lstCat.Add(new CategoryDto("RE1", "CatName"));
        BooksDto = new BooksDto("978-0000000001", lstCat);
        
        List<BooksDto> listDto = new List<BooksDto>();
        listDto.Add(BooksDto);

        BooksDto2 = new BooksDto("978-0000000002", lstCat);

        var lstStr = new List<String>();
        lstStr.Add("RE1");
        CreatingBooksDto = new CreatingBooksDto("978-0000000001", lstStr);
        CreatingBooksDto2 = new CreatingBooksDto("978-0000000002", lstStr);
        CreatingBooksDto3 = new CreatingBooksDto("978-0000000003", lstStr);

        var BookService = new Mock<IBooksService>();
        BookService.Setup(bk => bk.GetAllAsync().Result).Returns(listDto);
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
        
        Cac = new BooksController(BookService.Object);
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
        var lstCat = new List<CategoryDto>();
        lstCat.Add(new CategoryDto("RE1", "CatName"));
        var bok = new BooksDto("978-0000000001", lstCat);
        List<BooksDto> listBok = new List<BooksDto>();
        listBok.Add(bok);
        var cdto = await Cac.GetAll();
        var result = cdto.Value;
        Assert.Equal(JsonConvert.SerializeObject(listBok), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestGetGetByIdValid()
    {
        var cdto = await Cac.GetGetById("978-0000000001");
        var result = cdto.Value;

        Assert.Equal(JsonConvert.SerializeObject(BooksDto), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestGetGetByIdInValid()
    {
        var cdto = await Cac.GetGetById("978-0000000009");

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(cdto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestCreate()
    {
        var cdto = await Cac.Create(CreatingBooksDto);
        var result = cdto.Value;

        Assert.Equal(JsonConvert.SerializeObject(BooksDto), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestUpdateMismatchId()
    {
        var cdto = await Cac.Update("978-0000000002", CreatingBooksDto);

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(cdto);
        Assert.IsType<BadRequestResult>(actionResult.Result);
    }

    [Fact]
    public async void TestUpdateNullDto()
    {
        var cdto = await Cac.Update("978-0000000001", CreatingBooksDto);

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(cdto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestUpdateSuccess()
    {
        var cdto = await Cac.Update("978-0000000002", CreatingBooksDto2);

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(cdto);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = cdto.Result as OkObjectResult;
        var actualValue = returnValue?.Value;


        Assert.Equal(JsonConvert.SerializeObject(BooksDto2), JsonConvert.SerializeObject(actualValue));
    }

    [Fact]
    public async void TestUpdateBadReq()
    {
        var cdto = await Cac.Update("978-0000000003", CreatingBooksDto3);

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(cdto);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }

    [Fact]
    public async void TestDeleteNullDto()
    {
        var cdto = await Cac.HardDelete("978-0000000001");

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(cdto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestDeleteSuccess()
    {
        var cdto = await Cac.HardDelete("978-0000000002");

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(cdto);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = cdto.Result as OkObjectResult;
        var actualValue = returnValue?.Value;


        Assert.Equal(JsonConvert.SerializeObject(BooksDto2), JsonConvert.SerializeObject(actualValue));
    }

    [Fact]
    public async void TestDeleteBadReq()
    {
        var cdto = await Cac.HardDelete("978-0000000003");

        var actionResult = Assert.IsType<ActionResult<BooksDto>>(cdto);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }
}
