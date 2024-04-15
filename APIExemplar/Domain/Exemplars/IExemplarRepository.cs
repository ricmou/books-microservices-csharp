using System.Collections.Generic;
using System.Threading.Tasks;
using APIExemplar.Domain.Shared;

namespace APIExemplar.Domain.Exemplars
{
    public interface IExemplarRepository: IRepository<Exemplar, ExemplarId>
    {
        Task<List<Exemplar>> GetExemplarsOfId(BookId bookId);

        Task<List<Exemplar>> GetExemplarsOfClient(ClientId clientId);
    }
}