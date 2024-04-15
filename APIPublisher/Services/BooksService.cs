using System.Collections.Generic;
using System.Threading.Tasks;
using APIPublisher.Domain.Books;
using APIPublisher.Domain.Publishers;
using APIPublisher.Domain.Shared;

namespace APIPublisher.Services
{
    public class BooksService : IBooksService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBooksRepository _bookRepo;
        private readonly IPublishersRepository _pubRepo;

        public BooksService(IUnitOfWork unitOfWork, IBooksRepository bookRepo, IPublishersRepository pubRepo)
        {
            this._unitOfWork = unitOfWork;
            this._bookRepo = bookRepo;
            this._pubRepo = pubRepo;
        }

        public async Task<List<BooksDto>> GetAllAsync()
        {
            var list = await this._bookRepo.GetAllAsync();

            List<BooksDto> listDto = list.ConvertAll<BooksDto>(book => new BooksDto(
                book.Id.AsString(),
                new PublisherDto(book.Publisher.Id.AsString(), book.Publisher.Name, book.Publisher.Country.Name)));

            return listDto;
        }

        public async Task<List<BooksDto>> GetAllFromPublisherAsync(PublisherId id)
        {
            var list = await this._bookRepo.GetByPublisherIdAsync(id);

            List<BooksDto> listDto = list.ConvertAll<BooksDto>(book => new BooksDto(
                book.Id.AsString(),
                new PublisherDto(book.Publisher.Id.AsString(), book.Publisher.Name, book.Publisher.Country.Name)));

            return listDto;
        }

        public async Task<BooksDto> GetByIdAsync(BookId id)
        {
            var book = await this._bookRepo.GetByIdAsync(id);

            if (book == null)
                return null;

            return new BooksDto(book.Id.AsString(),
                new PublisherDto(book.Publisher.Id.AsString(), book.Publisher.Name, book.Publisher.Country.Name));
        }

        public async Task<BooksDto> AddAsync(CreatingBooksDto dto)
        {
            //Create Value objects here
            var existPublisher =
                await _pubRepo.GetByIdAsync(new PublisherId(dto.PublisherId));

            if (existPublisher == null)
            {
                //Console.WriteLine("New author it is: " + cat.Name);
                throw new BusinessRuleValidationException("There is no author identified as " + dto.PublisherId + " in the database. Please add the author before adding the book");
            }

            var book = new Book(dto.Id, existPublisher);

            await this._bookRepo.AddAsync(book);

            await this._unitOfWork.CommitAsync();

            return new BooksDto(book.Id.AsString(), new PublisherDto(book.Publisher.Id.AsString(), book.Publisher.Name, book.Publisher.Country.Name));
        }

        public async Task<BooksDto> UpdateAsync(CreatingBooksDto dto)
        {
            var book = await this._bookRepo.GetByIdAsync(new BookId(dto.Id)); 

            if (book == null)
                return null;   

            // change all field
            var existPublisher =
                await _pubRepo.GetByIdAsync(new PublisherId(dto.PublisherId));
            
            if (existPublisher == null)
            {
                throw new BusinessRuleValidationException("There is no author identified as " + dto.PublisherId + " in the database. Please add the author before adding the book");
            }
            
            book.ChangePublisher(existPublisher);

            await this._unitOfWork.CommitAsync();

            return new BooksDto(book.Id.AsString(), new PublisherDto(book.Publisher.Id.AsString(), book.Publisher.Name, book.Publisher.Country.Name));
        }

        public async Task<BooksDto> DeleteAsync(BookId id)
        {
            var book = await this._bookRepo.GetByIdAsync(id);

            if (book == null)
                return null;

            this._bookRepo.Remove(book);
            await this._unitOfWork.CommitAsync();

            return new BooksDto(book.Id.AsString(), new PublisherDto(book.Publisher.Id.AsString(), book.Publisher.Name, book.Publisher.Country.Name));
        }
    }
}