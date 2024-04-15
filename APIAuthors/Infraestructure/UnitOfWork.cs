using System.Threading.Tasks;
using APIAuthors.Domain.Shared;

namespace APIAuthors.Infraestructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApiAuthorsDbContext _context;

        public UnitOfWork(ApiAuthorsDbContext context)
        {
            this._context = context;
        }

        public async Task<int> CommitAsync()
        {
            return await this._context.SaveChangesAsync();
        }
    }
}