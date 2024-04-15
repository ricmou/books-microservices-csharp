using System.Collections.Generic;
using System.Threading.Tasks;
using APIAuthors.Domain.Authors;

namespace APIAuthors.Services;

public interface IAuthorsService
{
    Task<List<AuthorDto>> GetAllAsync();
    Task<AuthorDto> GetByIdAsync(AuthorId id);
    Task<AuthorDto> AddAsync(CreatingAuthorsDto dto);
    Task<AuthorDto> UpdateAsync(AuthorDto dto);
    Task<AuthorDto> DeleteAsync(AuthorId id);
}