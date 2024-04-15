using System.Collections.Generic;
using System.Threading.Tasks;
using APIClients.Domain.Clients;
using APIClients.Domain.Shared;

namespace APIClients.Services
{
    public class ClientsService : IClientsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClientsRepository _repo;

        public ClientsService(IUnitOfWork unitOfWork, IClientsRepository repo)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
        }

        public async Task<List<ClientDto>> GetAllAsync()
        {
            var list = await this._repo.GetAllAsync();

            List<ClientDto> listDto = list.ConvertAll<ClientDto>(client => new ClientDto(
                client.Id.AsString(), client.Name, client.Address.Street, client.Address.Local, client.Address.PostalCode, client.Address.Country.Name));

            return listDto;
        }

        public async Task<ClientDto> GetByIdAsync(ClientId id)
        {
            var client = await this._repo.GetByIdAsync(id);
            
            if(client == null)
                return null;

            return new ClientDto(client.Id.AsString(), client.Name, client.Address.Street, client.Address.Local,
                client.Address.PostalCode, client.Address.Country.Name);
        }

        public async Task<ClientDto> AddAsync(CreatingClientDto dto)
        {
            //Create Value objects here
            var address = new Address(dto.Street, dto.Local, dto.PostalCode, dto.Country);
            
            var client = new Client(dto.Name, address);

            await this._repo.AddAsync(client);

            await this._unitOfWork.CommitAsync();

            return new ClientDto(client.Id.AsString(), client.Name, client.Address.Street, client.Address.Local,
                client.Address.PostalCode, client.Address.Country.Name);
        }

        public async Task<ClientDto> UpdateAsync(ClientDto dto)
        {
            var client = await this._repo.GetByIdAsync(new ClientId(dto.ClientId)); 

            if (client == null)
                return null;   

            // change all field
            client.ChangeName(dto.Name);
            client.ChangeAddress(new Address(dto.Street, dto.Local, dto.PostalCode, dto.Country));

            await this._unitOfWork.CommitAsync();

            return new ClientDto(client.Id.AsString(), client.Name, client.Address.Street, client.Address.Local,
                client.Address.PostalCode, client.Address.Country.Name);
        }

        public async Task<ClientDto> DeleteAsync(ClientId id)
        {
            var client = await this._repo.GetByIdAsync(id); 

            if (client == null)
                return null;

            this._repo.Remove(client);
            await this._unitOfWork.CommitAsync();

            return new ClientDto(client.Id.AsString(), client.Name, client.Address.Street, client.Address.Local,
                client.Address.PostalCode, client.Address.Country.Name);
        }
        
    }
}