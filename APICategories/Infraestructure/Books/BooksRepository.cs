using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APICategories.Domain.Books;
using APICategories.Infraestructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace APICategories.Infraestructure.Books
{
    public class BooksRepository : BaseRepository<Book, BookId>, IBooksRepository
    {
        private APICategoriesDbContext _context;
    
        public BooksRepository(APICategoriesDbContext context):base(context.Books)
        {
            _context = context;
        }

        public new async Task<Book> GetByIdAsync(BookId id)
        {
            return await _context.Books.Include(book => book.Categories).Where(x => id.Equals(x.Id)).FirstOrDefaultAsync();
        }

        public new async Task<List<Book>> GetAllAsync()
        {
            return await _context.Books.Include(book => book.Categories).ToListAsync();
        }
    }
}