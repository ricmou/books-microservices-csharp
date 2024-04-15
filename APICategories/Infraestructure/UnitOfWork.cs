using System.Threading.Tasks;
using APICategories.Domain.Shared;

namespace APICategories.Infraestructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly APICategoriesDbContext _context;

        public UnitOfWork(APICategoriesDbContext context)
        {
            this._context = context;
        }

        public async Task<int> CommitAsync()
        {
            return await this._context.SaveChangesAsync();
        }
    }
}