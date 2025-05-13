using Microsoft.AspNetCore.Mvc;
using VoxDocs.Models.Dto;
using VoxDocs.Services;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VoxDocs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AreasDocumentoController : ControllerBase
    {
        private readonly IAreasDocumentoService _service;

        public AreasDocumentoController(IAreasDocumentoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTOAreasDocumentos>>> GetAll()
        {
            var dtos = await _service.GetAllAsync();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DTOAreasDocumentos>> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<DTOAreasDocumentos>> Create([FromBody] DTOAreasDocumentos dto)
        {
            var createdDto = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DTOAreasDocumentos>> Update(int id, [FromBody] DTOAreasDocumentos dto)
        {
            if (id != dto.Id) return BadRequest();
            var updatedDto = await _service.UpdateAsync(dto);
            if (updatedDto == null) return NotFound();
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