using System.Threading.Tasks;

namespace APIExemplar.Domain.Shared
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
    }
}