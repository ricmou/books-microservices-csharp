using System.Threading.Tasks;
using APIPublisher.Domain.Publishers;
using APIPublisher.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace APIPublisher.Controllers;

public class PublishersGrpcController : APIPublisherPublisherGRPC.APIPublisherPublisherGRPCBase
{
    private readonly ILogger<PublishersGrpcController> _logger;
    private readonly IPublisherService _service;

    public PublishersGrpcController(ILogger<PublishersGrpcController> logger, IPublisherService service)
    {
        _logger = logger;
        _service = service;
    }
    
    public override async Task<PublisherGrpcDto> GetPublisherByID(RequestWithPublisherId request, ServerCallContext context)
    {
        var publisher = await this._service.GetByIdAsync(new PublisherId(request.Id));

        if (publisher == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.Id }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "No data Found"), metadata);
        }

        return new PublisherGrpcDto
        {
            PublisherId = publisher.PublisherId,
            Name = publisher.Name,
            Country = publisher.Country
        };
    }

    public override async Task GetAllPublishers(Empty request, IServerStreamWriter<PublisherGrpcDto> responseStream, ServerCallContext context)
    {
        var lstAllPublisher = await _service.GetAllAsync();

        if (lstAllPublisher == null || lstAllPublisher.Count < 1)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "No data Found"));
        }
        
        foreach (var publisher in lstAllPublisher)
        {
            await responseStream.WriteAsync(new PublisherGrpcDto
            {
                PublisherId = publisher.PublisherId,
                Name = publisher.Name,
                Country = publisher.Country
            });
        }
    }
    
    

    public override async Task<PublisherGrpcDto> AddNewPublisher(CreatingPublisherGrpcDto request, ServerCallContext context)
    {
        var publisher = await _service.AddAsync(new CreatingPublisherDto(request.PublisherId, request.Name, request.Country));

        if (publisher == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.PublisherId }
            };
            throw new RpcException(new Status(StatusCode.Aborted, "Failed to add publisher"), metadata);
        }

        /*var publisherFetch = await _service.GetByIdAsync(new PublisherId(request.PublisherId));

        if (publisherFetch == null)
        {
            var metadata = new Metadata
            {
                { "ISBN", request.PublisherId }
            };
            throw new RpcException(new Status(StatusCode.Aborted, "Failed to add publisher"), metadata);
        }*/

        return new PublisherGrpcDto
        {
            PublisherId = publisher.PublisherId,
            Name = publisher.Name,
            Country = publisher.Country
        };
    }

    public override async Task<PublisherGrpcDto> ModifyPublisher(PublisherGrpcDto request, ServerCallContext context)
    {
        var publisher = await _service.UpdateAsync(new PublisherDto(request.PublisherId, request.Name, request.Country));
        
        if (publisher == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.PublisherId }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "Could not find publisher"), metadata);
        }
        
        return new PublisherGrpcDto
        {
            PublisherId = publisher.PublisherId,
            Name = publisher.Name,
            Country = publisher.Country
        };
    }

    public override async Task<PublisherGrpcDto> DeletePublisher(RequestWithPublisherId request, ServerCallContext context)
    {
        var publisher = await _service.DeleteAsync(new PublisherId(request.Id));
        
        if (publisher == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.Id }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "Could not find publisher"), metadata);
        }

        return new PublisherGrpcDto
        {
            PublisherId = publisher.PublisherId,
            Name = publisher.Name,
            Country = publisher.Country
        };
    }
}