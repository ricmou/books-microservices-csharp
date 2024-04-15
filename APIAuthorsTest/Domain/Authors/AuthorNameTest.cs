using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Shared;
using APIAuthorsTest.Helpers;

namespace APIAuthorsTest.Domain.Authors;

public class AuthorNameTest
{
    [Fact]
    public void CheckAuthorNameNotAcceptNullFirstName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new AuthorName(null, "LastName"));
    }
        
    [Fact]
    public void CheckAuthorNameNotAcceptEmptyFirstName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new AuthorName("", "LastName"));
    }
    
    [Fact]
    public void CheckAuthorNameNotAcceptOverflowFirstName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new AuthorName(Util.RandomString(129), "LastName"));
    }
        
    [Fact]
    public void CheckAddressNotAcceptNullLastName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new AuthorName("FirstName", null));
    }
        
    [Fact]
    public void CheckAddressNotAcceptEmptyLastName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new AuthorName("FirstName", ""));
    }
    
    [Fact]
    public void CheckAuthorNameNotAcceptOverflowLastName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new AuthorName("", Util.RandomString(129)));
    }
}