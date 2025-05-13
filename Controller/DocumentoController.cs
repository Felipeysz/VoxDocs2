using Microsoft.AspNetCore.Mvc;
using VoxDocs.Models;
using VoxDocs.DTO;
using VoxDocs.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace VoxDocs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentoController : ControllerBase
    {
        private readonly IDocumentoService _service;

        public DocumentoController(IDocumentoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTODocumento>>> GetAll()
        {
            var docs = await _service.GetAllAsync();
            var dtos = docs.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DTODocumento>> GetById(int id)
        {
            var doc = await _service.GetByIdAsync(id);
            if (doc == null) return NotFound();
            return Ok(MapToDto(doc));
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<DTODocumento>>> GetByFilter(
            [FromQuery] int areaDocumentoId,
            [FromQuery] int tipoDocumentoId)
        {
            var docs = await _service.GetAllAsync();
            var filtrados = docs
                .Where(d => d.AreaDocumentoId == areaDocumentoId
                         && d.TipoDocumentoId == tipoDocumentoId)
                .Select(MapToDto)
                .ToList();
            return Ok(filtrados);
        }

        [HttpPost]
        public async Task<ActionResult<DTODocumento>> Create([FromBody] DTODocumento dto)
        {
            var model = new DocumentoModel
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                AreaDocumentoId = dto.AreaDocumentoId,
                TipoDocumentoId = dto.TipoDocumentoId,
                DocumentoUploadId = dto.DocumentoUploadId,
                UsuarioCriador = dto.UsuarioCriador,
                DataCriacao = dto.DataCriacao,
                UsuarioUltimaAlteracao = dto.UsuarioUltimaAlteracao,
                DataAtualizacao = dto.DataAtualizacao
            };
            var created = await _service.CreateAsync(model);
            dto.Id = created.Id;
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DTODocumento>> Update(int id, [FromBody] DTODocumento dto)
        {
            if (id != dto.Id) return BadRequest();
            var model = new DocumentoModel
            {
                Id = dto.Id,
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                AreaDocumentoId = dto.AreaDocumentoId,
                TipoDocumentoId = dto.TipoDocumentoId,
                DocumentoUploadId = dto.DocumentoUploadId,
                UsuarioCriador = dto.UsuarioCriador,
                DataCriacao = dto.DataCriacao,
                UsuarioUltimaAlteracao = dto.UsuarioUltimaAlteracao,
                DataAtualizacao = dto.DataAtualizacao
            };
            var updated = await _service.UpdateAsync(model);
            if (updated == null) return NotFound();
            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        private static DTODocumento MapToDto(DocumentoModel doc) => new DTODocumento
        {
            Id = doc.Id,
            Nome = doc.Nome,
            Descricao = doc.Descricao,
            AreaDocumentoId = doc.AreaDocumentoId,
            TipoDocumentoId = doc.TipoDocumentoId,
            DocumentoUploadId = doc.DocumentoUploadId,
            UsuarioCriador = doc.UsuarioCriador,
            DataCriacao = doc.DataCriacao,
            UsuarioUltimaAlteracao = doc.UsuarioUltimaAlteracao,
            DataAtualizacao = doc.DataAtualizacao
        };
    }
}