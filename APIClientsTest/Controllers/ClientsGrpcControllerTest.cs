using APIClients;
using APIClients.Controllers;
using APIClients.Domain.Clients;
using APIClients.Services;
using APIClientsTest.Helpers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace APIClientsTest.Controllers;

public abstract class ClientGrpcControllerTestSetup : IDisposable
{
    protected ClientsGrpcController Clc;

    protected ClientDto ClientDto;
    protected ClientDto ClientDto2;
    protected CreatingClientDto CreatingClientDto;
    protected CreatingClientDto CreatingClientDto2;

    public ClientGrpcControllerTestSetup()
    {
        ClientDto = new ClientDto("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "Sebastian Vettel",
            "Wald-Michelbacher Straße, 66", "Heppenheim", "5553-351", "DE");
        List<ClientDto> listDto = new List<ClientDto>();
        listDto.Add(ClientDto);

        ClientDto2 = new ClientDto("cccccccc-cccc-cccc-cccc-cccccccccccc", "Daniel Joseph Ricciardo",
            "Cliff Street, 77", "Perth", "4201-898", "AU");

        CreatingClientDto = new CreatingClientDto("Sebastian Vettel",
            "Wald-Michelbacher Straße, 66", "Heppenheim", "5553-351", "DE");
        
        CreatingClientDto2 = new CreatingClientDto("Daniel Joseph Ricciardo",
            "Cliff Street, 77", "Perth", "4201-898", "AU");

        var ClientService = new Mock<IClientsService>();
        ClientService.Setup(au => au.GetAllAsync().Result).Returns(listDto);
        ClientService.Setup(au => au.GetByIdAsync(new ClientId("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")).Result).Returns(ClientDto);
        ClientService.Setup(au => au.GetByIdAsync(new ClientId("ffffffff-ffff-ffff-ffff-ffffffffffff")).Result).Returns<ClientDto>(null);
        ClientService.Setup(au => au.AddAsync(It.Is<CreatingClientDto>(i => 
            i.Name == CreatingClientDto.Name)).Result).Returns(ClientDto);
        ClientService.Setup(au => au.AddAsync(It.Is<CreatingClientDto>(i => 
            i.Name == CreatingClientDto2.Name)).Result).Returns<ClientDto>(null);
        ClientService.Setup(au => au.UpdateAsync(It.Is<ClientDto>(i => 
            i.ClientId == ClientDto.ClientId)).Result).Returns<ClientDto>(null);
        ClientService.Setup(au => au.UpdateAsync(It.Is<ClientDto>(i => 
            i.ClientId == ClientDto2.ClientId)).Result).Returns(ClientDto2);
        ClientService.Setup(au => au.DeleteAsync(new ClientId("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")).Result).Returns<ClientDto>(null);
        ClientService.Setup(au => au.DeleteAsync(new ClientId("cccccccc-cccc-cccc-cccc-cccccccccccc")).Result).Returns(ClientDto2);

        var logger = new Mock<ILogger<ClientsGrpcController>>();
        
        Clc = new ClientsGrpcController(logger.Object, ClientService.Object);
    }

    public void Dispose()
    {
    }
}

public class ClientsGrpcControllerTest : ClientGrpcControllerTestSetup
{
    [Fact]
    public async void TestGetAllAsync()
    {
        var clt = new ClientGrpcDto{
           ClientId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", 
           Name = "Sebastian Vettel", 
           Street = "Wald-Michelbacher Straße, 66", 
           Local = "Heppenheim",
           PostalCode= "5553-351", 
           Country = "DE"
        };
        List<ClientGrpcDto> listCli = new List<ClientGrpcDto>();
        listCli.Add(clt);
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        var responseStream = new TestServerStreamWriter<ClientGrpcDto>(callContext);

        using var call = Clc.GetAllClients(new Empty(), responseStream, callContext);

        await call;
        responseStream.Complete();
        
        var allMessages = new List<ClientGrpcDto>();
        await foreach (var message in responseStream.ReadAllAsync())
        {
            allMessages.Add(message);
        }
        
        Assert.Equal(JsonConvert.SerializeObject(listCli), JsonConvert.SerializeObject(allMessages));
    }

    [Fact]
    public async void TestGetGetByIdValid()
    {
        var clt = new ClientGrpcDto{
            ClientId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", 
            Name = "Sebastian Vettel", 
            Street = "Wald-Michelbacher Straße, 66", 
            Local = "Heppenheim",
            PostalCode= "5553-351", 
            Country = "DE"
        };

        //GRPC setup
        var callContext = TestServerCallContext.Create();

        var response = await Clc.GetClientByID(new RequestWithClientId{Id = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"}, callContext);
        

        Assert.Equal(JsonConvert.SerializeObject(response), JsonConvert.SerializeObject(clt));
    }

    [Fact]
    public async Task TestGetGetByIdInValid()
    {
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>( () => Clc.GetClientByID(new RequestWithClientId{Id = "ffffffff-ffff-ffff-ffff-ffffffffffff"}, callContext));
        
    }
    
    [Fact]
    public async void TestCreate()
    {
        var clt = new ClientGrpcDto{
            ClientId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", 
            Name = "Sebastian Vettel", 
            Street = "Wald-Michelbacher Straße, 66", 
            Local = "Heppenheim",
            PostalCode= "5553-351", 
            Country = "DE"
        };

        //GRPC setup
        var callContext = TestServerCallContext.Create();
        
        var response = await Clc.AddNewClient(new CreatingClientGrpcDto
        {
            Name = CreatingClientDto.Name,
            Street = CreatingClientDto.Street,
            Local = CreatingClientDto.Local,
            PostalCode= CreatingClientDto.PostalCode,
            Country = CreatingClientDto.Country
        }, callContext);

        Assert.Equal(JsonConvert.SerializeObject(clt), JsonConvert.SerializeObject(response));
    }
    
    [Fact]
    public async Task TestCreateInvalid()
    {
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>( () => Clc.AddNewClient(new CreatingClientGrpcDto
        {
            Name = CreatingClientDto2.Name,
            Street = CreatingClientDto2.Street,
            Local = CreatingClientDto2.Local,
            PostalCode= CreatingClientDto2.PostalCode,
            Country = CreatingClientDto2.Country
        }, callContext));
        
    }


    [Fact]
    public async void TestUpdateNullDto()
    {
        var clt = new ClientGrpcDto{
            ClientId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", 
            Name = "Sebastian Vettel", 
            Street = "Wald-Michelbacher Straße, 66", 
            Local = "Heppenheim",
            PostalCode= "5553-351", 
            Country = "DE"
        };
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        
        await Assert.ThrowsAsync<RpcException>( () => Clc.ModifyClient(clt, callContext));
    }

    [Fact]
    public async void TestUpdateSuccess()
    {
        var clt = new ClientGrpcDto{
            ClientId = ClientDto2.ClientId, 
            Name = ClientDto2.Name, 
            Street = ClientDto2.Street, 
            Local = ClientDto2.Local, 
            PostalCode = ClientDto2.PostalCode,
            Country = ClientDto2.Country
        };
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        
        var adto = await Clc.ModifyClient(clt, callContext);
        
        Assert.Equal(JsonConvert.SerializeObject(adto), JsonConvert.SerializeObject(clt));
    }

    [Fact]
    public async void TestDeleteNullDto()
    {
        //GRPC setup
        var callContext = TestServerCallContext.Create();

        await Assert.ThrowsAsync<RpcException>( () => Clc.DeleteClient(new RequestWithClientId{Id ="aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"}, callContext));
    }

    [Fact]
    public async void TestDeleteSuccess()
    {
        var clt = new ClientGrpcDto{
            ClientId = ClientDto2.ClientId, 
            Name = ClientDto2.Name, 
            Street = ClientDto2.Street, 
            Local = ClientDto2.Local, 
            PostalCode = ClientDto2.PostalCode,
            Country = ClientDto2.Country
        };
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();

        var adto = await Clc.DeleteClient(new RequestWithClientId { Id = "cccccccc-cccc-cccc-cccc-cccccccccccc" }, callContext);
        
        Assert.Equal(JsonConvert.SerializeObject(clt), JsonConvert.SerializeObject(adto));
    }
    
}