using APIExemplar;
using APIExemplar.Controllers;
using APIExemplar.Domain.Exemplars;
using APIExemplar.Services;
using APIExemplarTest.Helpers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace APIExemplarTest.Controllers;

public abstract class ExemplarGrpcControllerTestSetup : IDisposable
{
    protected ExemplarGrpcController Exc;

    protected ExemplarDto ExemplarDto;
    protected ExemplarDto ExemplarDto2;
    protected CreatingExemplarDto CreatingExemplarDto;
    protected CreatingExemplarDto CreatingExemplarDto2;

    public ExemplarGrpcControllerTestSetup()
    {
        ExemplarDto = new ExemplarDto("11111111-1111-1111-1111-111111111111", "978-1491900864",
            3, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "05/01/2017");
        List<ExemplarDto> listDto = new List<ExemplarDto>();
        listDto.Add(ExemplarDto);

        ExemplarDto2 = new ExemplarDto("22222222-2222-2222-2222-222222222222", "978-1617292545",
            1, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "05/01/2018");

        CreatingExemplarDto = new CreatingExemplarDto("978-1491900864",
            3, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "05/01/2017");
        
        CreatingExemplarDto2 = new CreatingExemplarDto("978-1617292545",
            1, "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "05/01/2018");

        var ExemplarService = new Mock<IExemplarService>();
        ExemplarService.Setup(ex => ex.GetAllAsync().Result).Returns(listDto);
        ExemplarService.Setup(ex => ex.GetByIdAsync(new ExemplarId("11111111-1111-1111-1111-111111111111")).Result).Returns(ExemplarDto);
        ExemplarService.Setup(ex => ex.GetByIdAsync(new ExemplarId("99999999-9999-9999-9999-999999999999")).Result).Returns<ExemplarDto>(null);
        ExemplarService.Setup(ex => ex.AddAsync(It.Is<CreatingExemplarDto>(i => 
            i.BookId == CreatingExemplarDto.BookId && i.SellerId == CreatingExemplarDto.SellerId)).Result).Returns(ExemplarDto);
        ExemplarService.Setup(ex => ex.AddAsync(It.Is<CreatingExemplarDto>(i => 
            i.BookId == CreatingExemplarDto2.BookId && i.SellerId == CreatingExemplarDto2.SellerId)).Result).Returns<ExemplarDto>(null);
        ExemplarService.Setup(ex => ex.UpdateAsync(It.Is<ExemplarDto>(i => 
            i.ExemplarId == ExemplarDto.ExemplarId)).Result).Returns<ExemplarDto>(null);
        ExemplarService.Setup(ex => ex.UpdateAsync(It.Is<ExemplarDto>(i => 
            i.ExemplarId == ExemplarDto2.ExemplarId)).Result).Returns(ExemplarDto2);
        ExemplarService.Setup(ex => ex.DeleteAsync(new ExemplarId("11111111-1111-1111-1111-111111111111")).Result).Returns<ExemplarDto>(null);
        ExemplarService.Setup(ex => ex.DeleteAsync(new ExemplarId("22222222-2222-2222-2222-222222222222")).Result).Returns(ExemplarDto2);
        ExemplarService.Setup(ex => ex.GetByBookIdAsync(new BookId("978-1491900864")).Result).Returns(listDto);
        ExemplarService.Setup(ex => ex.GetByBookIdAsync(new BookId("978-9999999999")).Result).Returns<List<ExemplarDto>>(null);
        ExemplarService.Setup(ex => ex.GetBySellerIdAsync(new ClientId("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb")).Result).Returns(listDto);
        ExemplarService.Setup(ex => ex.GetBySellerIdAsync(new ClientId("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")).Result).Returns<List<ExemplarDto>>(null);


        var logger = new Mock<ILogger<ExemplarGrpcController>>();
        
        Exc = new (logger.Object, ExemplarService.Object);
    }

    public void Dispose()
    {
    }
}

public class ExemplarGrpcControllerTest : ExemplarGrpcControllerTestSetup
{
    [Fact]
    public async void TestGetAllAsync()
    {
        var exp = new ExemplarGrpcDto{
            ExemplarId = ExemplarDto.ExemplarId, 
            BookId = ExemplarDto.BookId, 
            BookState = ExemplarDto.BookState, 
            SellerId = ExemplarDto.SellerId,
            DateOfAcquisition= ExemplarDto.DateOfAcquisition,
        };
        List<ExemplarGrpcDto> lstCli = new List<ExemplarGrpcDto>();
        lstCli.Add(exp);
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        var responseStream = new TestServerStreamWriter<ExemplarGrpcDto>(callContext);

        using var call = Exc.GetAllExemplars(new Empty(), responseStream, callContext);

        await call;
        responseStream.Complete();
        
        var allMessages = new List<ExemplarGrpcDto>();
        await foreach (var message in responseStream.ReadAllAsync())
        {
            allMessages.Add(message);
        }
        
        Assert.Equal(JsonConvert.SerializeObject(lstCli), JsonConvert.SerializeObject(allMessages));
    }

    [Fact]
    public async void TestGetGetByIdValid()
    {
        var exp = new ExemplarGrpcDto{
            ExemplarId = ExemplarDto.ExemplarId, 
            BookId = ExemplarDto.BookId, 
            BookState = ExemplarDto.BookState, 
            SellerId = ExemplarDto.SellerId,
            DateOfAcquisition= ExemplarDto.DateOfAcquisition,
        };

        //GRPC setup
        var callContext = TestServerCallContext.Create();

        var response = await Exc.GetExemplarByID(new RequestWithExemplarId{Id = "11111111-1111-1111-1111-111111111111"}, callContext);
        

        Assert.Equal(JsonConvert.SerializeObject(response), JsonConvert.SerializeObject(exp));
    }

    [Fact]
    public async Task TestGetGetByIdInValid()
    {
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>( () => Exc.GetExemplarByID(new RequestWithExemplarId{Id = "99999999-9999-9999-9999-999999999999"}, callContext));
        
    }
    
    [Fact]
    public async void TestCreate()
    {
        var exp = new ExemplarGrpcDto{
            ExemplarId = ExemplarDto.ExemplarId, 
            BookId = ExemplarDto.BookId, 
            BookState = ExemplarDto.BookState, 
            SellerId = ExemplarDto.SellerId,
            DateOfAcquisition= ExemplarDto.DateOfAcquisition,
        };

        //GRPC setup
        var callContext = TestServerCallContext.Create();
        
        var response = await Exc.AddNewExemplar(new CreatingExemplarGrpcDto
        {
            BookId = CreatingExemplarDto.BookId,
            BookState = CreatingExemplarDto.BookState,
            SellerId = CreatingExemplarDto.SellerId,
            DateOfAcquisition = CreatingExemplarDto.DateOfAcquisition
        }, callContext);

        Assert.Equal(JsonConvert.SerializeObject(exp), JsonConvert.SerializeObject(response));
    }
    
    [Fact]
    public async Task TestCreateInvalid()
    {
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>( () => Exc.AddNewExemplar(new CreatingExemplarGrpcDto
        {
            BookId = CreatingExemplarDto2.BookId,
            BookState = CreatingExemplarDto2.BookState,
            SellerId = CreatingExemplarDto2.SellerId,
            DateOfAcquisition= CreatingExemplarDto2.DateOfAcquisition
        }, callContext));
        
    }


    [Fact]
    public async void TestUpdateNullDto()
    {
        var exp = new ExemplarGrpcDto{
            ExemplarId = ExemplarDto.ExemplarId, 
            BookId = ExemplarDto.BookId, 
            BookState = ExemplarDto.BookState, 
            SellerId = ExemplarDto.SellerId,
            DateOfAcquisition = ExemplarDto.DateOfAcquisition,
        };
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        
        await Assert.ThrowsAsync<RpcException>( () => Exc.ModifyExemplar(exp, callContext));
    }

    [Fact]
    public async void TestUpdateSuccess()
    {
        var exp = new ExemplarGrpcDto{
            ExemplarId = ExemplarDto2.ExemplarId, 
            BookId = ExemplarDto2.BookId, 
            BookState = ExemplarDto2.BookState, 
            SellerId = ExemplarDto2.SellerId, 
            DateOfAcquisition = ExemplarDto2.DateOfAcquisition
        };
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        
        var edto = await Exc.ModifyExemplar(exp, callContext);
        
        Assert.Equal(JsonConvert.SerializeObject(edto), JsonConvert.SerializeObject(exp));
    }

    [Fact]
    public async void TestDeleteNullDto()
    {
        //GRPC setup
        var callContext = TestServerCallContext.Create();

        await Assert.ThrowsAsync<RpcException>( () => Exc.DeleteExemplar(new RequestWithExemplarId{Id ="11111111-1111-1111-1111-111111111111"}, callContext));
    }

    [Fact]
    public async void TestDeleteSuccess()
    {
        var exp = new ExemplarGrpcDto{
            ExemplarId = ExemplarDto2.ExemplarId, 
            BookId = ExemplarDto2.BookId, 
            BookState = ExemplarDto2.BookState, 
            SellerId = ExemplarDto2.SellerId, 
            DateOfAcquisition = ExemplarDto2.DateOfAcquisition
        };
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();

        var edto = await Exc.DeleteExemplar(new RequestWithExemplarId { Id = "22222222-2222-2222-2222-222222222222" }, callContext);
        
        Assert.Equal(JsonConvert.SerializeObject(exp), JsonConvert.SerializeObject(edto));
    }
    
    [Fact]
    public async void TestGetByBookSuccess()
    {
        var lstExp = new List<ExemplarGrpcDto>();
        lstExp.Add(new ExemplarGrpcDto
        {
            ExemplarId = ExemplarDto.ExemplarId, 
            BookId = ExemplarDto.BookId, 
            BookState = ExemplarDto.BookState, 
            SellerId = ExemplarDto.SellerId,
            DateOfAcquisition= ExemplarDto.DateOfAcquisition,
        });
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        var responseStream = new TestServerStreamWriter<ExemplarGrpcDto>(callContext);

        using var call = Exc.GetAllExemplarsFromBook(new RequestWithISBN{Id ="978-1491900864"}, responseStream, callContext);

        await call;
        responseStream.Complete();
        
        var allMessages = new List<ExemplarGrpcDto>();
        await foreach (var message in responseStream.ReadAllAsync())
        {
            allMessages.Add(message);
        }
        Assert.Equal(JsonConvert.SerializeObject(lstExp), JsonConvert.SerializeObject(allMessages));
    }

    [Fact]
    public async void TestGetByBookFail()
    {
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        var responseStream = new TestServerStreamWriter<ExemplarGrpcDto>(callContext);

        await Assert.ThrowsAsync<RpcException>(() =>Exc.GetAllExemplarsFromBook(new RequestWithISBN{Id ="978-9999999999"}, responseStream, callContext));
        
    }
    
    [Fact]
    public async void TestGetBySellerSuccess()
    {
        var lstExp = new List<ExemplarGrpcDto>();
        lstExp.Add(new ExemplarGrpcDto
        {
            ExemplarId = ExemplarDto.ExemplarId, 
            BookId = ExemplarDto.BookId, 
            BookState = ExemplarDto.BookState, 
            SellerId = ExemplarDto.SellerId,
            DateOfAcquisition= ExemplarDto.DateOfAcquisition,
        });
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        var responseStream = new TestServerStreamWriter<ExemplarGrpcDto>(callContext);

        using var call = Exc.GetAllExemplarsFromClient(new RequestWithClientId{Id ="bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"}, responseStream, callContext);

        await call;
        responseStream.Complete();
        
        var allMessages = new List<ExemplarGrpcDto>();
        await foreach (var message in responseStream.ReadAllAsync())
        {
            allMessages.Add(message);
        }
        Assert.Equal(JsonConvert.SerializeObject(lstExp), JsonConvert.SerializeObject(allMessages));
    }

    [Fact]
    public async void TestGetBySellerFail()
    {
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        var responseStream = new TestServerStreamWriter<ExemplarGrpcDto>(callContext);

        await Assert.ThrowsAsync<RpcException>(() =>Exc.GetAllExemplarsFromClient(new RequestWithClientId{Id ="aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"}, responseStream, callContext));
        
    }
}