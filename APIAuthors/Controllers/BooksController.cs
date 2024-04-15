using System.Collections.Generic;
using System.Threading.Tasks;
using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Books;
using APIAuthors.Domain.Shared;
using APIAuthors.Services;
using Microsoft.AspNetCore.Mvc;

namespace APIAuthors.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBooksService _booksService;

        public BooksController(IBooksService booksService)
        {
            _booksService = booksService;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BooksDto>>> GetAll()
        {
            return await _booksService.GetAllAsync();
        }
        
        [HttpGet("Author/{id}")]
        public async Task<ActionResult<IEnumerable<BooksDto>>> GetAllFromAuthor(string id)
        {
            return await _booksService.GetByAuthorIdAsync(new AuthorId(id));
        }

        // GET: api/Books/[ISBN]
        [HttpGet("{id}")]
        public async Task<ActionResult<BooksDto>> GetGetById(string id)
        {
            var book = await _booksService.GetByIdAsync(new BookId(id));

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<BooksDto>> Create(CreatingBooksDto dto)
        {
            var book = await _booksService.AddAsync(dto);

            //return CreatedAtAction(nameof(GetGetById), new { id = book.Id }, book);

            return book;
        }

        
        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<ActionResult<BooksDto>> Update(string id, CreatingBooksDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }

            try
            {
                var book = await _booksService.UpdateAsync(dto);

                if (book == null)
                {
                    return NotFound();
                }

                return Ok(book);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<BooksDto>> HardDelete(string id)
        {
            try
            {
                var book = await _booksService.DeleteAsync(new BookId(id));

                if (book == null)
                {
                    return NotFound();
                }

                return Ok(book);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }

        }
    }
}