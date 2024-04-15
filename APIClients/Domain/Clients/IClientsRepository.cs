using APIClients.Domain.Shared;

namespace APIClients.Domain.Clients
{
    public interface IClientsRepository: IRepository<Client, ClientId>
    {
    }
}