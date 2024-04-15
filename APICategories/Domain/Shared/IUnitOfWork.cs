using System.Threading.Tasks;

namespace APICategories.Domain.Shared
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
    }
}