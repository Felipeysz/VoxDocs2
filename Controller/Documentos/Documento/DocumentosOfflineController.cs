using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using VoxDocs.DTO;
using VoxDocs.Services;

namespace VoxDocs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentosOfflineController : ControllerBase
    {
        private readonly IDocumentosOfflineService _service;
        private readonly ILogger<DocumentosOfflineController> _logger;

        public DocumentosOfflineController(
            IDocumentosOfflineService service,
            ILogger<DocumentosOfflineController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("precache")]
        public async Task<IActionResult> PrecacheDocuments()
        {
            try
            {
                var empresa = User.FindFirst("Empresa")?.Value;
                if (string.IsNullOrEmpty(empresa))
                {
                    return BadRequest("Não foi possível identificar a empresa do usuário.");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var documents = await _service.GetDocumentsForOfflineAsync(empresa, User);
                await _service.CacheUserDocumentsAsync(userId, documents);

                _logger.LogInformation($"Pré-cache de documentos offline realizado para usuário {userId}");
                return Ok(new { Count = documents.Count() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar pré-cache de documentos offline");
                return StatusCode(500, "Erro ao preparar documentos para modo offline");
            }
        }

        [HttpGet("cached")]
        public async Task<IActionResult> GetCachedDocuments()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var documents = await _service.GetCachedUserDocumentsAsync(userId);

                return Ok(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter documentos em cache");
                return StatusCode(500, "Erro ao recuperar documentos offline");
            }
        }

        [HttpGet("validate/{documentId}")]
        public async Task<IActionResult> ValidateOfflineAccess(Guid documentId)
        {
            try
            {
                var hasAccess = await _service.ValidateDocumentForOfflineAccess(documentId, User);
                return Ok(new { HasAccess = hasAccess });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao validar acesso offline para documento {documentId}");
                return StatusCode(500, "Erro ao validar acesso offline");
            }
        }
    }
}