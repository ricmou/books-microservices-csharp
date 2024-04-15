using System.Collections.Generic;
using System.Threading.Tasks;
using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Books;

namespace APIAuthors.Services
{
    public interface IBooksService
    {
        Task<List<BooksDto>> GetAllAsync();
        Task<List<BooksDto>> GetByAuthorIdAsync(AuthorId id);
        Task<BooksDto> GetByIdAsync(BookId id);
        Task<BooksDto> AddAsync(CreatingBooksDto dto);
        Task<BooksDto> UpdateAsync(CreatingBooksDto dto);
        Task<BooksDto> DeleteAsync(BookId id);
    }
}