using APIExemplar.Domain.Exemplars;
using APIExemplar.Domain.Shared;
using APIExemplar.Services;
using Moq;
using Newtonsoft.Json;

namespace APIExemplarTest.Services;

public abstract class ExemplarServiceTestSetup : IDisposable
{
    public ExemplarService Exc;

    public ExemplarServiceTestSetup()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(unit => unit.CommitAsync().Result).Returns(0);

        var exemplar = new Exemplar("11111111111111111111111111111111", new BookId("978-1491900864"),
            new ExemplarState(3), new ClientId("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"), new DateOnly(2017, 1, 5));
        var exemplar2 = new Exemplar("22222222222222222222222222222222", new BookId("978-1617292545"),
            new ExemplarState(1), new ClientId("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"), new DateOnly(2018, 1, 5));
        List<Exemplar> list = new List<Exemplar>();
        list.Add(exemplar);

        var repo = new Mock<IExemplarRepository>();
        repo.Setup(rep => rep.GetAllAsync().Result).Returns(list);
        repo.Setup(rep => rep.GetByIdAsync(new ExemplarId("11111111111111111111111111111111")).Result).Returns(exemplar);
        repo.Setup(rep => rep.GetByIdAsync(new ExemplarId("22222222222222222222222222222222")).Result)
            .Returns<Exemplar>(null);
        repo.Setup(rep => rep.AddAsync(It.Is<Exemplar>(c => c.Book == exemplar.Book && c.SellerId == exemplar.SellerId)).Result).Returns(exemplar);
        repo.Setup(rep => rep.AddAsync(It.IsAny<Exemplar>()).Result).Returns<Exemplar>(null);
        repo.Setup(rep => rep.GetExemplarsOfId(new BookId("978-1491900864")).Result).Returns(list);
        repo.Setup(rep => rep.GetExemplarsOfId(new BookId("978-1617292545")).Result).Returns<List<Exemplar>>(null);
        repo.Setup(rep => rep.GetExemplarsOfClient(new ClientId("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb")).Result).Returns(list);
        repo.Setup(rep => rep.GetExemplarsOfClient(new ClientId("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")).Result).Returns<List<Exemplar>>(null);

        Exc = new ExemplarService(unitOfWork.Object, repo.Object);
    }

    public void Dispose()
    {
    }
}

public class ExemplarServiceTest : ExemplarServiceTestSetup
{
    [Fact]
    public void TestGetByIdAsyncValid()
    {
        var exp = new ExemplarDto("11111111-1111-1111-1111-111111111111", "978-1491900864",
            3, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "05/01/2017");
        var edto = Exc.GetByIdAsync(new ExemplarId("11111111111111111111111111111111"));
        Assert.Equal(JsonConvert.SerializeObject(exp), JsonConvert.SerializeObject(edto.Result));
    }

    [Fact]
    public void TestGetByIdAsyncInvalid()
    {
        var edto = Exc.GetByIdAsync(new ExemplarId("22222222222222222222222222222222"));
        Assert.Null(edto.Result);
    }

    [Fact]
    public void TestGetAllAsync()
    {
        var exp = new ExemplarDto("11111111-1111-1111-1111-111111111111", "978-1491900864",
            3, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "05/01/2017");
        var edto = Exc.GetAllAsync();
        Assert.Equal(JsonConvert.SerializeObject(exp), JsonConvert.SerializeObject(edto.Result[0]));
    }

    [Fact]
    public void TestDeleteAsyncValid()
    {
        var exp = new ExemplarDto("11111111-1111-1111-1111-111111111111", "978-1491900864",
            3, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "05/01/2017");
        var edto = Exc.DeleteAsync(new ExemplarId("11111111111111111111111111111111"));
        Assert.Equal(JsonConvert.SerializeObject(exp), JsonConvert.SerializeObject(edto.Result));
    }

    [Fact]
    public void TestDeleteAsyncInvalid()
    {
        var edto = Exc.DeleteAsync(new ExemplarId("22222222222222222222222222222222"));
        Assert.Null(edto.Result);
    }

    [Fact]
    public void TestUpdateAsyncValid()
    {
        var exp = new ExemplarDto("11111111-1111-1111-1111-111111111111", "978-2491900864",
            2, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "04/01/2017");
        var edto = Exc.UpdateAsync(new ExemplarDto("11111111-1111-1111-1111-111111111111", "978-2491900864",
            2, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "04/01/2017"));
        Assert.Equal(JsonConvert.SerializeObject(exp), JsonConvert.SerializeObject(edto.Result));
    }

    [Fact]
    public void TestUpdateAsyncInvalid()
    {
        var edto = Exc.UpdateAsync(new ExemplarDto("22222222222222222222222222222222", "978-1617292545",
            1, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "05/01/2018"));
        Assert.Null(edto.Result);
    }

    [Fact]
    public void TestAddAsyncValid()
    {
        var createexp = Exc.AddAsync(new CreatingExemplarDto("978-1491900864",
            3, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "04/01/2017"));
        var resexp = new ExemplarDto("11111111-1111-1111-1111-111111111111", "978-1491900864",
            3, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "04/01/2017");
        //Having tags match would require some very nasty hackery or more time
        Assert.Equal(resexp.BookId, createexp.Result.BookId);
        Assert.Equal(resexp.BookState, createexp.Result.BookState);
        Assert.Equal(resexp.SellerId, createexp.Result.SellerId);
        Assert.Equal(resexp.DateOfAcquisition, createexp.Result.DateOfAcquisition);
    }
    
    [Fact]
    public void TestGetAllOfIdAsync()
    {
        var exp = new ExemplarDto("11111111-1111-1111-1111-111111111111", "978-1491900864",
            3, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "05/01/2017");
        var edto = Exc.GetByBookIdAsync(new BookId("978-1491900864"));
        Assert.Equal(JsonConvert.SerializeObject(exp),JsonConvert.SerializeObject(edto.Result[0]));
    }
        
    [Fact]
    public async void TestGetAllOfIdInvalid()
    {
        await Assert.ThrowsAsync<NullReferenceException>(() => Exc.GetByBookIdAsync(new BookId("978-1617292545")));
    }
    
    [Fact]
    public void TestGetAllOfClientAsync()
    {
        var exp = new ExemplarDto("11111111-1111-1111-1111-111111111111", "978-1491900864",
            3, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "05/01/2017");
        var edto = Exc.GetBySellerIdAsync(new ClientId("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"));
        Assert.Equal(JsonConvert.SerializeObject(exp),JsonConvert.SerializeObject(edto.Result[0]));
    }
        
    [Fact]
    public async void TestGetAllOfClientInvalid()
    {
        await Assert.ThrowsAsync<NullReferenceException>(() => Exc.GetBySellerIdAsync(new ClientId("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")));
    }
}