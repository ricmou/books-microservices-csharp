using System.Collections.Generic;
using System.Threading.Tasks;
using APIPublisher.Domain.Publishers;

namespace APIPublisher.Services;

public interface IPublisherService
{
    Task<List<PublisherDto>> GetAllAsync();
    Task<PublisherDto> GetByIdAsync(PublisherId id);
    Task<PublisherDto> AddAsync(CreatingPublisherDto dto);
    Task<PublisherDto> UpdateAsync(PublisherDto dto);
    Task<PublisherDto> DeleteAsync(PublisherId id);
}