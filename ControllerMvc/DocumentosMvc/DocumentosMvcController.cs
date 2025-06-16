using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using VoxDocs.Models.ViewModels;
using VoxDocs.DTO;
using VoxDocs.Services;
using Azure.Storage.Blobs;

namespace VoxDocs.Controllers
{
    [Authorize]
    public class DocumentosMvcController : Controller
    {
        private readonly IDocumentosPastasService _documentosService;
        private readonly IDocumentosOfflineService _documentosofflineService;
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public DocumentosMvcController(
            IDocumentosPastasService documentosService,
            IHttpClientFactory httpClientFactory,
            IUserService userService,
            IConfiguration configuration,
            BlobServiceClient blobServiceClient)
        {
            _documentosService = documentosService;
            _httpClient = httpClientFactory.CreateClient("VoxDocsApi");
            _userService = userService;
            _blobServiceClient = blobServiceClient;
            _containerName = configuration["AzureBlobStorage:ContainerName"] ?? "documentos";
        }

        [HttpGet]
        public async Task<IActionResult> Documentos(string? pastaPrincipalNome = null, string? subPastaNome = null)
        {
            Console.WriteLine($"Iniciando Documentos - pastaPrincipalNome: {pastaPrincipalNome}, subPastaNome: {subPastaNome}");

            // Obter empresa do usuário de forma consistente
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                Console.WriteLine("Usuário não identificado");
                TempData["ErrorMessage"] = "Não foi possível identificar o usuário.";
                return View(new DocumentosViewModel());
            }

            var user = await _userService.GetUserByIdAsync(userId);
            var empresaUsuario = user?.EmpresaContratante;

            if (string.IsNullOrEmpty(empresaUsuario))
            {
                Console.WriteLine("Empresa do usuário não identificada");
                TempData["ErrorMessage"] = "Não foi possível identificar a empresa do usuário.";
                return View(new DocumentosViewModel());
            }

            Console.WriteLine($"Empresa do usuário: {empresaUsuario}");

            try
            {
                Console.WriteLine("Obtendo pastas principais...");
                var pastasPrincipais = (await _documentosService.GetPastasPrincipaisByEmpresaAsync(empresaUsuario))
                    .Where(p => p != null)
                    .Select(p => new DTOPastaPrincipal 
                    { 
                        NomePastaPrincipal = p.NomePastaPrincipal,
                        EmpresaContratante = p.EmpresaContratante
                    })
                    .DistinctBy(p => p.NomePastaPrincipal)
                    .OrderBy(p => p.NomePastaPrincipal)
                    .ToList();

                Console.WriteLine($"Pastas principais encontradas: {pastasPrincipais.Count}");

                var subPastas = new List<DTOSubPasta>();
                var documentos = new List<DocumentoDto>();

                if (!string.IsNullOrEmpty(pastaPrincipalNome))
                {
                    Console.WriteLine($"Pasta principal selecionada: {pastaPrincipalNome}");
                    
                    var pastaExiste = pastasPrincipais.Any(p => 
                        p.NomePastaPrincipal.Equals(pastaPrincipalNome, StringComparison.OrdinalIgnoreCase));
                    
                    if (!pastaExiste)
                    {
                        Console.WriteLine($"Pasta principal não encontrada: {pastaPrincipalNome}");
                        TempData["ErrorMessage"] = $"Pasta principal '{pastaPrincipalNome}' não encontrada.";
                        return RedirectToAction(nameof(Documentos));
                    }

                    Console.WriteLine("Obtendo subpastas...");
                    subPastas = (await _documentosService.GetSubPastasByPastaPrincipalAsync(pastaPrincipalNome))
                        .Where(s => s != null)
                        .Select(s => new DTOSubPasta 
                        { 
                            NomeSubPasta = s.NomeSubPasta,
                            NomePastaPrincipal = s.NomePastaPrincipal
                        })
                        .DistinctBy(s => s.NomeSubPasta)
                        .OrderBy(s => s.NomeSubPasta)
                        .ToList();

                    Console.WriteLine($"Subpastas encontradas: {subPastas.Count}");

                    if (!string.IsNullOrEmpty(subPastaNome))
                    {
                        Console.WriteLine($"Subpasta selecionada: {subPastaNome}");
                        
                        var subPastaExiste = subPastas.Any(s => 
                            s.NomeSubPasta.Equals(subPastaNome, StringComparison.OrdinalIgnoreCase));
                        
                        if (!subPastaExiste)
                        {
                            Console.WriteLine($"Subpasta não encontrada: {subPastaNome}");
                            TempData["ErrorMessage"] = $"Subpasta '{subPastaNome}' não encontrada.";
                            return RedirectToAction(nameof(Documentos), new { pastaPrincipalNome });
                        }

                        Console.WriteLine("Obtendo documentos...");
                        documentos = (await _documentosService.GetDocumentosBySubPastaAsync(subPastaNome))
                            .Where(d => d != null && d.EmpresaContratante == empresaUsuario)
                            .OrderBy(d => d.NomeArquivo)
                            .ToList();

                        Console.WriteLine($"Documentos encontrados: {documentos.Count}");
                    }
                }

                var viewModel = new DocumentosViewModel
                {
                    PastaPrincipais = pastasPrincipais,
                    SelectedPastaPrincipalNome = pastaPrincipalNome,
                    SelectedSubPastaNome = subPastaNome,
                    SubPastas = subPastas,
                    Documentos = documentos
                };

                Console.WriteLine("Retornando view com modelo");
                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar documentos: {ex}");
                TempData["ErrorMessage"] = "Ocorreu um erro ao carregar os documentos.";
                return View(new DocumentosViewModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, string? token)
        {
            try
            {
                var documento = await _documentosService.GetDocumentoByIdAsync(id, token ?? string.Empty);
                await _documentosService.DeleteDocumentoAsync(id, token);
                return Json(new { success = true, message = "Documento excluído com sucesso." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro inesperado: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ValidateToken(string nomeArquivo, string? token = null)
        {
            try
            {
                bool isValid = await _documentosService.ValidateTokenDocumentoAsync(nomeArquivo, token ?? string.Empty);
                return Ok(new { sucesso = isValid });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { sucesso = false, mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPorNome(string nomeArquivo, string? token = null)
        {
            try
            {
                var result = await _documentosService.DownloadDocumentoProtegidoAsync(nomeArquivo, token ?? string.Empty);
                return File(result.stream, result.contentType, nomeArquivo);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> Edit([FromForm] DocumentoAtualizacaoDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
                return Json(new { success = false, message = "Erro de validação.", errors });
            }

            try
            {
                var userName = User.FindFirstValue(ClaimTypes.Name);
                dto.UsuarioUltimaAlteracao = userName;

                var documentoOriginal = await _documentosService.GetDocumentoByIdAsync(dto.Id, dto.TokenSeguranca);

                if (documentoOriginal.NivelSeguranca == NivelSeguranca.Confidencial && !User.HasClaim("PermissionAccount", "admin"))
                {
                    return Json(new { success = false, message = "Apenas administradores podem editar documentos confidenciais." });
                }

                await _documentosService.UpdateDocumentoAsync(dto);
                return Json(new { success = true, message = "Documento atualizado com sucesso." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Erro ao atualizar documento: " + ex.Message });
            }
        }

        [HttpGet("offline")]
        public async Task<IActionResult> DocumentosOffline()
        {
            var empresaUsuario = User.FindFirst("Empresa")?.Value;
            if (string.IsNullOrEmpty(empresaUsuario))
            {
                TempData["ErrorMessage"] = "Não foi possível identificar a empresa do usuário.";
                return View(new DocumentosViewModel());
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var documentos = await _documentosofflineService.GetCachedUserDocumentsAsync(userId);

            if (!documentos.Any())
            {
                TempData["WarningMessage"] = "Nenhum documento disponível para modo offline. Conecte-se à internet e clique em 'Preparar Offline'.";
            }

            return View("Documentos", new DocumentosViewModel
            {
                Documentos = documentos,
                IsOfflineMode = true
            });
        }

        [HttpPost("prepare-offline")]
        public async Task<IActionResult> PrepareForOffline()
        {
            var empresaUsuario = User.FindFirst("Empresa")?.Value;
            if (string.IsNullOrEmpty(empresaUsuario))
            {
                return Json(new { success = false, message = "Empresa não identificada." });
            }

            try
            {
                var documentos = await _documentosofflineService.GetDocumentsForOfflineAsync(empresaUsuario, User);
                
                return Json(new { 
                    success = true, 
                    message = $"Documentos preparados para modo offline com sucesso! {documentos.Count()} documentos disponíveis.",
                    count = documentos.Count()
                });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Erro ao preparar documentos offline." });
            }
        }
    }
}