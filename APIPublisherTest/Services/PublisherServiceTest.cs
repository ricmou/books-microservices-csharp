using APIPublisher.Domain.Publishers;
using APIPublisher.Domain.Shared;
using APIPublisher.Services;
using Moq;
using Newtonsoft.Json;

namespace APIPublisherTest.Services;

public abstract class PublisherServiceTestSetup : IDisposable
{
    public PublisherService Whs;

    public PublisherServiceTestSetup()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(unit => unit.CommitAsync().Result).Returns(0);

        var publisher = new Publisher("AWE", "Addison Wesley", "US");
        var publisher2 =
            new Publisher("ORE", "O'Reilly", "GB");
        List<Publisher> list = new List<Publisher>();
        list.Add(publisher);
            
        var repo = new Mock<IPublishersRepository>();
        repo.Setup(rep => rep.GetAllAsync().Result).Returns(list);
        repo.Setup(rep => rep.GetByIdAsync(new PublisherId("AWE")).Result).Returns(publisher);
        repo.Setup(rep => rep.GetByIdAsync(new PublisherId("ORE")).Result).Returns<Publisher>(null);
        repo.Setup(rep => rep.AddAsync(publisher).Result).Returns(publisher);
        repo.Setup(rep => rep.AddAsync(It.IsAny<Publisher>()).Result).Returns<Publisher>(null);

        Whs = new PublisherService(unitOfWork.Object, repo.Object);
    }
        
    public void Dispose()
    {

    }
}

public class PublisherServiceTest : PublisherServiceTestSetup
{
    [Fact]
        public void TestGetByIdAsyncValid()
        {
            var pub = new PublisherDto("AWE", "Addison Wesley", "US");
            var pdto = Whs.GetByIdAsync(new PublisherId("AWE"));
            Assert.Equal(JsonConvert.SerializeObject(pub),JsonConvert.SerializeObject(pdto.Result));
        }
        
        [Fact]
        public void TestGetByIdAsyncInvalid()
        {
            var pdto = Whs.GetByIdAsync(new PublisherId("ORE"));
            Assert.Null(pdto.Result);
        }

        [Fact]
        public void TestGetAllAsync()
        {
            var pub = new PublisherDto("AWE", "Addison Wesley", "US");
            var pdto = Whs.GetAllAsync();
            Assert.Equal(JsonConvert.SerializeObject(pub),JsonConvert.SerializeObject(pdto.Result[0]));
        }
        
        [Fact]
        public void TestDeleteAsyncValid()
        {
            var pub = new PublisherDto("AWE", "Addison Wesley", "US");
            var pdto = Whs.DeleteAsync(new PublisherId("AWE"));
            Assert.Equal(JsonConvert.SerializeObject(pub),JsonConvert.SerializeObject(pdto.Result));
        }
        
        [Fact]
        public void TestDeleteAsyncInvalid()
        {
            var pdto = Whs.DeleteAsync(new PublisherId("ORE"));
            Assert.Null(pdto.Result);
        }
        
        [Fact]
        public void TestUpdateAsyncValid()
        {
            var pub = new PublisherDto("AWE", "Addison Wesleyss", "DE");
            var pdto = Whs.UpdateAsync(new PublisherDto("AWE", "Addison Wesleyss", "DE"));
            Assert.Equal(JsonConvert.SerializeObject(pub),JsonConvert.SerializeObject(pdto.Result));
        }
        
        [Fact]
        public void TestUpdateAsyncInvalid()
        {
            var pdto = Whs.UpdateAsync(new PublisherDto("ORE", "O'Reilly", "GB"));
            Assert.Null(pdto.Result);
        }
        
        [Fact]
        public void TestAddAsyncValid()
        {
            var createpub = Whs.AddAsync(new CreatingPublisherDto("AWE", "Addison Wesley", "US"));
            var respub = new PublisherDto("AWE", "Addison Wesley", "US");
            Assert.Equal(JsonConvert.SerializeObject(respub),JsonConvert.SerializeObject(createpub.Result));
        }
}