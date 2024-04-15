using System;
using System.Linq;
using System.Threading.Tasks;
using APIAuthors.Domain.Authors;
using APIAuthors.Infraestructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace APIAuthors.Infraestructure.Authors
{
    public class AuthorRepository : BaseRepository<Author, AuthorId>, IAuthorsRepository
    {
    
        private ApiAuthorsDbContext _context;
    
        public AuthorRepository(ApiAuthorsDbContext context):base(context.Authors)
        {
            _context = context;
        }

        public async Task<Author> GetByNameBirthdate(string firstName, string lastName, DateOnly date)
        {
            return await _context.Authors
                .Where(author => firstName.Equals(author.Name.FirstName))
                .Where(author => lastName.Equals(author.Name.LastName))
                .Where(author => date.Equals(author.BirthDate))
                .FirstOrDefaultAsync();
        }
    }
}