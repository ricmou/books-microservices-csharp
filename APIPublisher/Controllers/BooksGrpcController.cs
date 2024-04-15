using System.Threading.Tasks;
using APIPublisher.Domain.Books;
using APIPublisher.Domain.Publishers;
using APIPublisher.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace APIPublisher.Controllers;

public class BooksGrpcController : APIPublisherBooksGRPC.APIPublisherBooksGRPCBase
{
    private readonly ILogger<BooksGrpcController> _logger;
    private readonly IBooksService _service;
    
    public BooksGrpcController(ILogger<BooksGrpcController> logger, IBooksService service)
    {
        _logger = logger;
        _service = service;
    }

    public override async Task<BookGrpcDto> GetBookByISBN(RequestWithISBN request, ServerCallContext context)
    {
        var book = await this._service.GetByIdAsync(new BookId(request.Id));
        
        if (book == null)
        {
            var metadata = new Metadata
            {
                { "ISBN", request.Id }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "No data Found"), metadata);
        }

        return new BookGrpcDto
        {
            Id = book.Id,
            Publisher = new PublisherGrpcDto
            {
                PublisherId = book.Publisher.PublisherId,
                Name = book.Publisher.Name,
                Country = book.Publisher.Country
            }
        };
    }

    public override async Task GetAllBooks(Empty request, IServerStreamWriter<BookGrpcDto> responseStream, ServerCallContext context)
    {
        var lstAllBooks = await _service.GetAllAsync();

        if (lstAllBooks == null || lstAllBooks.Count < 1)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "No data Found"));
        }

        foreach (var book in lstAllBooks)
        {
            await responseStream.WriteAsync(new BookGrpcDto
            {
                Id = book.Id,
                Publisher = new PublisherGrpcDto
                {
                    PublisherId = book.Publisher.PublisherId,
                    Name = book.Publisher.Name,
                    Country = book.Publisher.Country
                }
            });
        }
    }

    public override async Task GetAllBooksFromPublisher(RequestWithPublisherId request, IServerStreamWriter<BookGrpcDto> responseStream,
        ServerCallContext context)
    {
        var lstAllBooks = await _service.GetAllFromPublisherAsync(new PublisherId(request.Id));

        if (lstAllBooks == null || lstAllBooks.Count < 1)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "No data Found"));
        }

        foreach (var book in lstAllBooks)
        {
            await responseStream.WriteAsync(new BookGrpcDto
            {
                Id = book.Id,
                Publisher = new PublisherGrpcDto
                {
                    PublisherId = book.Publisher.PublisherId,
                    Name = book.Publisher.Name,
                    Country = book.Publisher.Country
                }
            });
        }
    }

    public override async Task<BookGrpcDto> AddNewBook(CreatingBooksGrpcDto request, ServerCallContext context)
    {
        var book = await _service.AddAsync(new CreatingBooksDto(request.Id, request.PublisherId));
        
        if (book == null)
        {
            var metadata = new Metadata
            {
                { "ISBN", request.Id}
            };
            throw new RpcException(new Status(StatusCode.Aborted, "Failed to add book"), metadata);
        }
        
        /*var bookFetch = await _service.GetByIdAsync(new BookId(request.Id));
        
        if (bookFetch == null)
        {
            var metadata = new Metadata
            {
                { "ISBN", request.Id}
            };
            throw new RpcException(new Status(StatusCode.Aborted, "Failed to add book"), metadata);
        }*/
        
        return new BookGrpcDto
        {
            Id = book.Id,
            Publisher = new PublisherGrpcDto
            {
                PublisherId = book.Publisher.PublisherId,
                Name = book.Publisher.Name,
                Country = book.Publisher.Country
            }
        };
        
    }

    public override async Task<BookGrpcDto> ModifyBook(CreatingBooksGrpcDto request, ServerCallContext context)
    {
        var book = await _service.UpdateAsync(new CreatingBooksDto(request.Id, request.PublisherId));
        
        if (book == null)
        {
            var metadata = new Metadata
            {
                { "ISBN", request.Id }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "Could not find Book"), metadata);
        }
        
        return new BookGrpcDto
        {
            Id = book.Id,
            Publisher = new PublisherGrpcDto
            {
                PublisherId = book.Publisher.PublisherId,
                Name = book.Publisher.Name,
                Country = book.Publisher.Country
            }
        };
    }

    public override async Task<BookGrpcDto> DeleteBook(RequestWithISBN request, ServerCallContext context)
    {
        var book = await _service.DeleteAsync(new BookId(request.Id));
        
        if (book == null)
        {
            var metadata = new Metadata
            {
                { "ISBN", request.Id }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "Could not find Book"), metadata);
        }
        
        return new BookGrpcDto
        {
            Id = book.Id,
            Publisher = new PublisherGrpcDto
            {
                PublisherId = book.Publisher.PublisherId,
                Name = book.Publisher.Name,
                Country = book.Publisher.Country
            }
        };
    }
}