using System.Collections.Generic;
using System.Threading.Tasks;
using APIPublisher.Domain.Books;
using APIPublisher.Domain.Publishers;

namespace APIPublisher.Services
{
    public interface IBooksService
    {
        Task<List<BooksDto>> GetAllAsync();
        Task<List<BooksDto>> GetAllFromPublisherAsync(PublisherId id);
        Task<BooksDto> GetByIdAsync(BookId id);
        Task<BooksDto> AddAsync(CreatingBooksDto dto);
        Task<BooksDto> UpdateAsync(CreatingBooksDto dto);
        Task<BooksDto> DeleteAsync(BookId id);
    }
}