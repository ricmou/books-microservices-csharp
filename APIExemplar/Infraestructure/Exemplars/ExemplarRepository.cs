using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIExemplar.Domain.Exemplars;
using APIExemplar.Infraestructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace APIExemplar.Infraestructure.Exemplars
{
    public class ExemplarRepository : BaseRepository<Exemplar, ExemplarId>, IExemplarRepository
    {
        private APIExemplarDbContext _context;
    
        public ExemplarRepository(APIExemplarDbContext context):base(context.Exemplars)
        {
            _context = context;
        }

        public async Task<List<Exemplar>> GetExemplarsOfId(BookId bookId)
        {
            return await _context.Exemplars.Where(exemplar => exemplar.Book.Value == bookId.Value).ToListAsync();
        }

        public async Task<List<Exemplar>> GetExemplarsOfClient(ClientId clientId)
        {
            return await _context.Exemplars.Where(exemplar => exemplar.SellerId.Value == clientId.Value).ToListAsync();
        }
    }
}