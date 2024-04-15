using APIExemplar.Domain.Exemplars;
using APIExemplar.Domain.Shared;

namespace APIExemplarTest.Domain.Exemplars;

public class ExemplarTest
{
    [Fact]
    public void CheckExemplarNotAcceptNullExemplarId()
    {
        Assert.Throws<NullReferenceException>(() => new Exemplar(null, new BookId("978-1491900864"), new ExemplarState(3), new ClientId("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"), new DateOnly(2017,1,5)));
    }

    [Fact]
    public void CheckExemplarNotAcceptNullBookIdInMem()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Exemplar("11111111111111111111111111111111", null, new ExemplarState(3), new ClientId("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"), new DateOnly(2017,1,5)));
    }
    
    [Fact]
    public void CheckExemplarNotAcceptNullBookId()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Exemplar(null, new ExemplarState(3), new ClientId("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"), new DateOnly(2017,1,5)));
    }
    
    [Fact]
    public void CheckExemplarNotAcceptNullExemplarStateInMem()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Exemplar("11111111111111111111111111111111", new BookId("978-1491900864"), null, new ClientId("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"), new DateOnly(2017,1,5)));
    }
    
    [Fact]
    public void CheckExemplarNotAcceptNullExemplarState()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Exemplar(new BookId("978-1491900864"), null, new ClientId("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"), new DateOnly(2017,1,5)));
    }
    
    [Fact]
    public void CheckExemplarNotAcceptNullSellerIdInMem()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Exemplar("11111111111111111111111111111111", new BookId("978-1491900864"), new ExemplarState(3), null, new DateOnly(2017,1,5)));
    }
    
    [Fact]
    public void CheckExemplarNotAcceptNullSellerId()
    {
        Assert.Throws<BusinessRuleValidationException>(() => new Exemplar(new BookId("978-1491900864"), new ExemplarState(3), null, new DateOnly(2017,1,5)));
    }
}