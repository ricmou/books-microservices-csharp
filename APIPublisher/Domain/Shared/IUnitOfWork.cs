using System.Threading.Tasks;

namespace APIPublisher.Domain.Shared
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
    }
}