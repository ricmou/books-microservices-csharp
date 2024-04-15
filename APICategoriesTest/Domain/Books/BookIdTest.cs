using APICategories.Domain.Books;
using APICategories.Domain.Shared;

namespace APICategoriesTest.Domain.Books;

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