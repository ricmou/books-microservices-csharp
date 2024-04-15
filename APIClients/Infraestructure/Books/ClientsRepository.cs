using APIClients.Domain.Clients;
using APIClients.Infraestructure.Shared;

namespace APIClients.Infraestructure.Books
{
    public class ClientsRepository : BaseRepository<Client, ClientId>, IClientsRepository
    {
    
        public ClientsRepository(APIClientsDbContext context):base(context.Clients)
        {
           
        }


    }
}