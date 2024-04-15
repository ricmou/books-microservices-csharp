using APIPublisher.Domain.Publishers;
using APIPublisher.Domain.Shared;
using APIPublisherTest.Helpers;

namespace APIPublisherTest.Domain.Publishers;

public class PublisherTest
{
    [Fact]
    public void CheckPublisherNotAcceptNullId()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Publisher(null, "Name", "UA"));
    }
        
    [Fact]
    public void CheckPublisherNotAcceptEmptyId()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Publisher("", "Name", "UA"));
    }
        
    [Fact]
    public void CheckPublisherNotAcceptInvalidId()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Publisher("AY!", "Name", "UA"));
    }
    
    [Fact]
    public void CheckPublisherNotAcceptNullName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Publisher("RE1", null, "UA"));
    }
        
    [Fact]
    public void CheckPublisherNotAcceptEmptyName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Publisher("RE1", "", "UA"));
    }
        
    [Fact]
    public void CheckPublisherNotAcceptInvalidName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Publisher("RE1", Util.RandomString(129), "UA"));
    }
}