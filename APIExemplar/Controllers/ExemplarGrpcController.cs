using System.Threading.Tasks;
using APIExemplar.Domain.Exemplars;
using APIExemplar.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace APIExemplar.Controllers;

public class ExemplarGrpcController : APIExemplarExemplarGRPC.APIExemplarExemplarGRPCBase
{
    private readonly ILogger<ExemplarGrpcController> _logger;
    private readonly IExemplarService _service;

    public ExemplarGrpcController(ILogger<ExemplarGrpcController> logger, IExemplarService service)
    {
        _logger = logger;
        _service = service;
    }
    
    public override async Task<ExemplarGrpcDto> GetExemplarByID(RequestWithExemplarId request, ServerCallContext context)
    {
        var exemplar = await this._service.GetByIdAsync(new ExemplarId(request.Id));

        if (exemplar == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.Id }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "No data Found"), metadata);
        }

        return new ExemplarGrpcDto()
        {
            ExemplarId = exemplar.ExemplarId,
            BookId = exemplar.BookId,
            BookState = exemplar.BookState,
            SellerId = exemplar.SellerId,
            DateOfAcquisition = exemplar.DateOfAcquisition
        };
    }

    public override async Task GetAllExemplarsFromBook(RequestWithISBN request, IServerStreamWriter<ExemplarGrpcDto> responseStream, ServerCallContext context)
    {
        var lstAllExemplar = await _service.GetByBookIdAsync(new BookId(request.Id));

        if (lstAllExemplar == null || lstAllExemplar.Count < 1)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "No data Found"));
        }
        
        foreach (var exemplar in lstAllExemplar)
        {
            await responseStream.WriteAsync(new ExemplarGrpcDto
            {
                ExemplarId = exemplar.ExemplarId,
                BookId = exemplar.BookId,
                BookState = exemplar.BookState,
                SellerId = exemplar.SellerId,
                DateOfAcquisition = exemplar.DateOfAcquisition
            });
        }
    }

    public override async Task GetAllExemplarsFromClient(RequestWithClientId request, IServerStreamWriter<ExemplarGrpcDto> responseStream,
        ServerCallContext context)
    {
        var lstAllExemplar = await _service.GetBySellerIdAsync(new ClientId(request.Id));

        if (lstAllExemplar == null || lstAllExemplar.Count < 1)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "No data Found"));
        }
        
        foreach (var exemplar in lstAllExemplar)
        {
            await responseStream.WriteAsync(new ExemplarGrpcDto
            {
                ExemplarId = exemplar.ExemplarId,
                BookId = exemplar.BookId,
                BookState = exemplar.BookState,
                SellerId = exemplar.SellerId,
                DateOfAcquisition = exemplar.DateOfAcquisition
            });
        }
    }

    public override async Task GetAllExemplars(Empty request, IServerStreamWriter<ExemplarGrpcDto> responseStream, ServerCallContext context)
    {
        var lstAllExemplar = await _service.GetAllAsync();

        if (lstAllExemplar == null || lstAllExemplar.Count < 1)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "No data Found"));
        }
        
        foreach (var exemplar in lstAllExemplar)
        {
            await responseStream.WriteAsync(new ExemplarGrpcDto
            {
                ExemplarId = exemplar.ExemplarId,
                BookId = exemplar.BookId,
                BookState = exemplar.BookState,
                SellerId = exemplar.SellerId,
                DateOfAcquisition = exemplar.DateOfAcquisition
            });
        }
    }

    public override async Task<ExemplarGrpcDto> AddNewExemplar(CreatingExemplarGrpcDto request, ServerCallContext context)
    {
        var exemplar = await _service.AddAsync(new CreatingExemplarDto(request.BookId, request.BookState, request.SellerId, request.DateOfAcquisition));

        if (exemplar == null)
        {
            var metadata = new Metadata
            {
                { "BookID", request.BookId },
                { "SellerID", request.SellerId}
            };
            throw new RpcException(new Status(StatusCode.Aborted, "Failed to add exemplar"), metadata);
        }

        /*var exemplarFetch = await _service.GetByIdAsync(new ExemplarId(exemplar.ExemplarId));

        if (exemplarFetch == null)
        {
            var metadata = new Metadata
            {
                { "Book ID", request.BookId },
                { "Seller ID", request.SellerId}
            };
            throw new RpcException(new Status(StatusCode.Aborted, "Failed to add exemplar"), metadata);
        }*/

        return new ExemplarGrpcDto()
        {
            ExemplarId = exemplar.ExemplarId,
            BookId = exemplar.BookId,
            BookState = exemplar.BookState,
            SellerId = exemplar.SellerId,
            DateOfAcquisition = exemplar.DateOfAcquisition
        };
    }

    public override async Task<ExemplarGrpcDto> ModifyExemplar(ExemplarGrpcDto request, ServerCallContext context)
    {
        var exemplar = await _service.UpdateAsync(new ExemplarDto(request.ExemplarId, request.BookId, request.BookState,
            request.SellerId, request.DateOfAcquisition));
        
        if (exemplar == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.ExemplarId }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "Could not find exemplar"), metadata);
        }
        
        return new ExemplarGrpcDto()
        {
            ExemplarId = exemplar.ExemplarId,
            BookId = exemplar.BookId,
            BookState = exemplar.BookState,
            SellerId = exemplar.SellerId,
            DateOfAcquisition = exemplar.DateOfAcquisition
        };
    }

    public override async Task<ExemplarGrpcDto> DeleteExemplar(RequestWithExemplarId request, ServerCallContext context)
    {
        var exemplar = await _service.DeleteAsync(new ExemplarId(request.Id));
        
        if (exemplar == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.Id }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "Could not find exemplar"), metadata);
        }

        return new ExemplarGrpcDto()
        {
            ExemplarId = exemplar.ExemplarId,
            BookId = exemplar.BookId,
            BookState = exemplar.BookState,
            SellerId = exemplar.SellerId,
            DateOfAcquisition = exemplar.DateOfAcquisition
        };
    }
}