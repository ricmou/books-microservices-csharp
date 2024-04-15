using System.Collections.Generic;
using System.Threading.Tasks;
using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Books;
using APIAuthors.Domain.Shared;

namespace APIAuthors.Services
{
    public class BooksService : IBooksService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBooksRepository _booksRepository;
        private readonly IAuthorsRepository _authorsRepository;

        public BooksService(IUnitOfWork unitOfWork, IBooksRepository repo, IAuthorsRepository authorsSRepository)
        {
            this._unitOfWork = unitOfWork;
            this._booksRepository = repo;
            this._authorsRepository = authorsSRepository;
        }

        public async Task<List<BooksDto>> GetAllAsync()
        {
            var list = await this._booksRepository.GetAllAsync();

            List<BooksDto> listDto = list.ConvertAll<BooksDto>(book => new BooksDto(
                book.Id.AsString(), 
                book.Authors.ConvertAll<AuthorDto>(author => new AuthorDto(author.Id.AsString(), author.Name.FirstName, author.Name.LastName, author.BirthDate.ToString(), author.Country.Name))));
            
            
            return listDto;
        }

        public async Task<List<BooksDto>> GetByAuthorIdAsync(AuthorId id)
        {
            var list = await this._booksRepository.GetByAuthorIdAsync(id);
            
            List<BooksDto> listDto = list.ConvertAll<BooksDto>(book => new BooksDto(
                book.Id.AsString(), 
                book.Authors.ConvertAll<AuthorDto>(author => new AuthorDto(author.Id.AsString(), author.Name.FirstName, author.Name.LastName, author.BirthDate.ToString(), author.Country.Name))));
            return listDto;
        }

        public async Task<BooksDto> GetByIdAsync(BookId id)
        {
            var book = await this._booksRepository.GetByIdAsync(id);
            
            if(book == null)
                return null;

            return new BooksDto(book.Id.AsString(), book.Authors.ConvertAll<AuthorDto>(author =>
                new AuthorDto(author.Id.AsString(), author.Name.FirstName, author.Name.LastName, author.BirthDate.ToString(), author.Country.Name)));
        }

        public async Task<BooksDto> AddAsync(CreatingBooksDto dto)
        {
            //Create Value objects here

            var book = new Book(dto.Id);

            foreach (string auth in dto.Authors)
            {
                var existAuthor =
                    await _authorsRepository.GetByIdAsync(new AuthorId(auth));

                if (existAuthor != null)
                {
                    book.AddAuthor(existAuthor);
                }
                else
                {
                    throw new BusinessRuleValidationException("There is no author identified as " + auth + " in the database. Please add the author before adding the book");
                }
            }

            await this._booksRepository.AddAsync(book);

            await this._unitOfWork.CommitAsync();

            return new BooksDto(book.Id.AsString(), book.Authors.ConvertAll<AuthorDto>(author =>
                new AuthorDto(author.Id.AsString(), author.Name.FirstName, author.Name.LastName, author.BirthDate.ToString(), author.Country.Name)));
        }

        public async Task<BooksDto> UpdateAsync(CreatingBooksDto dto)
        {
            var book = await this._booksRepository.GetByIdAsync(new BookId(dto.Id)); 

            if (book == null)
                return null;   
            
            book.ClearAuthors();
            
            // change all field
            foreach (string auth in dto.Authors)
            {
                var existAuthor =
                    await _authorsRepository.GetByIdAsync(new AuthorId(auth));
                
                //Check if valid
                if (existAuthor == null)
                {
                    throw new BusinessRuleValidationException("There is no Author identified as " + auth + " in the database. Please add the author before adding the book");
                }
                
                //Check if not already in list
                if (!book.Authors.Contains(existAuthor))
                {
                    book.AddAuthor(existAuthor);
                }
                
                //If none of these, already exists, skip
            }
            
            await this._unitOfWork.CommitAsync();

            return new BooksDto(book.Id.AsString(), book.Authors.ConvertAll<AuthorDto>(author =>
                new AuthorDto(author.Id.AsString(), author.Name.FirstName, author.Name.LastName, author.BirthDate.ToString(), author.Country.Name)));
        }

        public async Task<BooksDto> DeleteAsync(BookId id)
        {
            var book = await this._booksRepository.GetByIdAsync(id);

            if (book == null)
                return null;

            this._booksRepository.Remove(book);
            await this._unitOfWork.CommitAsync();

            return new BooksDto(book.Id.AsString(), book.Authors.ConvertAll<AuthorDto>(author =>
                new AuthorDto(author.Id.AsString(), author.Name.FirstName, author.Name.LastName, author.BirthDate.ToString(), author.Country.Name)));
        }
    }
}