using System.Threading.Tasks;
using APIPublisher.Domain.Shared;

namespace APIPublisher.Infraestructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly APIPublisherDbContext _context;

        public UnitOfWork(APIPublisherDbContext context)
        {
            this._context = context;
        }

        public async Task<int> CommitAsync()
        {
            return await this._context.SaveChangesAsync();
        }
    }
}