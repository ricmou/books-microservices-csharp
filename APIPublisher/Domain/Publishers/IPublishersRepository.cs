using APIPublisher.Domain.Shared;

namespace APIPublisher.Domain.Publishers;

public interface IPublishersRepository: IRepository<Publisher, PublisherId>
{
    
}