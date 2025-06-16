using Microsoft.AspNetCore.Mvc;
using VoxDocs.DTO;
using VoxDocs.Services;

namespace VoxDocs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubPastaController : ControllerBase
    {
        private readonly IDocumentosPastasService _service;

        public SubPastaController(IDocumentosPastasService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTOSubPasta>>> GetAll()
        {
            var subPastas = await _service.GetAllSubPastasAsync();
            return Ok(subPastas);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DTOSubPasta>> GetById(Guid id)
        {
            var subPasta = await _service.GetSubPastaByIdAsync(id);
            if (subPasta == null) return NotFound();
            return Ok(subPasta);
        }

        [HttpGet("empresa/{empresa}")]
        public async Task<ActionResult<IEnumerable<DTOSubPasta>>> GetByEmpresa(string empresa)
        {
            var subPastas = await _service.GetSubPastasByEmpresaAsync(empresa);
            return Ok(subPastas);
        }

        [HttpGet("nome/{nomeSubPasta}")]
        public async Task<ActionResult<DTOSubPasta>> GetByNameSubPasta(string nomeSubPasta)
        {
            var subPasta = await _service.GetSubPastaByNameAsync(nomeSubPasta);
            if (subPasta == null) return NotFound();
            return Ok(subPasta);
        }

        [HttpPost]
        public async Task<ActionResult<DTOSubPasta>> Create(DTOSubPastaCreate dto)
        {
            var created = await _service.CreateSubPastaAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteSubPastaAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpGet("subchildren/{nomePastaPrincipal}")]
        public async Task<ActionResult<IEnumerable<DTOSubPasta>>> GetSubChildren(string nomePastaPrincipal)
        {
            var subPastas = await _service.GetSubPastasByPastaPrincipalAsync(nomePastaPrincipal);
            return Ok(subPastas);
        }
    }
}