using APIPublisher.Controllers;
using APIPublisher.Domain.Publishers;
using APIPublisher.Domain.Shared;
using APIPublisher.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace APIPublisherTest.Controllers;

public abstract class PublisherControllerTestSetup : IDisposable
{
    protected PublishersController Puc;

    protected PublisherDto PublisherDto;
    protected PublisherDto PublisherDto2;
    protected PublisherDto PublisherDto3;
    protected CreatingPublisherDto CreatingPublisherDto;

    public PublisherControllerTestSetup()
    {
        PublisherDto = new PublisherDto("AWE", "Addison Wesley", "US");
        List<PublisherDto> listDto = new List<PublisherDto>();
        listDto.Add(PublisherDto);

        PublisherDto2 = new PublisherDto("ORE", "O'Reilly", "GB");

        PublisherDto3 = new PublisherDto("MAN", "Manning Publications", "FR");
        
        CreatingPublisherDto = new CreatingPublisherDto("AWE", "Addison Wesley", "US");
        
        var PublisherService = new Mock<IPublisherService>();
        PublisherService.Setup(au => au.GetAllAsync().Result).Returns(listDto);
        PublisherService.Setup(au => au.GetByIdAsync(new PublisherId("AWE")).Result).Returns(PublisherDto);
        PublisherService.Setup(au => au.GetByIdAsync(new PublisherId("LUL")).Result).Returns<PublisherDto>(null);
        PublisherService.Setup(au => au.AddAsync(CreatingPublisherDto).Result).Returns(PublisherDto);
        PublisherService.Setup(au => au.UpdateAsync(PublisherDto).Result).Returns<PublisherDto>(null);
        PublisherService.Setup(au => au.UpdateAsync(PublisherDto2)).ReturnsAsync(PublisherDto2);
        PublisherService.Setup(au => au.UpdateAsync(PublisherDto3).Result)
            .Throws(new BusinessRuleValidationException("Whatever"));
        PublisherService.Setup(au => au.DeleteAsync(new PublisherId("AWE")).Result).Returns<PublisherDto>(null);
        PublisherService.Setup(au => au.DeleteAsync(new PublisherId("ORE")).Result).Returns(PublisherDto2);
        PublisherService.Setup(au => au.DeleteAsync(new PublisherId("MAN")).Result)
            .Throws(new BusinessRuleValidationException("Whatever"));
        
        Puc = new PublishersController(PublisherService.Object);
    }

    public void Dispose()
    {
    }
}

public class PublisherControllerTest : PublisherControllerTestSetup
{
    [Fact]
    public async void TestGetAllAsync()
    {
        var pub = new PublisherDto("AWE", "Addison Wesley", "US");
        List<PublisherDto> listPub = new List<PublisherDto>();
        listPub.Add(pub);
        var pdto = await Puc.GetAll();
        var result = pdto.Value;
        Assert.Equal(JsonConvert.SerializeObject(listPub), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestGetGetByIdValid()
    {
        var pdto = await Puc.GetGetById("AWE");
        var result = pdto.Value;

        Assert.Equal(JsonConvert.SerializeObject(PublisherDto), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestGetGetByIdInValid()
    {
        var pdto = await Puc.GetGetById("LUL");

        var actionResult = Assert.IsType<ActionResult<PublisherDto>>(pdto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }
    
    [Fact]
    public async void TestCreate()
    {
        var pdto = await Puc.Create(CreatingPublisherDto);
        var result = pdto.Value;

        Assert.Equal(JsonConvert.SerializeObject(PublisherDto), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestUpdateMismatchId()
    {
        var pdto = await Puc.Update("NO1", PublisherDto);

        var actionResult = Assert.IsType<ActionResult<PublisherDto>>(pdto);
        Assert.IsType<BadRequestResult>(actionResult.Result);
    }

    [Fact]
    public async void TestUpdateNullDto()
    {
        var pdto = await Puc.Update("AWE", PublisherDto);

        var actionResult = Assert.IsType<ActionResult<PublisherDto>>(pdto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestUpdateSuccess()
    {
        var pdto = await Puc.Update("ORE", PublisherDto2);

        var actionResult = Assert.IsType<ActionResult<PublisherDto>>(pdto);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = pdto.Result as OkObjectResult;
        var actualValue = returnValue?.Value;


        Assert.Equal(JsonConvert.SerializeObject(PublisherDto2), JsonConvert.SerializeObject(actualValue));
    }

    [Fact]
    public async void TestUpdateBadReq()
    {
        var pdto = await Puc.Update("MAN", PublisherDto3);

        var actionResult = Assert.IsType<ActionResult<PublisherDto>>(pdto);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }

    [Fact]
    public async void TestDeleteNullDto()
    {
        var pdto = await Puc.HardDelete("AWE");

        var actionResult = Assert.IsType<ActionResult<PublisherDto>>(pdto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestDeleteSuccess()
    {
        var pdto = await Puc.HardDelete("ORE");

        var actionResult = Assert.IsType<ActionResult<PublisherDto>>(pdto);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = pdto.Result as OkObjectResult;
        var actualValue = returnValue?.Value;


        Assert.Equal(JsonConvert.SerializeObject(PublisherDto2), JsonConvert.SerializeObject(actualValue));
    }

    [Fact]
    public async void TestDeleteBadReq()
    {
        var pdto = await Puc.HardDelete("MAN");

        var actionResult = Assert.IsType<ActionResult<PublisherDto>>(pdto);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }
}