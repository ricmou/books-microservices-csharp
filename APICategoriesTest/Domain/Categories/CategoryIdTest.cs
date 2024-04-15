using APICategories.Domain.Categories;
using APICategories.Domain.Shared;

namespace APICategoriesTest.Domain.Categories;

public class CategoryIdTest
{
    [Fact]
    public void CheckCategoryIdNotAcceptNull()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new CategoryId(null));
    }
        
    [Fact]
    public void CheckCategoryIdNotAcceptEmpty()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new CategoryId(""));
    }
        
    [Fact]
    public void CheckCategoryIdNotAcceptInvalid()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new CategoryId("AY!"));
    }
}