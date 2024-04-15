using APIAuthors.Domain.Books;
using APIAuthors.Domain.Shared;

namespace APIAuthorsTest.Domain.Books;

public class BookIdTest
{
    [Fact]
    public void CheckBookIdNotAcceptNull()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new BookId(null));
    }

    [Fact]
    public void CheckBookIdNotAcceptEmpty()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new BookId(""));
    }

    [Fact]
    public void CheckBookIdNotAcceptInvalid()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new BookId("975-0123456789"));
    }
}