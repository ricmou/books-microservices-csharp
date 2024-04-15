using System.Collections.Generic;
using System.Threading.Tasks;
using APIPublisher.Domain.Publishers;
using APIPublisher.Domain.Shared;

namespace APIPublisher.Services;

public class PublisherService : IPublisherService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishersRepository _repo;

    public PublisherService(IUnitOfWork unitOfWork, IPublishersRepository repo)
    {
        _unitOfWork = unitOfWork;
        _repo = repo;
    }

    public async Task<List<PublisherDto>> GetAllAsync()
    {
        var list = await this._repo.GetAllAsync();

        List<PublisherDto> listDto = list.ConvertAll<PublisherDto>(publisher => new PublisherDto(publisher.Id.AsString(), publisher.Name, publisher.Country.Name));

        return listDto;
    }

    public async Task<PublisherDto> GetByIdAsync(PublisherId id)
    {
        var publisher = await this._repo.GetByIdAsync(id);
            
        if(publisher == null)
            return null;

        return new PublisherDto(publisher.Id.AsString(), publisher.Name, publisher.Country.Name);
    }

    public async Task<PublisherDto> AddAsync(CreatingPublisherDto dto)
    {
        var publisher = new Publisher(dto.PublisherId, dto.Name, dto.Country);
        
        await this._repo.AddAsync(publisher);

        await this._unitOfWork.CommitAsync();
        
        return new PublisherDto(publisher.Id.AsString(), publisher.Name, publisher.Country.Name);
    }

    public async Task<PublisherDto> UpdateAsync(PublisherDto dto)
    {
        var publisher = await this._repo.GetByIdAsync(new PublisherId(dto.PublisherId)); 

        if (publisher == null)
            return null;   

        // change all field
        publisher.ChangeName(dto.Name);
        publisher.ChangeCountry(dto.Country);

        await this._unitOfWork.CommitAsync();

        return new PublisherDto(publisher.Id.AsString(), publisher.Name, publisher.Country.Name);
    }

    public async Task<PublisherDto> DeleteAsync(PublisherId id)
    {
        var publisher = await this._repo.GetByIdAsync(id);

        if (publisher == null)
            return null;

        this._repo.Remove(publisher);
        await this._unitOfWork.CommitAsync();

        return new PublisherDto(publisher.Id.AsString(), publisher.Name, publisher.Country.Name);
    }
}