using System.Collections.Generic;
using System.Threading.Tasks;
using APIExemplar.Domain.Exemplars;

namespace APIExemplar.Services
{
    public interface IExemplarService
    {
        Task<List<ExemplarDto>> GetAllAsync();
        Task<ExemplarDto> GetByIdAsync(ExemplarId id);
        Task<List<ExemplarDto>> GetByBookIdAsync(BookId id);
        Task<List<ExemplarDto>> GetBySellerIdAsync(ClientId id);
        Task<ExemplarDto> AddAsync(CreatingExemplarDto dto);
        Task<ExemplarDto> UpdateAsync(ExemplarDto dto);
        Task<ExemplarDto> DeleteAsync(ExemplarId id);
    }
}