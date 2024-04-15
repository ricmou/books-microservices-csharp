using APICategories.Domain.Categories;
using APICategories.Domain.Shared;
using APICategoriesTest.Helpers;

namespace APICategoriesTest.Domain.Categories;

public class CategoryTest
{
    [Fact]
    public void CheckCategoryNotAcceptNullId()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Category(null, "Category"));
    }
    [Fact]
    public void CheckCategoryNotAcceptNullName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Category("CAT", null));
    }
        
    [Fact]
    public void CheckCategoryNotAcceptEmptyName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Category("CAT", ""));
    }
    
    [Fact]
    public void CheckCategoryNotAcceptOverflowName()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Category("CAT", Util.RandomString(51)));
    }
}