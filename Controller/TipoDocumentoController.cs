using Microsoft.AspNetCore.Mvc;
using VoxDocs.Models.Dto;
using VoxDocs.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VoxDocs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoDocumentoController : ControllerBase
    {
        private readonly ITipoDocumentoService _service;

        public TipoDocumentoController(ITipoDocumentoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTOTipoDocumento>>> GetAll()
        {
            var tipos = await _service.GetAllAsync();
            var dtos = new List<DTOTipoDocumento>();
            foreach (var t in tipos)
            {
                dtos.Add(new DTOTipoDocumento
                {
                    Id = t.Id,
                    Nome = t.Nome
                });
            }
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DTOTipoDocumento>> GetById(int id)
        {
            var tipo = await _service.GetByIdAsync(id);
            if (tipo == null) return NotFound();
            var dto = new DTOTipoDocumento
            {
                Id = tipo.Id,
                Nome = tipo.Nome
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<DTOTipoDocumento>> Create([FromBody] DTOTipoDocumento dto)
        {
            var tipo = new VoxDocs.Models.TipoDocumentoModel
            {
                Nome = dto.Nome
            };
            var created = await _service.CreateAsync(tipo);
            var createdDto = new DTOTipoDocumento
            {
                Id = created.Id,
                Nome = created.Nome
            };
            return CreatedAtAction(nameof(GetById), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DTOTipoDocumento>> Update(int id, [FromBody] DTOTipoDocumento dto)
        {
            if (id != dto.Id) return BadRequest();
            var tipo = new VoxDocs.Models.TipoDocumentoModel
            {
                Id = dto.Id,
                Nome = dto.Nome
            };
            var updated = await _service.UpdateAsync(tipo);
            if (updated == null) return NotFound();
            var updatedDto = new DTOTipoDocumento
            {
                Id = updated.Id,
                Nome = updated.Nome
            };
            return Ok(updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}