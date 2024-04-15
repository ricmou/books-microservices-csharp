using System.Collections.Generic;
using System.Threading.Tasks;
using APICategories.Domain.Books;
using APICategories.Domain.Categories;
using APICategories.Domain.Shared;

namespace APICategories.Services
{
    public class BooksService : IBooksService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBooksRepository _bookRepo;
        private readonly ICategoryRepository _catRepo;

        public BooksService(IUnitOfWork unitOfWork, IBooksRepository bookRepo, ICategoryRepository catRepo)
        {
            this._unitOfWork = unitOfWork;
            this._bookRepo = bookRepo;
            this._catRepo = catRepo;
        }

        public async Task<List<BooksDto>> GetAllAsync()
        {
            var list = await this._bookRepo.GetAllAsync();

            List<BooksDto> listDto = list.ConvertAll<BooksDto>(book => new BooksDto(
                book.Id.AsString(), 
                book.Categories.ConvertAll<CategoryDto>(cat => new CategoryDto(cat.Id.AsString(), cat.Name))));

            return listDto;
        }

        public async Task<BooksDto> GetByIdAsync(BookId id)
        {
            var book = await this._bookRepo.GetByIdAsync(id);
            
            if(book == null)
                return null;

            return new BooksDto(book.Id.AsString(), book.Categories.ConvertAll<CategoryDto>(cat =>
                new CategoryDto(cat.Id.AsString(), cat.Name)));
        }

        public async Task<BooksDto> AddAsync(CreatingBooksDto dto)
        {
            //Create Value objects here

            var book = new Book(dto.Id);

            foreach (string cat in dto.Categories)
            {
                var existCategory =
                    await _catRepo.GetByIdAsync(new CategoryId(cat));

                if (existCategory != null)
                {
                    //Console.WriteLine("Crap, this Cat already exists: " + existCategory.Name);
                    book.AddCategory(existCategory);
                }
                else
                {
                    //Console.WriteLine("New author it is: " + cat.Name);
                    throw new BusinessRuleValidationException("There is no Category identified as " + cat + " in the database. Please add the author before adding the book");
                }
            }

            await this._bookRepo.AddAsync(book);

            await this._unitOfWork.CommitAsync();

            return new BooksDto(book.Id.AsString(), book.Categories.ConvertAll<CategoryDto>(cat =>
                new CategoryDto(cat.Id.AsString(), cat.Name)));
        }

        public async Task<BooksDto> UpdateAsync(CreatingBooksDto dto)
        {
            var book = await this._bookRepo.GetByIdAsync(new BookId(dto.Id)); 

            if (book == null)
                return null;

            book.ClearCategories();
            
            // change all field
            foreach (string cat in dto.Categories)
            {
                var existCategory =
                    await _catRepo.GetByIdAsync(new CategoryId(cat));
                
                //Check if valid
                if (existCategory == null)
                {
                    throw new BusinessRuleValidationException("There is no Category identified as " + cat + " in the database. Please add the author before adding the book");
                }
                
                //Check if not already in list
                if (!book.Categories.Contains(existCategory))
                {
                    book.AddCategory(existCategory);
                }
                
                //If none of these, already exists, skip
            }
            
            await this._unitOfWork.CommitAsync();

            return new BooksDto(book.Id.AsString(), book.Categories.ConvertAll<CategoryDto>(cat =>
                new CategoryDto(cat.Id.AsString(), cat.Name)));
        }

        public async Task<BooksDto> DeleteAsync(BookId id)
        {
            var book = await this._bookRepo.GetByIdAsync(id);

            if (book == null)
                return null;

            this._bookRepo.Remove(book);
            await this._unitOfWork.CommitAsync();

            return new BooksDto(book.Id.AsString(), book.Categories.ConvertAll<CategoryDto>(cat =>
                new CategoryDto(cat.Id.AsString(), cat.Name)));
        }
    }
}