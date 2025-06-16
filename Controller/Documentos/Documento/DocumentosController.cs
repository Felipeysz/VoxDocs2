using Microsoft.AspNetCore.Mvc;
using VoxDocs.Services;
using VoxDocs.DTO;
using System;
using System.Threading.Tasks;

namespace VoxDocs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentosController : ControllerBase
    {
        private readonly IDocumentosPastasService _service;

        public DocumentosController(IDocumentosPastasService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] DocumentoCriacaoDto dto)
        {
            try
            {
                if (dto.Arquivo == null || dto.Arquivo.Length == 0)
                    return BadRequest("Arquivo não fornecido ou vazio");

                var createdDoc = await _service.CreateDocumentoAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = createdDoc.Id }, createdDoc);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.ParamName + " é obrigatório" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Erro ao criar documento", 
                    details = ex.Message 
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id, [FromQuery] string token = null)
        {
            try
            {
                var doc = await _service.GetDocumentoByIdAsync(id, token);
                if (doc == null)
                    return NotFound();

                await _service.IncrementarAcessoDocumentoAsync(id);
                return Ok(doc);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Token inválido ou não fornecido");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Erro ao buscar documento", 
                    details = ex.Message 
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var docs = await _service.GetAllDocumentosAsync();
                return Ok(docs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Erro ao listar documentos", 
                    details = ex.Message 
                });
            }
        }

        [HttpGet("pasta/{pastaPrincipal}")]
        public async Task<IActionResult> GetByPastaPrincipal(string pastaPrincipal)
        {
            try
            {
                var docs = await _service.GetDocumentosByPastaPrincipalAsync(pastaPrincipal);
                return Ok(docs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Erro ao buscar documentos por pasta", 
                    details = ex.Message 
                });
            }
        }

        [HttpGet("subpasta/{subPasta}")]
        public async Task<IActionResult> GetBySubPasta(string subPasta)
        {
            try
            {
                var docs = await _service.GetDocumentosBySubPastaAsync(subPasta);
                return Ok(docs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Erro ao buscar documentos por subpasta", 
                    details = ex.Message 
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] DocumentoAtualizacaoDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest("ID na rota não corresponde ao ID no DTO");

                var updatedDoc = await _service.UpdateDocumentoAsync(dto);
                return Ok(updatedDoc);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Token inválido ou não fornecido");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Erro ao atualizar documento", 
                    details = ex.Message 
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, [FromQuery] string token = null)
        {
            try
            {
                await _service.DeleteDocumentoAsync(id, token);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Token inválido ou não fornecido");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Erro ao excluir documento", 
                    details = ex.Message 
                });
            }
        }

        [HttpGet("download/{nomeArquivo}")]
        public async Task<IActionResult> Download(string nomeArquivo, [FromQuery] string token = null)
        {
            try
            {
                var (stream, contentType) = await _service.DownloadDocumentoProtegidoAsync(nomeArquivo, token);
                return File(stream, contentType, nomeArquivo);
            }
            catch (FileNotFoundException)
            {
                return NotFound("Arquivo não encontrado");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Token inválido ou não fornecido");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Erro ao baixar documento", 
                    details = ex.Message 
                });
            }
        }

        [HttpGet("estatisticas/{empresa}")]
        public async Task<IActionResult> GetEstatisticas(string empresa)
        {
            try
            {
                var stats = await _service.GetEstatisticasEmpresaAsync(empresa);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Erro ao buscar estatísticas", 
                    details = ex.Message 
                });
            }
        }

        [HttpGet("validar-token")]
        public async Task<IActionResult> ValidateToken([FromQuery] string nomeArquivo, [FromQuery] string token)
        {
            try
            {
                var isValid = await _service.ValidateTokenDocumentoAsync(nomeArquivo, token);
                return Ok(new { valid = isValid });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Erro ao validar token", 
                    details = ex.Message 
                });
            }
        }
    }
}