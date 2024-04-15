using System.Threading.Tasks;

namespace APIAuthors.Domain.Shared
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
    }
}