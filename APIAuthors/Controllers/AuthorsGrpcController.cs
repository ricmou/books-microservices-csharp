using System.Threading.Tasks;
using APIAuthors.Domain.Authors;
using APIAuthors.Services;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace APIAuthors.Controllers;

public class AuthorsGrpcController : APIAuthorsAuthorGRPC.APIAuthorsAuthorGRPCBase
{
    private readonly ILogger<AuthorsGrpcController> _logger;
    private readonly IAuthorsService _service;

    public AuthorsGrpcController(ILogger<AuthorsGrpcController> logger, IAuthorsService service)
    {
        _logger = logger;
        _service = service;
    }

    public override async Task<AuthorGrpcDto> GetAuthorByID(RequestWithAuthorId request, ServerCallContext context)
    {
        var author = await this._service.GetByIdAsync(new AuthorId(request.Id));

        if (author == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.Id }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "No data Found"), metadata);
        }

        return new AuthorGrpcDto
        {
            AuthorId = author.AuthorId,
            FirstName = author.FirstName,
            LastName = author.LastName,
            BirthDate = author.BirthDate,
            Country = author.Country
        };
    }

    public override async Task GetAllAuthors(Empty request, IServerStreamWriter<AuthorGrpcDto> responseStream,
        ServerCallContext context)
    {
        var lstAllAuthor = await _service.GetAllAsync();

        if (lstAllAuthor == null || lstAllAuthor.Count < 1)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "No data Found"));
        }

        foreach (var author in lstAllAuthor)
        {
            await responseStream.WriteAsync(new AuthorGrpcDto
            {
                AuthorId = author.AuthorId,
                FirstName = author.FirstName,
                LastName = author.LastName,
                BirthDate = author.BirthDate,
                Country = author.Country
            });
        }
    }

    public override async Task<AuthorGrpcDto> AddNewAuthor(CreatingAuthorGrpcDto request, ServerCallContext context)
    {
        var author = await _service.AddAsync(new CreatingAuthorsDto(request.AuthorId, request.FirstName,
            request.LastName, request.BirthDate, request.Country));

        if (author == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.AuthorId }
            };
            throw new RpcException(new Status(StatusCode.Aborted, "Failed to add author"), metadata);
        }

        //var authorFetch = await _service.GetByIdAsync(new AuthorId(request.AuthorId));

        /*if (authorFetch == null)
        {
            var metadata = new Metadata
            {
                { "ISBN", request.AuthorId }
            };
            throw new RpcException(new Status(StatusCode.Aborted, "Failed to add author"), metadata);
        }*/

        return new AuthorGrpcDto
        {
            AuthorId = author.AuthorId,
            FirstName = author.FirstName,
            LastName = author.LastName,
            BirthDate = author.BirthDate,
            Country = author.Country
        };
    }

    public override async Task<AuthorGrpcDto> ModifyAuthor(AuthorGrpcDto request, ServerCallContext context)
    {
        var author = await _service.UpdateAsync(new AuthorDto(request.AuthorId, request.FirstName, request.LastName, request.BirthDate, request.Country));
        
        if (author == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.AuthorId }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "Could not find author"), metadata);
        }
        
        return new AuthorGrpcDto
        {
            AuthorId = author.AuthorId,
            FirstName = author.FirstName,
            LastName = author.LastName,
            BirthDate = author.BirthDate,
            Country = author.Country
        };
    }

    public override async Task<AuthorGrpcDto> DeleteAuthor(RequestWithAuthorId request, ServerCallContext context)
    {
        var author = await _service.DeleteAsync(new AuthorId(request.Id));
        
        if (author == null)
        {
            var metadata = new Metadata
            {
                { "ID", request.Id }
            };
            throw new RpcException(new Status(StatusCode.NotFound, "Could not find Author"), metadata);
        }

        return new AuthorGrpcDto
        {
            AuthorId = author.AuthorId,
            FirstName = author.FirstName,
            LastName = author.LastName,
            BirthDate = author.BirthDate,
            Country = author.Country
        };
    }
}