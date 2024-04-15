using System.Threading.Tasks;
using APIClients.Domain.Clients;
using APIClients.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace APIClients.Controllers;

public class ClientsGrpcController : APIClientsClientGRPC.APIClientsClientGRPCBase
{
    private readonly ILogger<ClientsGrpcController> _logger;
    private readonly IClientsService _service;

    public ClientsGrpcController(ILogger<ClientsGrpcController> logger, IClientsService service)
    {
        _logger = logger;
        _service = service;
    }
    
    public override async Task<ClientGrpcDto> GetClientByID(RequestWithClientId request, ServerCallContext context)
    {
        var client = await this._service.GetByIdAsync(new ClientId(request.Id));

        if (client == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.Id }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "No data Found"), metadata);
        }

        return new ClientGrpcDto()
        {
            ClientId = client.ClientId,
            Name = client.Name,
            Street = client.Street,
            Local = client.Local,
            PostalCode = client.PostalCode,
            Country = client.Country
        };
    }

    public override async Task GetAllClients(Empty request, IServerStreamWriter<ClientGrpcDto> responseStream, ServerCallContext context)
    {
        var lstAllClient = await _service.GetAllAsync();

        if (lstAllClient == null || lstAllClient.Count < 1)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "No data Found"));
        }
        
        foreach (var client in lstAllClient)
        {
            await responseStream.WriteAsync(new ClientGrpcDto
            {
                ClientId = client.ClientId,
                Name = client.Name,
                Street = client.Street,
                Local = client.Local,
                PostalCode = client.PostalCode,
                Country = client.Country
            });
        }
    }

    public override async Task<ClientGrpcDto> AddNewClient(CreatingClientGrpcDto request, ServerCallContext context)
    {
        var client = await _service.AddAsync(new CreatingClientDto(request.Name, request.Street, request.Local, request.PostalCode, request.Country));

        if (client == null)
        {
            var metadata = new Metadata
            {
                { "Name", request.Name }
            };
            throw new RpcException(new Status(StatusCode.Aborted, "Failed to add client"), metadata);
        }

        /*var clientFetch = await _service.GetByIdAsync(new ClientId(client.ClientId));

        if (clientFetch == null)
        {
            var metadata = new Metadata
            {
                { "Name", request.Name }
            };
            throw new RpcException(new Status(StatusCode.Aborted, "Failed to add client"), metadata);
        }*/

        return new ClientGrpcDto
        {
            ClientId = client.ClientId,
            Name = client.Name,
            Street = client.Street,
            Local = client.Local,
            PostalCode = client.PostalCode,
            Country = client.Country
        };
    }

    public override async Task<ClientGrpcDto> ModifyClient(ClientGrpcDto request, ServerCallContext context)
    {
        var client = await _service.UpdateAsync(new ClientDto(request.ClientId, request.Name, request.Street, request.Local,
            request.PostalCode, request.Country));
        
        if (client == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.ClientId }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "Could not find client"), metadata);
        }
        
        return new ClientGrpcDto()
        {
            ClientId = client.ClientId,
            Name = client.Name,
            Street = client.Street,
            Local = client.Local,
            PostalCode = client.PostalCode,
            Country = client.Country
        };
    }

    public override async Task<ClientGrpcDto> DeleteClient(RequestWithClientId request, ServerCallContext context)
    {
        var client = await _service.DeleteAsync(new ClientId(request.Id));
        
        if (client == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.Id }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "Could not find client"), metadata);
        }

        return new ClientGrpcDto()
        {
            ClientId = client.ClientId,
            Name = client.Name,
            Street = client.Street,
            Local = client.Local,
            PostalCode = client.PostalCode,
            Country = client.Country
        };
    }
}