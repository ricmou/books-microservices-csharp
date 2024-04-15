using System.Collections.Generic;
using System.Threading.Tasks;
using APIClients.Domain.Clients;
using APIClients.Domain.Shared;
using APIClients.Services;
using Microsoft.AspNetCore.Mvc;

namespace APIClients.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _service;

        public ClientsController(IClientsService service)
        {
            _service = service;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientDto>>> GetAll()
        {
            return await _service.GetAllAsync();
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ClientDto>> GetGetById(string id)
        {
            var client = await _service.GetByIdAsync(new ClientId(id));

            if (client == null)
            {
                return NotFound();
            }

            return client;
        }

        // POST: api/Books
        [HttpPost]
        public async Task<ActionResult<ClientDto>> Create(CreatingClientDto dto)
        {
            var client = await _service.AddAsync(dto);

            //return CreatedAtAction(nameof(GetGetById), new { id = client.ClientId }, client);

            return client;
        }

        
        // PUT: api/Books/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ClientDto>> Update(string id, ClientDto dto)
        {
            if (id != dto.ClientId)
            {
                return BadRequest();
            }

            try
            {
                var client = await _service.UpdateAsync(dto);

                if (client == null)
                {
                    return NotFound();
                }

                return Ok(client);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ClientDto>> HardDelete(string id)
        {
            try
            {
                var client = await _service.DeleteAsync(new ClientId(id));

                if (client == null)
                {
                    return NotFound();
                }

                return Ok(client);
            }
            catch(BusinessRuleValidationException ex)
            {
               return BadRequest(new {Message = ex.Message});
            }
        }
    }
}