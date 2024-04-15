using System.Threading.Tasks;

namespace APIClients.Domain.Shared
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
    }
}