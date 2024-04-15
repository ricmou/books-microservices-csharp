using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Shared;

namespace APIAuthors.Services;

public class AuthorsService : IAuthorsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthorsRepository _repo;

    public AuthorsService(IUnitOfWork unitOfWork, IAuthorsRepository repo)
    {
        this._unitOfWork = unitOfWork;
        this._repo = repo;
    }

    public async Task<List<AuthorDto>> GetAllAsync()
    {
        var list = await this._repo.GetAllAsync();

        List<AuthorDto> listDto = list.ConvertAll<AuthorDto>(author => new AuthorDto(author.Id.AsString(), author.Name.FirstName, author.Name.LastName, author.BirthDate.ToString(), author.Country.Name));

        return listDto;
    }

    public async Task<AuthorDto> GetByIdAsync(AuthorId id)
    {
        var author = await this._repo.GetByIdAsync(id);
            
        if(author == null)
            return null;

        return new AuthorDto(author.Id.AsString(), author.Name.FirstName, author.Name.LastName, author.BirthDate.ToString(), author.Country.Name);
    }

    public async Task<AuthorDto> AddAsync(CreatingAuthorsDto dto)
    {
        var author = new Author(dto.AuthorId, dto.FirstName, dto.LastName, DateOnly.Parse(dto.BirthDate),
            dto.Country);
        
        await this._repo.AddAsync(author);

        await this._unitOfWork.CommitAsync();
        
        return new AuthorDto(author.Id.AsString(), author.Name.FirstName, author.Name.LastName, author.BirthDate.ToString(), author.Country.Name);
    }

    public async Task<AuthorDto> UpdateAsync(AuthorDto dto)
    {
        var author = await this._repo.GetByIdAsync(new AuthorId(dto.AuthorId)); 

        if (author == null)
            return null;   

        // change all field
        author.ChangeName(new AuthorName(dto.FirstName,dto.LastName));
        author.ChangeBirthDate(DateOnly.Parse(dto.BirthDate));
        author.ChangeCountry(dto.Country);

        await this._unitOfWork.CommitAsync();

        return new AuthorDto(author.Id.AsString(), author.Name.FirstName, author.Name.LastName, author.BirthDate.ToString(), author.Country.Name);
    }

    public async Task<AuthorDto> DeleteAsync(AuthorId id)
    {
        var author = await this._repo.GetByIdAsync(id);

        if (author == null)
            return null;

        this._repo.Remove(author);
        await this._unitOfWork.CommitAsync();

        return new AuthorDto(author.Id.AsString(), author.Name.FirstName, author.Name.LastName, author.BirthDate.ToString(), author.Country.Name);
    }
}