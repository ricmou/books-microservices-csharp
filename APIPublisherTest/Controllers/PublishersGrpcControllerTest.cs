using APIPublisher;
using APIPublisher.Controllers;
using APIPublisher.Domain.Publishers;
using APIPublisher.Services;
using APIPublisherTest.Helpers;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;

namespace APIPublisherTest.Controllers;

public abstract class PublisherGrpcControllerTestSetup : IDisposable
{
    protected PublishersGrpcController Auc;

    protected PublisherDto PubDto;
    protected PublisherDto PubDto2;
    protected CreatingPublisherDto CreatingPubDto;
    protected CreatingPublisherDto CreatingPubDto2;

    public PublisherGrpcControllerTestSetup()
    {
        PubDto = new PublisherDto("AWE", "Addison Wesley", "US");
        List<PublisherDto> listDto = new List<PublisherDto>();
        listDto.Add(PubDto);

        PubDto2 = new PublisherDto("ORE", "O'Reilly", "GB");

        CreatingPubDto = new CreatingPublisherDto("AWE", "Addison Wesley", "US");
        
        CreatingPubDto2 = new CreatingPublisherDto("ORE", "O'Reilly", "GB");

        var PublisherService = new Mock<IPublisherService>();
        PublisherService.Setup(au => au.GetAllAsync().Result).Returns(listDto);
        PublisherService.Setup(au => au.GetByIdAsync(new PublisherId("AWE")).Result).Returns(PubDto);
        PublisherService.Setup(au => au.GetByIdAsync(new PublisherId("LUL")).Result).Returns<PublisherDto>(null);
        PublisherService.Setup(au => au.AddAsync(It.Is<CreatingPublisherDto>(i => 
            i.PublisherId == CreatingPubDto.PublisherId)).Result).Returns(PubDto);
        PublisherService.Setup(au => au.AddAsync(It.Is<CreatingPublisherDto>(i => 
            i.PublisherId == CreatingPubDto2.PublisherId)).Result).Returns<PublisherDto>(null);
        PublisherService.Setup(au => au.UpdateAsync(It.Is<PublisherDto>(i => 
            i.PublisherId == PubDto.PublisherId)).Result).Returns<PublisherDto>(null);
        PublisherService.Setup(au => au.UpdateAsync(It.Is<PublisherDto>(i => 
            i.PublisherId == PubDto2.PublisherId)).Result).Returns(PubDto2);
        PublisherService.Setup(au => au.DeleteAsync(new PublisherId("AWE")).Result).Returns<PublisherDto>(null);
        PublisherService.Setup(au => au.DeleteAsync(new PublisherId("ORE")).Result).Returns(PubDto2);

        var logger = new Mock<ILogger<PublishersGrpcController>>();
        
        Auc = new PublishersGrpcController(logger.Object, PublisherService.Object);
    }

    public void Dispose()
    {
    }
}

public class PublishersGrpcControllerTest : PublisherGrpcControllerTestSetup
{
    [Fact]
    public async void TestGetAllAsync()
    {
        var pub = new PublisherGrpcDto{
           PublisherId = PubDto.PublisherId, 
           Name = PubDto.Name,
           Country = PubDto.Country
        };
        List<PublisherGrpcDto> listPub = new List<PublisherGrpcDto>();
        listPub.Add(pub);
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        var responseStream = new TestServerStreamWriter<PublisherGrpcDto>(callContext);

        using var call = Auc.GetAllPublishers(new Empty(), responseStream, callContext);

        await call;
        responseStream.Complete();
        
        var allMessages = new List<PublisherGrpcDto>();
        await foreach (var message in responseStream.ReadAllAsync())
        {
            allMessages.Add(message);
        }
        
        Assert.Equal(JsonConvert.SerializeObject(listPub), JsonConvert.SerializeObject(allMessages));
    }

    [Fact]
    public async void TestGetGetByIdValid()
    {
        var pub = new PublisherGrpcDto{
            PublisherId = PubDto.PublisherId, 
            Name = PubDto.Name,
            Country = PubDto.Country
        };

        //GRPC setup
        var callContext = TestServerCallContext.Create();

        var response = await Auc.GetPublisherByID(new RequestWithPublisherId{Id = "AWE"}, callContext);
        

        Assert.Equal(JsonConvert.SerializeObject(response), JsonConvert.SerializeObject(pub));
    }

    [Fact]
    public async Task TestGetGetByIdInValid()
    {
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>( () => Auc.GetPublisherByID(new RequestWithPublisherId{Id = "LUL"}, callContext));
        
    }
    
    [Fact]
    public async void TestCreate()
    {
        var pub = new PublisherGrpcDto{
            PublisherId = PubDto.PublisherId, 
            Name = PubDto.Name,
            Country = PubDto.Country
        };

        //GRPC setup
        var callContext = TestServerCallContext.Create();
        
        var response = await Auc.AddNewPublisher(new CreatingPublisherGrpcDto
        {
            PublisherId = CreatingPubDto.PublisherId,
            Name = CreatingPubDto.Name,
            Country = CreatingPubDto.Country
        }, callContext);

        Assert.Equal(JsonConvert.SerializeObject(pub), JsonConvert.SerializeObject(response));
    }
    
    [Fact]
    public async Task TestCreateInvalid()
    {
        var callContext = TestServerCallContext.Create();
        await Assert.ThrowsAsync<RpcException>( () => Auc.AddNewPublisher(new CreatingPublisherGrpcDto
        {
            PublisherId = CreatingPubDto2.PublisherId,
            Name = CreatingPubDto2.Name,
            Country = CreatingPubDto2.Country
        }, callContext));
        
    }


    [Fact]
    public async void TestUpdateNullDto()
    {
        var pub = new PublisherGrpcDto{
            PublisherId = PubDto.PublisherId, 
            Name = PubDto.Name,
            Country = PubDto.Country
        };
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        
        await Assert.ThrowsAsync<RpcException>( () => Auc.ModifyPublisher(pub, callContext));
    }

    [Fact]
    public async void TestUpdateSuccess()
    {
        var pub = new PublisherGrpcDto{
            PublisherId = PubDto2.PublisherId, 
            Name = PubDto2.Name,
            Country = PubDto2.Country
        };
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();
        
        var adto = await Auc.ModifyPublisher(pub, callContext);
        
        Assert.Equal(JsonConvert.SerializeObject(adto), JsonConvert.SerializeObject(pub));
    }

    [Fact]
    public async void TestDeleteNullDto()
    {
        //GRPC setup
        var callContext = TestServerCallContext.Create();

        await Assert.ThrowsAsync<RpcException>( () => Auc.DeletePublisher(new RequestWithPublisherId{Id ="AWE"}, callContext));
    }

    [Fact]
    public async void TestDeleteSuccess()
    {
        var pub = new PublisherGrpcDto{
            PublisherId = PubDto2.PublisherId, 
            Name = PubDto2.Name,
            Country = PubDto2.Country
        };
        
        //GRPC setup
        var callContext = TestServerCallContext.Create();

        var adto = await Auc.DeletePublisher(new RequestWithPublisherId { Id = "ORE" }, callContext);
        
        Assert.Equal(JsonConvert.SerializeObject(pub), JsonConvert.SerializeObject(adto));
    }
    
}