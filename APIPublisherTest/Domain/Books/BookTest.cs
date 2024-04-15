using APIPublisher.Domain.Books;
using APIPublisher.Domain.Publishers;
using APIPublisher.Domain.Shared;

namespace APIPublisherTest.Domain.Books;

public class BookTest
{
    [Fact]
    public void CheckBookNotAcceptNullId()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Book(null, new Publisher("RE1", "Name",  "UA")));
    }

    [Fact]
    public void CheckBookNotAcceptEmptyId()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Book("", new Publisher("RE1", "Name",  "UA")));
    }

    [Fact]
    public void CheckBookNotAcceptInvalidId()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Book("975-0123456789", new Publisher("RE1", "Name",  "UA")));
    }
    
    [Fact]
    public void CheckBookNotAcceptNullPublisher()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Book("978-0123456789", null));
    }
}