using APIClients.Domain.Clients;
using APIClients.Domain.Shared;
using APIClients.Services;
using Moq;
using Newtonsoft.Json;

namespace APIClientsTest.Services;

public abstract class ClientServiceTestSetup : IDisposable
{
    public ClientsService Cts;

    public ClientServiceTestSetup()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(unit => unit.CommitAsync().Result).Returns(0);

        var client = new Client("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "Sebastian Vettel",
            new Address("Wald-Michelbacher Straße, 66", "Heppenheim", "5553-351", "DE"));
        var client2 =
            new Client("cccccccccccccccccccccccccccccccc", "Daniel Joseph Ricciardo",
                new Address("Cliff Street, 77", "Perth", "4201-898", "AU"));
        List<Client> list = new List<Client>();
        list.Add(client);

        var repo = new Mock<IClientsRepository>();
        repo.Setup(rep => rep.GetAllAsync().Result).Returns(list);
        repo.Setup(rep => rep.GetByIdAsync(new ClientId("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")).Result).Returns(client);
        repo.Setup(rep => rep.GetByIdAsync(new ClientId("cccccccccccccccccccccccccccccccc")).Result)
            .Returns<Client>(null);
        repo.Setup(rep => rep.AddAsync(It.Is<Client>(c => c.Name == client.Name)).Result).Returns(client);
        repo.Setup(rep => rep.AddAsync(It.IsAny<Client>()).Result).Returns<Client>(null);

        Cts = new ClientsService(unitOfWork.Object, repo.Object);
    }

    public void Dispose()
    {
    }
}

public class ClientServiceTest : ClientServiceTestSetup
{
    [Fact]
    public void TestGetByIdAsyncValid()
    {
        var clt = new ClientDto("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "Sebastian Vettel",
            "Wald-Michelbacher Straße, 66", "Heppenheim", "5553-351", "DE");
        var cdto = Cts.GetByIdAsync(new ClientId("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"));
        Assert.Equal(JsonConvert.SerializeObject(clt), JsonConvert.SerializeObject(cdto.Result));
    }

    [Fact]
    public void TestGetByIdAsyncInvalid()
    {
        var cdto = Cts.GetByIdAsync(new ClientId("cccccccccccccccccccccccccccccccc"));
        Assert.Null(cdto.Result);
    }

    [Fact]
    public void TestGetAllAsync()
    {
        var clt = new ClientDto("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "Sebastian Vettel",
            "Wald-Michelbacher Straße, 66", "Heppenheim", "5553-351", "DE");
        var cdto = Cts.GetAllAsync();
        Assert.Equal(JsonConvert.SerializeObject(clt), JsonConvert.SerializeObject(cdto.Result[0]));
    }

    [Fact]
    public void TestDeleteAsyncValid()
    {
        var clt = new ClientDto("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "Sebastian Vettel",
            "Wald-Michelbacher Straße, 66", "Heppenheim", "5553-351", "DE");
        var cdto = Cts.DeleteAsync(new ClientId("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"));
        Assert.Equal(JsonConvert.SerializeObject(clt), JsonConvert.SerializeObject(cdto.Result));
    }

    [Fact]
    public void TestDeleteAsyncInvalid()
    {
        var cdto = Cts.DeleteAsync(new ClientId("cccccccccccccccccccccccccccccccc"));
        Assert.Null(cdto.Result);
    }

    [Fact]
    public void TestUpdateAsyncValid()
    {
        var clt = new ClientDto("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "Sebastian Vet", "Wald-Michelbacher Straße, 68",
            "Heppenhei", "5553-352", "CA");
        var cdto = Cts.UpdateAsync(new ClientDto("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "Sebastian Vet",
            "Wald-Michelbacher Straße, 68", "Heppenhei", "5553-352", "CA"));
        Assert.Equal(JsonConvert.SerializeObject(clt), JsonConvert.SerializeObject(cdto.Result));
    }

    [Fact]
    public void TestUpdateAsyncInvalid()
    {
        var cdto = Cts.UpdateAsync(new ClientDto("cccccccccccccccccccccccccccccccc", "Sebastian Vettel",
            "Wald-Michelbacher Straße, 66", "Heppenheim", "5553-351", "DE"));
        Assert.Null(cdto.Result);
    }

    [Fact]
    public void TestAddAsyncValid()
    {
        var createclt = Cts.AddAsync(new CreatingClientDto("Sebastian Vettel", "Wald-Michelbacher Straße, 66",
            "Heppenheim", "5553-351", "DE"));
        var resclt = new ClientDto("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "Sebastian Vettel",
            "Wald-Michelbacher Straße, 66", "Heppenheim", "5553-351", "DE");
        //Having tags match would require some very nasty hackery or more time
        Assert.Equal(resclt.Name, createclt.Result.Name);
        Assert.Equal(resclt.Street, createclt.Result.Street);
        Assert.Equal(resclt.Local, createclt.Result.Local);
        Assert.Equal(resclt.PostalCode, createclt.Result.PostalCode);
        Assert.Equal(resclt.Country, createclt.Result.Country);
    }
}