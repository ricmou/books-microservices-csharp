using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Shared;

namespace APIAuthorsTest.Domain.Authors;

public class AuthorIdTest
{
    [Fact]
    public void CheckAuthorIdNotAcceptNull()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new AuthorId(null));
    }
        
    [Fact]
    public void CheckAuthorIdNotAcceptEmpty()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new AuthorId(""));
    }
        
    [Fact]
    public void CheckAuthorIdNotAcceptInvalid()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new AuthorId("AY!"));
    }
}