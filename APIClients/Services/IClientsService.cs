using System.Collections.Generic;
using System.Threading.Tasks;
using APIClients.Domain.Clients;

namespace APIClients.Services
{
    public interface IClientsService
    {
        Task<List<ClientDto>> GetAllAsync();
        Task<ClientDto> GetByIdAsync(ClientId id);
        Task<ClientDto> AddAsync(CreatingClientDto dto);
        Task<ClientDto> UpdateAsync(ClientDto dto);
        Task<ClientDto> DeleteAsync(ClientId id);
    }
}