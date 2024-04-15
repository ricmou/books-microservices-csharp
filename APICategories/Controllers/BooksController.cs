using System.Collections.Generic;
using System.Threading.Tasks;
using APICategories.Domain.Books;
using APICategories.Domain.Shared;
using APICategories.Services;
using Microsoft.AspNetCore.Mvc;

namespace APICategories.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBooksService _service;

        public BooksController(IBooksService service)
        {
            _service = service;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BooksDto>>> GetAll()
        {
            return await _service.GetAllAsync();
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BooksDto>> GetGetById(string id)
        {
            var book = await _service.GetByIdAsync(new BookId(id));

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
            var book = await _service.AddAsync(dto);

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
                var book = await _service.UpdateAsync(dto);

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
                var book = await _service.DeleteAsync(new BookId(id));

                if (book == null)
                {
                    return NotFound();
                }

                return Ok(book);
            }
            catch(BusinessRuleValidationException ex)
            {
               return BadRequest(new {Message = ex.Message});
            }
        }
    }
}