using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Books;
using APIAuthors.Infraestructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace APIAuthors.Infraestructure.Books
{
    public class BooksRepository : BaseRepository<Book, BookId>, IBooksRepository
    {
        private ApiAuthorsDbContext _context;
    
        public BooksRepository(ApiAuthorsDbContext context):base(context.Books)
        {
            _context = context;
        }

        public new async Task<Book> GetByIdAsync(BookId id)
        {
            return await _context.Books.Include(book => book.Authors).Where(x => id.Equals(x.Id)).FirstOrDefaultAsync();
        }

        public async Task<List<Book>> GetByAuthorIdAsync(AuthorId id)
        {
            return await _context.Books.Include(book => book.Authors).Where(x =>
                x.Authors.FirstOrDefault(author => author.Id.Value == id.Value) != null).ToListAsync();;
        }

        public new async Task<List<Book>> GetAllAsync()
        {
            return await _context.Books.Include(book => book.Authors).ToListAsync();
        }
    }
}