using APICategories.Domain.Books;
using APICategories.Domain.Shared;

namespace APICategoriesTest.Domain.Books;

public class BookTest
{
    [Fact]
    public void CheckBookNotAcceptNullId()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Book(null));
    }

    [Fact]
    public void CheckBookNotAcceptEmptyId()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Book(""));
    }

    [Fact]
    public void CheckBookNotAcceptInvalidId()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Book("975-0123456789"));
    }
    
    [Fact]
    public void CheckBookNotAcceptInvalidAuthor()
    {
        var book = new Book("978-0123456789");
        Assert.Throws<BusinessRuleValidationException>(() => book.AddCategory(null));
    }
}