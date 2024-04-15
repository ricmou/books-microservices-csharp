using System.Collections.Generic;
using System.Threading.Tasks;
using APIExemplar.Domain.Exemplars;
using APIExemplar.Domain.Shared;
using APIExemplar.Services;
using Microsoft.AspNetCore.Mvc;

namespace APIExemplar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExemplarController : ControllerBase
    {
        private readonly IExemplarService _service;

        public ExemplarController(IExemplarService service)
        {
            _service = service;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExemplarDto>>> GetAll()
        {
            return await _service.GetAllAsync();
        }
        
        // GET: api/Books
        [HttpGet("Book/{id}")]
        public async Task<ActionResult<IEnumerable<ExemplarDto>>> GetByBookId(string id)
        {
            return await _service.GetByBookIdAsync(new BookId(id));
        }
        
        [HttpGet("Client/{id}")]
        public async Task<ActionResult<IEnumerable<ExemplarDto>>> GetByClientId(string id)
        {
            return await _service.GetBySellerIdAsync(new ClientId(id));
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ExemplarDto>> GetGetById(string id)
        {
            var exemplar = await _service.GetByIdAsync(new ExemplarId(id));

            if (exemplar == null)
            {
                return NotFound();
            }

            return exemplar;
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<ExemplarDto>> Create(CreatingExemplarDto dto)
        {
            var exemplar = await _service.AddAsync(dto);

            return exemplar;
        }

        
        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ExemplarDto>> Update(string id, ExemplarDto dto)
        {
            if (id != dto.ExemplarId)
            {
                return BadRequest();
            }

            try
            {
                var exemplar = await _service.UpdateAsync(dto);

                if (exemplar == null)
                {
                    return NotFound();
                }

                return Ok(exemplar);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ExemplarDto>> HardDelete(string id)
        {
            try
            {
                var exemplar = await _service.DeleteAsync(new ExemplarId(id));

                if (exemplar == null)
                {
                    return NotFound();
                }

                return Ok(exemplar);
            }
            catch(BusinessRuleValidationException ex)
            {
               return BadRequest(new {Message = ex.Message});
            }
        }
    }
}