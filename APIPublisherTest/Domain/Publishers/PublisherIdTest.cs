using APIPublisher.Domain.Publishers;
using APIPublisher.Domain.Shared;

namespace APIPublisherTest.Domain.Publishers;

public class PublisherIdTest
{
    [Fact]
    public void CheckPublisherIdNotAcceptNull()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new PublisherId(null));
    }
        
    [Fact]
    public void CheckPublisherIdNotAcceptEmpty()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new PublisherId(""));
    }
        
    [Fact]
    public void CheckPublisherIdNotAcceptInvalid()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new PublisherId("AY!"));
    }
}