using APIClients.Controllers;
using APIClients.Domain.Clients;
using APIClients.Domain.Shared;
using APIClients.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;

namespace APIClientsTest.Controllers;

public abstract class ClientControllerTestSetup : IDisposable
{
    protected ClientsController Auc;

    protected ClientDto ClientDto;
    protected ClientDto ClientDto2;
    protected ClientDto ClientDto3;
    protected CreatingClientDto CreatingClientDto;

    public ClientControllerTestSetup()
    {
        ClientDto = new ClientDto("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "Sebastian Vettel",
            "Wald-Michelbacher Straße, 66", "Heppenheim", "5553-351", "DE");
        List<ClientDto> listDto = new List<ClientDto>();
        listDto.Add(ClientDto);

        ClientDto2 = new ClientDto("cccccccc-cccc-cccc-cccc-cccccccccccc", "Daniel Joseph Ricciardo",
            "Cliff Street, 77", "Perth", "4201-898", "AU");

        ClientDto3 = new ClientDto("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "Juan Pablo Montoya Roldán",
            "Autopista Norte, 54", "Bogotá", "6546-444","CO");
        
        CreatingClientDto = new CreatingClientDto("Sebastian Vettel",
            "Wald-Michelbacher Straße, 66", "Heppenheim", "5553-351", "DE");
        
        var ClientService = new Mock<IClientsService>();
        ClientService.Setup(au => au.GetAllAsync().Result).Returns(listDto);
        ClientService.Setup(au => au.GetByIdAsync(new ClientId("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")).Result).Returns(ClientDto);
        ClientService.Setup(au => au.GetByIdAsync(new ClientId("ffffffff-ffff-ffff-ffff-ffffffffffff")).Result).Returns<ClientDto>(null);
        ClientService.Setup(au => au.AddAsync(CreatingClientDto).Result).Returns(ClientDto);
        ClientService.Setup(au => au.UpdateAsync(ClientDto).Result).Returns<ClientDto>(null);
        ClientService.Setup(au => au.UpdateAsync(ClientDto2)).ReturnsAsync(ClientDto2);
        ClientService.Setup(au => au.UpdateAsync(ClientDto3).Result)
            .Throws(new BusinessRuleValidationException("Whatever"));
        ClientService.Setup(au => au.DeleteAsync(new ClientId("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")).Result).Returns<ClientDto>(null);
        ClientService.Setup(au => au.DeleteAsync(new ClientId("cccccccc-cccc-cccc-cccc-cccccccccccc")).Result).Returns(ClientDto2);
        ClientService.Setup(au => au.DeleteAsync(new ClientId("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")).Result)
            .Throws(new BusinessRuleValidationException("Whatever"));
        
        Auc = new ClientsController(ClientService.Object);
    }

    public void Dispose()
    {
    }
}

public class ClientsControllerTest : ClientControllerTestSetup
{
    [Fact]
    public async void TestGetAllAsync()
    {
        var cli = new ClientDto("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "Sebastian Vettel",
            "Wald-Michelbacher Straße, 66", "Heppenheim", "5553-351", "DE");
        List<ClientDto> listCli = new List<ClientDto>();
        listCli.Add(cli);
        var cdto = await Auc.GetAll();
        var result = cdto.Value;
        Assert.Equal(JsonConvert.SerializeObject(listCli), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestGetGetByIdValid()
    {
        var cdto = await Auc.GetGetById("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var result = cdto.Value;

        Assert.Equal(JsonConvert.SerializeObject(ClientDto), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestGetGetByIdInValid()
    {
        var cdto = await Auc.GetGetById("ffffffff-ffff-ffff-ffff-ffffffffffff");

        var actionResult = Assert.IsType<ActionResult<ClientDto>>(cdto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }
    
    [Fact]
    public async void TestCreate()
    {
        var cdto = await Auc.Create(CreatingClientDto);
        var result = cdto.Value;

        Assert.Equal(JsonConvert.SerializeObject(ClientDto), JsonConvert.SerializeObject(result));
    }

    [Fact]
    public async void TestUpdateMismatchId()
    {
        var cdto = await Auc.Update("NO1", ClientDto);

        var actionResult = Assert.IsType<ActionResult<ClientDto>>(cdto);
        Assert.IsType<BadRequestResult>(actionResult.Result);
    }

    [Fact]
    public async void TestUpdateNullDto()
    {
        var cdto = await Auc.Update("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", ClientDto);

        var actionResult = Assert.IsType<ActionResult<ClientDto>>(cdto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestUpdateSuccess()
    {
        var cdto = await Auc.Update("cccccccc-cccc-cccc-cccc-cccccccccccc", ClientDto2);

        var actionResult = Assert.IsType<ActionResult<ClientDto>>(cdto);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = cdto.Result as OkObjectResult;
        var actualValue = returnValue?.Value;


        Assert.Equal(JsonConvert.SerializeObject(ClientDto2), JsonConvert.SerializeObject(actualValue));
    }

    [Fact]
    public async void TestUpdateBadReq()
    {
        var cdto = await Auc.Update("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", ClientDto3);

        var actionResult = Assert.IsType<ActionResult<ClientDto>>(cdto);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }

    [Fact]
    public async void TestDeleteNullDto()
    {
        var cdto = await Auc.HardDelete("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        var actionResult = Assert.IsType<ActionResult<ClientDto>>(cdto);
        Assert.IsType<NotFoundResult>(actionResult.Result);
    }

    [Fact]
    public async void TestDeleteSuccess()
    {
        var cdto = await Auc.HardDelete("cccccccc-cccc-cccc-cccc-cccccccccccc");

        var actionResult = Assert.IsType<ActionResult<ClientDto>>(cdto);
        Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnValue = cdto.Result as OkObjectResult;
        var actualValue = returnValue?.Value;


        Assert.Equal(JsonConvert.SerializeObject(ClientDto2), JsonConvert.SerializeObject(actualValue));
    }

    [Fact]
    public async void TestDeleteBadReq()
    {
        var cdto = await Auc.HardDelete("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        var actionResult = Assert.IsType<ActionResult<ClientDto>>(cdto);
        Assert.IsType<BadRequestObjectResult>(actionResult.Result);
    }
}