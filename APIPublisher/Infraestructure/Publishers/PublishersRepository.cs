using APIPublisher.Domain.Publishers;
using APIPublisher.Infraestructure.Shared;

namespace APIPublisher.Infraestructure.Publishers;

public class PublishersRepository : BaseRepository<Publisher, PublisherId>, IPublishersRepository
{
    
    public PublishersRepository(APIPublisherDbContext context):base(context.Publishers)
    {
           
    }


}