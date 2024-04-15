using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Shared;
using APIAuthorsTest.Helpers;

namespace APIAuthorsTest.Domain.Authors;

public class AuthorTest
{
    [Fact]
    public void CheckAuthorNotAcceptNullId()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Author(null, "FirstName", "LastName", new DateOnly(1999,1,1), "DE"));
    }
        
    [Fact]
    public void CheckAuthorNotAcceptEmptyId()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Author("", "FirstName", "LastName", new DateOnly(1999,1,1), "DE"));
    }
        
    [Fact]
    public void CheckAuthorNotAcceptInvalidId()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Author("AY!", "FirstName", "LastName", new DateOnly(1999,1,1), "DE"));
    }
    
    [Fact]
    public void CheckAuthorNotAcceptNullFirstName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Author("TE1", null, "LastName", new DateOnly(1999,1,1), "DE"));
    }
        
    [Fact]
    public void CheckAuthorNotAcceptEmptyFirstName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Author("TE1", "", "LastName", new DateOnly(1999,1,1), "DE"));
    }
        
    [Fact]
    public void CheckAuthorNotAcceptInvalidFirstName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Author("TE1", Util.RandomString(129), "LastName", new DateOnly(1999,1,1), "DE"));
    }
    
    [Fact]
    public void CheckAuthorNotAcceptNullLastName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Author("TE1", "FirstName", null, new DateOnly(1999,1,1), "DE"));
    }
        
    [Fact]
    public void CheckAuthorNotAcceptEmptyLastName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Author("TE1", "FirstName", "", new DateOnly(1999,1,1), "DE"));
    }
        
    [Fact]
    public void CheckAuthorNotAcceptInvalidLastName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Author("TE1", "FirstName", Util.RandomString(129), new DateOnly(1999,1,1), "DE"));
    }
    
    [Fact]
    public void CheckAuthorNotAcceptNullCountry()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Author("TE1", "FirstName", "LastName", new DateOnly(1999,1,1), null));
    }
        
    [Fact]
    public void CheckAuthorNotAcceptEmptyCountry()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Author("TE1", "FirstName", "LastName", new DateOnly(1999,1,1), ""));
    }
        
    [Fact]
    public void CheckAuthorNotAcceptInvalidCountry()
    {
        Assert.Throws<ArgumentException>(() => new Author("TE1", "FirstName", "LastName", new DateOnly(1999,1,1), "ASDSADASD"));
    }
}