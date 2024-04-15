using APICategories.Domain.Shared;

namespace APICategories.Domain.Books
{
    public interface IBooksRepository: IRepository<Book, BookId>
    {
    }
}