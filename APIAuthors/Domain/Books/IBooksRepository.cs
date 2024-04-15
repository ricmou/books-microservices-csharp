using System.Collections.Generic;
using System.Threading.Tasks;
using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Shared;

namespace APIAuthors.Domain.Books
{
    public interface IBooksRepository: IRepository<Book, BookId>
    {
        Task<List<Book>> GetByAuthorIdAsync(AuthorId id);
    }
}