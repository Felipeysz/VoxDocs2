using Microsoft.AspNetCore.Mvc;
using VoxDocs.DTO;
using VoxDocs.Services;

namespace VoxDocs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PastaPrincipalController : ControllerBase
    {
        private readonly IDocumentosPastasService _service;

        public PastaPrincipalController(IDocumentosPastasService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTOPastaPrincipal>>> GetAll()
        {
            var pastas = await _service.GetAllPastasPrincipaisAsync();
            return Ok(pastas);
        }

        [HttpGet("nome/{nomePasta}")]
        public async Task<ActionResult<DTOPastaPrincipal>> GetByNamePrincipal(string nomePasta)
        {
            try
            {
                var pasta = await _service.GetPastaPrincipalByNameAsync(nomePasta);
                return Ok(pasta);
            }
            catch (DocumentosPastasService.CustomException ex) when (ex.StatusCode == 404)
            {
                return NotFound(ex.Message);
            }
            catch (DocumentosPastasService.CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet("empresa/{empresaContratante}")]
        public async Task<ActionResult<IEnumerable<DTOPastaPrincipal>>> GetByEmpresa(string empresaContratante)
        {
            try
            {
                var pastas = await _service.GetPastasPrincipaisByEmpresaAsync(empresaContratante);
                return Ok(pastas);
            }
            catch (DocumentosPastasService.CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DTOPastaPrincipal>> GetById(Guid id)
        {
            var pasta = await _service.GetPastaPrincipalByIdAsync(id);
            return pasta != null ? Ok(pasta) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<DTOPastaPrincipal>> Create(DTOPastaPrincipalCreate dto)
        {
            try
            {
                var created = await _service.CreatePastaPrincipalAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (DocumentosPastasService.CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _service.DeletePastaPrincipalAsync(id);
                return result ? NoContent() : NotFound();
            }
            catch (DocumentosPastasService.CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }
    }
}