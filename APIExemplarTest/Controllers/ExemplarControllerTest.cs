using APIExemplar.Controllers;
using APIExemplar.Domain.Exemplars;
using APIExemplar.Domain.Shared;
using APIExemplar.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace APIExemplarTest.Controllers;

public abstract class ExemplarControllerTestSetup : IDisposable
{
    protected ExemplarController Auc;

    protected ExemplarDto ExemplarDto;
    protected ExemplarDto ExemplarDto2;
    protected ExemplarDto ExemplarDto3;
    protected CreatingExemplarDto CreatingExemplarDto;

    public ExemplarControllerTestSetup()
    {
        ExemplarDto = new ExemplarDto("11111111-1111-1111-1111-111111111111", "978-1491900864",
            3, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "05/01/2017");
        List<ExemplarDto> listDto = new List<ExemplarDto>();
        listDto.Add(ExemplarDto);

        ExemplarDto2 = new ExemplarDto("22222222-2222-2222-2222-222222222222", "978-1617292545",
            1, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "05/01/2018");

        ExemplarDto3 = new ExemplarDto("33333333-3333-3333-3333-333333333333", "978-0321356680",
            4, "cccccccc-cccc-cccc-cccc-cccccccccccc", "05/01/2019");
        
        CreatingExemplarDto = new CreatingExemplarDto("978-1491900864",
            3, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "05/01/2017");
        
        var ExemplarService = new Mock<IExemplarService>();
        ExemplarService.Setup(ex => ex.GetAllAsync().Result).Returns(listDto);
        ExemplarService.Setup(ex => ex.GetByIdAsync(new ExemplarId("11111111111111111111111111111111")).Result).Returns(ExemplarDto);
        ExemplarService.Setup(ex => ex.GetByIdAsync(new ExemplarId("99999999999999999999999999999999")).Result).Returns<ExemplarDto>(null);
        ExemplarService.Setup(ex => ex.AddAsync(CreatingExemplarDto).Result).Returns(ExemplarDto);
        ExemplarService.Setup(ex => ex.UpdateAsync(ExemplarDto).Result).Returns<ExemplarDto>(null);
        ExemplarService.Setup(ex => ex.UpdateAsync(ExemplarDto2)).ReturnsAsync(ExemplarDto2);
        ExemplarService.Setup(ex => ex.UpdateAsync(ExemplarDto3).Result)
            .Throws(new BusinessRuleValidationException("Whatever"));
        ExemplarService.Setup(ex => ex.DeleteAsync(new ExemplarId("11111111111111111111111111111111")).Result).Returns<ExemplarDto>(null);
        ExemplarService.Setup(ex => ex.DeleteAsync(new ExemplarId("22222222222222222222222222222222")).Result).Returns(ExemplarDto2);
        ExemplarService.Setup(ex => ex.DeleteAsync(new ExemplarId("33333333333333333333333333333333")).Result)
            .Throws(new BusinessRuleValidationException("Whatever"));
        ExemplarService.Setup(ex => ex.GetByBookIdAsync(new BookId("978-1491900864")).Result).Returns(listDto);
        ExemplarService.Setup(ex => ex.GetByBookIdAsync(new BookId("978-9999999999")).Result).Returns<List<ExemplarDto>>(null);
        ExemplarService.Setup(ex => ex.GetBySellerIdAsync(new ClientId("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb")).Result).Returns(listDto);
        ExemplarService.Setup(ex => ex.GetBySellerIdAsync(new ClientId("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")).Result).Returns<List<ExemplarDto>>(null);
        
        Auc = new ExemplarController(ExemplarService.Object);
    }

    public void Dispose()
    {
    }
}

public class ExemplarControllerTest : ExemplarControllerTestSetup
{
    [Fact]
    public async void TestGetAllAsync()
    {
        var exp = new ExemplarDto("11111111-1111-1111-1111-111111111111", "978-1491900864",
            3, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "05/01/2017");
        List<ExemplarDto> listExp = new List<ExemplarDto>();
        listExp.Add(exp);
        var edto = await Auc.GetAll();
        var result = edto.Value;
        Assert.Equal(JsonConvert.SerializeObject(listExp), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestGetGetByIdValid()
    {
        var edto = await Auc.GetGetById("11111111-1111-1111-1111-111111111111");
        var result = edto.Value;

        Assert.Equal(JsonConvert.SerializeObject(ExemplarDto), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestGetGetByIdInValid()
    {
        var edto = await Auc.GetGetById("99999999-9999-9999-9999-999999999999");

        var actionResult = Assert.IsType<ActionResult<ExemplarDto>>(edto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }
    
    [Fact]
    public async void TestCreate()
    {
        var edto = await Auc.Create(CreatingExemplarDto);
        var result = edto.Value;

        Assert.Equal(JsonConvert.SerializeObject(ExemplarDto), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestUpdateMismatchId()
    {
        var edto = await Auc.Update("NO1", ExemplarDto);

        var actionResult = Assert.IsType<ActionResult<ExemplarDto>>(edto);
        Assert.IsType<BadRequestResult>(actionResult.Result);
    }

    [Fact]
    public async void TestUpdateNullDto()
    {
        var edto = await Auc.Update("11111111-1111-1111-1111-111111111111", ExemplarDto);

        var actionResult = Assert.IsType<ActionResult<ExemplarDto>>(edto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestUpdateSuccess()
    {
        var edto = await Auc.Update("22222222-2222-2222-2222-222222222222", ExemplarDto2);

        var actionResult = Assert.IsType<ActionResult<ExemplarDto>>(edto);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = edto.Result as OkObjectResult;
        var actualValue = returnValue?.Value;


        Assert.Equal(JsonConvert.SerializeObject(ExemplarDto2), JsonConvert.SerializeObject(actualValue));
    }

    [Fact]
    public async void TestUpdateBadReq()
    {
        var edto = await Auc.Update("33333333-3333-3333-3333-333333333333", ExemplarDto3);

        var actionResult = Assert.IsType<ActionResult<ExemplarDto>>(edto);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }

    [Fact]
    public async void TestDeleteNullDto()
    {
        var edto = await Auc.HardDelete("11111111-1111-1111-1111-111111111111");

        var actionResult = Assert.IsType<ActionResult<ExemplarDto>>(edto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestDeleteSuccess()
    {
        var edto = await Auc.HardDelete("22222222-2222-2222-2222-222222222222");

        var actionResult = Assert.IsType<ActionResult<ExemplarDto>>(edto);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = edto.Result as OkObjectResult;
        var actualValue = returnValue?.Value;


        Assert.Equal(JsonConvert.SerializeObject(ExemplarDto2), JsonConvert.SerializeObject(actualValue));
    }

    [Fact]
    public async void TestDeleteBadReq()
    {
        var edto = await Auc.HardDelete("33333333-3333-3333-3333-333333333333");

        var actionResult = Assert.IsType<ActionResult<ExemplarDto>>(edto);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }
    
    [Fact]
    public async void TestGetByBookSuccess()
    {
        var lstExp = new List<ExemplarDto>();
        lstExp.Add(new ExemplarDto("11111111-1111-1111-1111-111111111111", "978-1491900864",
            3, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "05/01/2017"));
        var edto = await Auc.GetByBookId("978-1491900864");
        var result = edto.Value;
        Assert.Equal(JsonConvert.SerializeObject(lstExp), JsonConvert.SerializeObject(result));
    }
    
    [Fact]
    public async void TestGetByBookFail()
    {
        var edto = await Auc.GetByBookId("978-9999999999");

        var actionResult = Assert.IsType<ActionResult<IEnumerable<ExemplarDto>>>(edto);
        Assert.Null(actionResult.Value);
    }
    
    [Fact]
    public async void TestGetBySellerSuccess()
    {
        var lstExp = new List<ExemplarDto>();
        lstExp.Add(new ExemplarDto("11111111-1111-1111-1111-111111111111", "978-1491900864",
            3, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "05/01/2017"));
        var edto = await Auc.GetByClientId("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");
        var result = edto.Value;
        Assert.Equal(JsonConvert.SerializeObject(lstExp), JsonConvert.SerializeObject(result));
    }
    
    [Fact]
    public async void TestGetBySellerFail()
    {
        var edto = await Auc.GetByClientId("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

        var actionResult = Assert.IsType<ActionResult<IEnumerable<ExemplarDto>>>(edto);
        Assert.Null(actionResult.Value);
    }
}