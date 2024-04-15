using APIClients.Domain.Clients;

namespace APIClientsTest.Domain.Clients;

public class ClientIdTest
{
    [Fact]
    public void CheckClientIdNotAcceptNull()
    {
        Assert.Throws<NullReferenceException>(() => new ClientId(null));
    }

    [Fact]
    public void CheckClientIdNotAcceptEmpty()
    {
        Assert.Throws<FormatException>(() => new ClientId(""));
    }
    
    [Fact]
    public void CheckClientIdNotAcceptInvalid()
    {
        Assert.Throws<FormatException>(() => new ClientId("asd"));
    }
}