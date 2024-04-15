using APIExemplar.Domain.Exemplars;
using APIExemplar.Domain.Shared;

namespace APIExemplarTest.Domain.Exemplars;

public class ExemplarStateTest
{
    public void CheckExemplarStateNotAcceptNull()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new ExemplarState(null));
    }

    [Fact]
    public void CheckExemplarStateNotAcceptEmpty()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new ExemplarState(""));
    }

    [Fact]
    public void CheckExemplarStateNotAcceptInvalidString()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new ExemplarState("Text"));
    }
    
    [Fact]
    public void CheckExemplarStateNotAcceptInvalidInteger()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new ExemplarState(5));
    }
    
    [Fact]
    public void CheckExemplarStateStringToInt()
    {
        var state = new ExemplarState("Good");
        Assert.Equal(3, state.State);
    }
}