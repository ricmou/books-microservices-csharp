using System;
using System.Threading.Tasks;
using APIAuthors.Domain.Shared;

namespace APIAuthors.Domain.Authors
{
    public interface IAuthorsRepository: IRepository<Author, AuthorId>
    {
        Task<Author> GetByNameBirthdate(string firstName, string lastName, DateOnly date);
    }
}