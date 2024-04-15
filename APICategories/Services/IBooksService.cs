using System.Collections.Generic;
using System.Threading.Tasks;
using APICategories.Domain.Books;

namespace APICategories.Services
{
    public interface IBooksService
    {
        Task<List<BooksDto>> GetAllAsync();
        Task<BooksDto> GetByIdAsync(BookId id);
        Task<BooksDto> AddAsync(CreatingBooksDto dto);
        Task<BooksDto> UpdateAsync(CreatingBooksDto dto);
        Task<BooksDto> DeleteAsync(BookId id);
    }
}