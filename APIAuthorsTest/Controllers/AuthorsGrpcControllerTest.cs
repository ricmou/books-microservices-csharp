using APIAuthors;
using APIAuthors.Controllers;
using APIAuthors.Domain.Authors;
using APIAuthors.Services;
using APIAuthorsTest.Helpers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace APIAuthorsTest.Controllers;

public abstract class AuthorGrpcControllerTestSetup : IDisposable
{
    protected AuthorsGrpcController Auc;

    protected AuthorDto AuthorDto;
    protected AuthorDto AuthorDto2;
    protected CreatingAuthorsDto CreatingAuthorDto;
    protected CreatingAuthorsDto CreatingAuthorDto2;

    public AuthorGrpcControllerTestSetup()
    {
        AuthorDto = new AuthorDto("RE1", "FirstName", "LastName", "01/01/1999", "DE");
        List<AuthorDto> listDto = new List<AuthorDto>();
        listDto.Add(AuthorDto);

        AuthorDto2 = new AuthorDto("RE2", "FirstName2", "LastName2", "02/02/1999", "CA");

        CreatingAuthorDto = new CreatingAuthorsDto("RE1", "FirstName", "LastName", "01/01/1999", "DE");
        
        CreatingAuthorDto2 = new CreatingAuthorsDto("RE2", "FirstName2", "LastName2", "01/01/1999", "ZA");

        var AuthorService = new Mock<IAuthorsService>();
        AuthorService.Setup(au => au.GetAllAsync().Result).Returns(listDto);
        AuthorService.Setup(au => au.GetByIdAsync(new AuthorId("RE1")).Result).Returns(AuthorDto);
        AuthorService.Setup(au => au.GetByIdAsync(new AuthorId("RE9")).Result).Returns<AuthorDto>(null);
        AuthorService.Setup(au => au.AddAsync(It.Is<CreatingAuthorsDto>(i => 
            i.AuthorId == CreatingAuthorDto.AuthorId)).Result).Returns(AuthorDto);
        AuthorService.Setup(au => au.AddAsync(It.Is<CreatingAuthorsDto>(i => 
            i.AuthorId == CreatingAuthorDto2.AuthorId)).Result).Returns<AuthorDto>(null);
        AuthorService.Setup(au => au.UpdateAsync(It.Is<AuthorDto>(i => 
            i.AuthorId == AuthorDto.AuthorId)).Result).Returns<AuthorDto>(null);
        AuthorService.Setup(au => au.UpdateAsync(It.Is<AuthorDto>(i => 
            i.AuthorId == AuthorDto2.AuthorId)).Result).Returns(AuthorDto2);
        AuthorService.Setup(au => au.DeleteAsync(new AuthorId("RE1")).Result).Returns<AuthorDto>(null);
        AuthorService.Setup(au => au.DeleteAsync(new AuthorId("RE2")).Result).Returns(AuthorDto2);

        var logger = new Mock<ILogger<AuthorsGrpcController>>();
        
        Auc = new AuthorsGrpcController(logger.Object, AuthorService.Object);
    }

    public void Dispose()
    {
    }
}

public class AuthorsGrpcControllerTest : AuthorGrpcControllerTestSetup
{
    [Fact]
    public async void TestGetAllAsync()
    {
        var aut = new AuthorGrpcDto{
           AuthorId = "RE1", 
           FirstName = "FirstName", 
           LastName = "LastName", 
           BirthDate = "01/01/1999", 
           Country = "DE"
        };
        List<AuthorGrpcDto> listAut = new List<AuthorGrpcDto>();
        listAut.Add(aut);
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        var responseStream = new TestServerStreamWriter<AuthorGrpcDto>(callContext);

        using var call = Auc.GetAllAuthors(new Empty(), responseStream, callContext);

        await call;
        responseStream.Complete();
        
        var allMessages = new List<AuthorGrpcDto>();
        await foreach (var message in responseStream.ReadAllAsync())
        {
            allMessages.Add(message);
        }
        
        Assert.Equal(JsonConvert.SerializeObject(listAut), JsonConvert.SerializeObject(allMessages));
    }

    [Fact]
    public async void TestGetGetByIdValid()
    {
        var aut = new AuthorGrpcDto{
            AuthorId = "RE1", 
            FirstName = "FirstName", 
            LastName = "LastName", 
            BirthDate = "01/01/1999", 
            Country = "DE"
        };

        //GRPC setup
        var callContext = TestServerCallContext.Create();

        var response = await Auc.GetAuthorByID(new RequestWithAuthorId{Id = "RE1"}, callContext);
        

        Assert.Equal(JsonConvert.SerializeObject(response), JsonConvert.SerializeObject(aut));
    }

    [Fact]
    public async Task TestGetGetByIdInValid()
    {
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>( () => Auc.GetAuthorByID(new RequestWithAuthorId{Id = "RE9"}, callContext));
        
    }
    
    [Fact]
    public async void TestCreate()
    {
        var aut = new AuthorGrpcDto{
            AuthorId = "RE1", 
            FirstName = "FirstName", 
            LastName = "LastName", 
            BirthDate = "01/01/1999", 
            Country = "DE"
        };

        //GRPC setup
        var callContext = TestServerCallContext.Create();
        
        var response = await Auc.AddNewAuthor(new CreatingAuthorGrpcDto
        {
            AuthorId = CreatingAuthorDto.AuthorId,
            FirstName = CreatingAuthorDto.FirstName,
            LastName = CreatingAuthorDto.LastName,
            BirthDate = CreatingAuthorDto.BirthDate,
            Country = CreatingAuthorDto.Country
        }, callContext);

        Assert.Equal(JsonConvert.SerializeObject(aut), JsonConvert.SerializeObject(response));
    }
    
    [Fact]
    public async Task TestCreateInvalid()
    {
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>( () => Auc.AddNewAuthor(new CreatingAuthorGrpcDto
        {
            AuthorId = CreatingAuthorDto2.AuthorId,
            FirstName = CreatingAuthorDto2.FirstName,
            LastName = CreatingAuthorDto2.LastName,
            BirthDate = CreatingAuthorDto2.BirthDate,
            Country = CreatingAuthorDto2.Country
        }, callContext));
        
    }


    [Fact]
    public async void TestUpdateNullDto()
    {
        var aut = new AuthorGrpcDto{
            AuthorId = "RE1", 
            FirstName = "FirstName", 
            LastName = "LastName", 
            BirthDate = "01/01/1999", 
            Country = "DE"
        };
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        
        await Assert.ThrowsAsync<RpcException>( () => Auc.ModifyAuthor(aut, callContext));
    }

    [Fact]
    public async void TestUpdateSuccess()
    {
        var aut = new AuthorGrpcDto{
            AuthorId = AuthorDto2.AuthorId, 
            FirstName = AuthorDto2.FirstName, 
            LastName = AuthorDto2.LastName, 
            BirthDate = AuthorDto2.BirthDate, 
            Country = AuthorDto2.Country
        };
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        
        var adto = await Auc.ModifyAuthor(aut, callContext);
        
        Assert.Equal(JsonConvert.SerializeObject(adto), JsonConvert.SerializeObject(aut));
    }

    [Fact]
    public async void TestDeleteNullDto()
    {
        //GRPC setup
        var callContext = TestServerCallContext.Create();

        await Assert.ThrowsAsync<RpcException>( () => Auc.DeleteAuthor(new RequestWithAuthorId{Id ="RE1"}, callContext));
    }

    [Fact]
    public async void TestDeleteSuccess()
    {
        var aut = new AuthorGrpcDto{
            AuthorId = AuthorDto2.AuthorId, 
            FirstName = AuthorDto2.FirstName, 
            LastName = AuthorDto2.LastName, 
            BirthDate = AuthorDto2.BirthDate, 
            Country = AuthorDto2.Country
        };
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();

        var adto = await Auc.DeleteAuthor(new RequestWithAuthorId { Id = "RE2" }, callContext);
        
        Assert.Equal(JsonConvert.SerializeObject(aut), JsonConvert.SerializeObject(adto));
    }
    
}