using System.Threading.Tasks;
using APIClients.Domain.Shared;

namespace APIClients.Infraestructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly APIClientsDbContext _context;

        public UnitOfWork(APIClientsDbContext context)
        {
            this._context = context;
        }

        public async Task<int> CommitAsync()
        {
            return await this._context.SaveChangesAsync();
        }
    }
}