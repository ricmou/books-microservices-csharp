using APIExemplar.Domain.Exemplars;

namespace APIExemplarTest.Domain.Exemplars;

public class ExemplarIdTest
{
    [Fact]
    public void CheckExemplarIdNotAcceptNull()
    {
        Assert.Throws<NullReferenceException>(() => new ExemplarId(null));
    }

    [Fact]
    public void CheckExemplarIdNotAcceptEmpty()
    {
        Assert.Throws<FormatException>(() => new ExemplarId(""));
    }
    
    [Fact]
    public void CheckExemplarIdNotAcceptInvalid()
    {
        Assert.Throws<FormatException>(() => new ExemplarId("asd"));
    }
}