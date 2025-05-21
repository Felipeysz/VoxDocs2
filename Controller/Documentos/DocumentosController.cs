using Microsoft.AspNetCore.Mvc;
using VoxDocs.Models;
using VoxDocs.Services;
using VoxDocs.DTO;
using VoxDocs.BusinessRules;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace VoxDocs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentosController : ControllerBase
    {
        private readonly IDocumentoService _service;
        private readonly AzureBlobService _blobService;
        private readonly DocumentoBusinessRules _rules;

        public DocumentosController(
            IDocumentoService service, 
            AzureBlobService blobService,
            DocumentoBusinessRules rules)
        {
            _service = service;
            _blobService = blobService;
            _rules = rules;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] DocumentoDto dto)
        {
            try
            {
                await _rules.ValidateDocumentoUploadAsync(dto);

                // Chama a Service para criar o documento
                var createdDoc = await _service.CreateAsync(dto);
                return Ok(createdDoc);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log do erro para depuração
                Console.WriteLine($"Erro ao realizar o upload: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return StatusCode(500, new { message = "Erro ao realizar o upload", detalhes = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, [FromQuery] string? token)
        {
            try
            {
                await _rules.CheckDocumentoExistsAsync(id);
                var doc = await _service.GetByIdAsync(id, token);
                await _service.IncrementarAcessoAsync(id);
                return Ok(doc);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor", detalhes = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var docs = await _service.GetAllAsync();
                return Ok(docs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor", detalhes = ex.Message });
            }
        }

        [HttpGet("subpasta/{subPasta}")]
        public async Task<IActionResult> GetBySubPasta(string subPasta)
        {
            try
            {
                var docs = await _service.GetBySubPastaAsync(subPasta);
                return Ok(docs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor", detalhes = ex.Message });
            }
        }

        [HttpGet("pastaprincipal/{pastaPrincipal}")]
        public async Task<IActionResult> GetByPastaPrincipal(string pastaPrincipal)
        {
            try
            {
                var docs = await _service.GetByPastaPrincipalAsync(pastaPrincipal);
                return Ok(docs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor", detalhes = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _rules.ValidateDocumentoDeleteAsync(id);
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor", detalhes = ex.Message });
            }
        }

        [HttpGet("estatisticas/{empresa}")]
        public async Task<IActionResult> GetEstatisticasEmpresa(string empresa)
        {
            try
            {
                var stats = await _service.GetEstatisticasEmpresaAsync(empresa);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor", detalhes = ex.Message });
            }
        }

        [HttpGet("acessos/{id}/{dias}")]
        public async Task<IActionResult> GetAcessosDocumento(int id, int dias)
        {
            try
            {
                var acessos = await _service.GetAcessosDocumentoAsync(id, dias);
                if (acessos == null) return NotFound();
                return Ok(acessos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor", detalhes = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DocumentoModel documento)
        {
            try
            {
                await _rules.CheckDocumentoExistsAsync(id);
                await _rules.ValidateDocumentoUpdateAsync(documento);

                var existingDoc = await _service.GetByIdAsync(id);
                if (existingDoc == null) return NotFound();

                // Preserve campos imutáveis
                documento.Id = id;
                documento.DataCriacao = existingDoc.DataCriacao;
                documento.UsuarioCriador = existingDoc.UsuarioCriador;

                // 1) Monta o DTO esperado pelo serviço
                var dtoUpdate = new DTODocumentoCreate
                {
                    Id = documento.Id,
                    NomeArquivo = documento.NomeArquivo,
                    UrlArquivo = documento.UrlArquivo,
                    UsuarioCriador = documento.UsuarioCriador,
                    DataCriacao = documento.DataCriacao,
                    UsuarioUltimaAlteracao = documento.UsuarioUltimaAlteracao,
                    DataUltimaAlteracao = documento.DataUltimaAlteracao,
                    Empresa = documento.Empresa,
                    NomePastaPrincipal = documento.NomePastaPrincipal,
                    NomeSubPasta = documento.NomeSubPasta,
                    TamanhoArquivo = documento.TamanhoArquivo,
                    NivelSeguranca = documento.NivelSeguranca,
                    TokenSeguranca = documento.TokenSeguranca,
                    Descrição = documento.Descrição
                };

                // 2) Passa o DTO para o serviço
                var updatedDoc = await _service.UpdateAsync(dtoUpdate);
                return Ok(updatedDoc);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno no servidor", detalhes = ex.Message });
            }
        }

    }
}