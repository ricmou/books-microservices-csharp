using System.Collections.Generic;
using System.Threading.Tasks;
using APIPublisher.Domain.Publishers;
using APIPublisher.Domain.Shared;

namespace APIPublisher.Domain.Books
{
    public interface IBooksRepository: IRepository<Book, BookId>
    {
        Task<List<Book>> GetByPublisherIdAsync(PublisherId id);
    }
}