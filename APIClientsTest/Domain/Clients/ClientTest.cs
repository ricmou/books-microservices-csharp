using APIClients.Domain.Clients;
using APIClients.Domain.Shared;
using APIClientsTest.Helpers;

namespace APIClientsTest.Domain.Clients;

public class ClientTest
{
    [Fact]
    public void CheckClientNotAcceptNullAddress()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Client("Name", null));
    }
    
    [Fact]
    public void CheckClientNotAcceptNullName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Client(null, new Address(null, "Local", "1111-111", "Country")));
    }
        
    [Fact]
    public void CheckClientNotAcceptEmptyName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Client("", new Address(null, "Local", "1111-111", "Country")));
    }
    
    [Fact]
    public void CheckClientNotAcceptOverflowName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Client(Util.RandomString(151), new Address(null, "Local", "1111-111", "Country")));
    }
}