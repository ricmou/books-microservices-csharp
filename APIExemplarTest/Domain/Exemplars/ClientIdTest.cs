using APIExemplar.Domain.Exemplars;

namespace APIExemplarTest.Domain.Exemplars;

public class ClientIdTest
{
    [Fact]
    public void CheckClientIdNotAcceptNull()
    {
        Assert.Throws<NullReferenceException>(() => new ClientId(null));
    }

    [Fact]
    public void CheckClientIdNotAcceptEmpty()
    {
        Assert.Throws<FormatException>(() => new ClientId(""));
    }
    
    [Fact]
    public void CheckClientIdNotAcceptInvalid()
    {
        Assert.Throws<FormatException>(() => new ClientId("asd"));
    }
}