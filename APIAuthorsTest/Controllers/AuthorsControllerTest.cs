using APIAuthors.Controllers;
using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Shared;
using APIAuthors.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace APIAuthorsTest.Controllers;

public abstract class AuthorControllerTestSetup : IDisposable
{
    protected AuthorsController Auc;

    protected AuthorDto AuthorDto;
    protected AuthorDto AuthorDto2;
    protected AuthorDto AuthorDto3;
    protected CreatingAuthorsDto CreatingAuthorDto;

    public AuthorControllerTestSetup()
    {
        AuthorDto = new AuthorDto("RE1", "FirstName", "LastName", "01/01/1999", "DE");
        List<AuthorDto> listDto = new List<AuthorDto>();
        listDto.Add(AuthorDto);

        AuthorDto2 = new AuthorDto("RE2", "FirstName2", "LastName2", "02/02/1999", "CA");

        AuthorDto3 = new AuthorDto("RE3", "FirstName3", "LastName3", "03/03/1999", "SE");
        
        CreatingAuthorDto = new CreatingAuthorsDto("RE1", "FirstName", "LastName", "01/01/1999", "DE");
        
        var AuthorService = new Mock<IAuthorsService>();
        AuthorService.Setup(au => au.GetAllAsync().Result).Returns(listDto);
        AuthorService.Setup(au => au.GetByIdAsync(new AuthorId("RE1")).Result).Returns(AuthorDto);
        AuthorService.Setup(au => au.GetByIdAsync(new AuthorId("RE9")).Result).Returns<AuthorDto>(null);
        AuthorService.Setup(au => au.AddAsync(CreatingAuthorDto).Result).Returns(AuthorDto);
        AuthorService.Setup(au => au.UpdateAsync(AuthorDto).Result).Returns<AuthorDto>(null);
        AuthorService.Setup(au => au.UpdateAsync(AuthorDto2)).ReturnsAsync(AuthorDto2);
        AuthorService.Setup(au => au.UpdateAsync(AuthorDto3).Result)
            .Throws(new BusinessRuleValidationException("Whatever"));
        AuthorService.Setup(au => au.DeleteAsync(new AuthorId("RE1")).Result).Returns<AuthorDto>(null);
        AuthorService.Setup(au => au.DeleteAsync(new AuthorId("RE2")).Result).Returns(AuthorDto2);
        AuthorService.Setup(au => au.DeleteAsync(new AuthorId("RE3")).Result)
            .Throws(new BusinessRuleValidationException("Whatever"));
        
        Auc = new AuthorsController(AuthorService.Object);
    }

    public void Dispose()
    {
    }
}

public class AuthorsControllerTest : AuthorControllerTestSetup
{
    [Fact]
    public async void TestGetAllAsync()
    {
        var aut = new AuthorDto("RE1", "FirstName", "LastName", "01/01/1999", "DE");
        List<AuthorDto> listAut = new List<AuthorDto>();
        listAut.Add(aut);
        var adto = await Auc.GetAll();
        var result = adto.Value;
        Assert.Equal(JsonConvert.SerializeObject(listAut), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestGetGetByIdValid()
    {
        var adto = await Auc.GetGetById("RE1");
        var result = adto.Value;

        Assert.Equal(JsonConvert.SerializeObject(AuthorDto), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestGetGetByIdInValid()
    {
        var adto = await Auc.GetGetById("RE9");

        var actionResult = Assert.IsType<ActionResult<AuthorDto>>(adto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }
    
    [Fact]
    public async void TestCreate()
    {
        var adto = await Auc.Create(CreatingAuthorDto);
        var result = adto.Value;

        Assert.Equal(JsonConvert.SerializeObject(AuthorDto), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestUpdateMismatchId()
    {
        var adto = await Auc.Update("NO1", AuthorDto);

        var actionResult = Assert.IsType<ActionResult<AuthorDto>>(adto);
        Assert.IsType<BadRequestResult>(actionResult.Result);
    }

    [Fact]
    public async void TestUpdateNullDto()
    {
        var adto = await Auc.Update("RE1", AuthorDto);

        var actionResult = Assert.IsType<ActionResult<AuthorDto>>(adto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestUpdateSuccess()
    {
        var adto = await Auc.Update("RE2", AuthorDto2);

        var actionResult = Assert.IsType<ActionResult<AuthorDto>>(adto);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = adto.Result as OkObjectResult;
        var actualValue = returnValue?.Value;


        Assert.Equal(JsonConvert.SerializeObject(AuthorDto2), JsonConvert.SerializeObject(actualValue));
    }

    [Fact]
    public async void TestUpdateBadReq()
    {
        var adto = await Auc.Update("RE3", AuthorDto3);

        var actionResult = Assert.IsType<ActionResult<AuthorDto>>(adto);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }

    [Fact]
    public async void TestDeleteNullDto()
    {
        var adto = await Auc.HardDelete("RE1");

        var actionResult = Assert.IsType<ActionResult<AuthorDto>>(adto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestDeleteSuccess()
    {
        var adto = await Auc.HardDelete("RE2");

        var actionResult = Assert.IsType<ActionResult<AuthorDto>>(adto);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = adto.Result as OkObjectResult;
        var actualValue = returnValue?.Value;


        Assert.Equal(JsonConvert.SerializeObject(AuthorDto2), JsonConvert.SerializeObject(actualValue));
    }

    [Fact]
    public async void TestDeleteBadReq()
    {
        var adto = await Auc.HardDelete("RE3");

        var actionResult = Assert.IsType<ActionResult<AuthorDto>>(adto);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }
}