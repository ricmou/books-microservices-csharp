using System.Threading.Tasks;
using APIExemplar.Domain.Shared;

namespace APIExemplar.Infraestructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly APIExemplarDbContext _context;

        public UnitOfWork(APIExemplarDbContext context)
        {
            this._context = context;
        }

        public async Task<int> CommitAsync()
        {
            return await this._context.SaveChangesAsync();
        }
    }
}