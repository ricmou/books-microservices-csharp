using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIPublisher.Domain.Books;
using APIPublisher.Domain.Publishers;
using APIPublisher.Infraestructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace APIPublisher.Infraestructure.Books
{
    public class BooksRepository : BaseRepository<Book, BookId>, IBooksRepository
    {
    
        private APIPublisherDbContext _context;
    
        public BooksRepository(APIPublisherDbContext context):base(context.Books)
        {
            _context = context;
        }
        
        public async Task<List<Book>> GetByPublisherIdAsync(PublisherId id)
        {
            return await _context.Books.Include(book => book.Publisher).Where(x =>
                x.Publisher.Id.Value == id.Value).ToListAsync();;
        }

        public new async Task<Book> GetByIdAsync(BookId id)
        {
            return await _context.Books.Include(book => book.Publisher).Where(x => id.Equals(x.Id)).FirstOrDefaultAsync();
        }

        public new async Task<List<Book>> GetAllAsync()
        {
            return await _context.Books.Include(book => book.Publisher).ToListAsync();
        }


    }
}