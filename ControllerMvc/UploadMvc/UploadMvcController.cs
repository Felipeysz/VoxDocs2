using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using VoxDocs.DTO;
using VoxDocs.Models.ViewModels;
using VoxDocs.Services;

namespace VoxDocs.Controllers
{
[Authorize]
public class UploadMvcController : Controller
{
    private readonly IDocumentoService _documentoService;
    private readonly IPastaPrincipalService _pastaService;
    private readonly ISubPastaService _subPastaService;
    private readonly IUserService _userService;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public UploadMvcController(
        IDocumentoService documentoService,
        IPastaPrincipalService pastaService,
        ISubPastaService subPastaService,
        IUserService userService,
        IConfiguration configuration,
        BlobServiceClient blobServiceClient)
    {
        _documentoService = documentoService;
        _pastaService = pastaService;
        _subPastaService = subPastaService;
        _userService = userService;
        _blobServiceClient = blobServiceClient;
        _containerName = configuration["AzureBlobStorage:ContainerName"] ?? "voxdocuments";
    }

        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Error", "Home");

            if (!Guid.TryParse(userIdClaim, out Guid userId))
                return RedirectToAction("Error", "Home");

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return RedirectToAction("Error", "Home");

            var vm = new UploadDocumentoViewModel
            {
                PastaPrincipais = await _pastaService.GetByEmpresaAsync(user.EmpresaContratante ?? string.Empty),
                SubPastas = await _subPastaService.GetByEmpresaAsync(user.EmpresaContratante ?? string.Empty),
                Descricao = string.Empty,
                Arquivo = null!
            };

            ViewBag.Usuario = user.Usuario;
            ViewBag.Empresa = user.EmpresaContratante;
            ViewBag.IsAdmin = User.HasClaim("PermissionAccount", "admin");
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] UploadDocumentoViewModel vm)
        {
            // Validação do usuário
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Json(new ResultadoOperacaoDto { Sucesso = false, Mensagem = "Usuário não autenticado." });

            if (!Guid.TryParse(userIdClaim, out Guid userId))
                return Json(new ResultadoOperacaoDto { Sucesso = false, Mensagem = "ID de usuário inválido." });

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return Json(new ResultadoOperacaoDto { Sucesso = false, Mensagem = "Usuário não encontrado." });

            // Validação do modelo
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();
                return Json(new ResultadoOperacaoDto 
                { 
                    Sucesso = false, 
                    Mensagem = "Erro de validação.",
                    Dados = new { errors }
                });
            }

            // Validação de nível de segurança
            if (vm.NivelSeguranca != NivelSeguranca.Publico.ToString() && string.IsNullOrWhiteSpace(vm.TokenSeguranca))
            {
                return Json(new ResultadoOperacaoDto
                {
                    Sucesso = false,
                    Mensagem = "Token de segurança obrigatório para nível Restrito ou Confidencial.",
                    Dados = new { errors = new[] { "Token de Segurança é obrigatório." } }
                });
            }

            if (vm.NivelSeguranca == NivelSeguranca.Confidencial.ToString() && !User.HasClaim("PermissionAccount", "admin"))
            {
                return Json(new ResultadoOperacaoDto 
                { 
                    Sucesso = false, 
                    Mensagem = "Apenas admins podem criar documentos confidenciais." 
                });
            }

            try
            {
                // Validação das pastas
                var pasta = await _pastaService.GetByIdAsync(vm.SelectedPastaPrincipalId);
                var sub = await _subPastaService.GetByIdAsync(vm.SelectedSubPastaId);

                if (pasta == null || sub == null ||
                    pasta.EmpresaContratante != (user.EmpresaContratante ?? string.Empty) ||
                    sub.EmpresaContratante != (user.EmpresaContratante ?? string.Empty))
                {
                    return Json(new ResultadoOperacaoDto 
                    { 
                        Sucesso = false, 
                        Mensagem = "Categoria ou subcategoria inválida." 
                    });
                }

                // Generate unique file name
                var originalFileName = vm.Arquivo.FileName;
                var fileExtension = Path.GetExtension(originalFileName);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
                var newFileName = $"{fileNameWithoutExtension}_{Guid.NewGuid()}{fileExtension}";

                // Upload do documento
                using var ms = new MemoryStream();
                await vm.Arquivo.CopyToAsync(ms);
                ms.Position = 0;

                var dto = new DocumentoCriacaoDto
                {
                    Arquivo = new FormFile(ms, 0, ms.Length, vm.Arquivo.Name, newFileName)
                    {
                        Headers = vm.Arquivo.Headers,
                        ContentType = vm.Arquivo.ContentType
                    },
                    NomePastaPrincipal = pasta.NomePastaPrincipal,
                    NomeSubPasta = sub.NomeSubPasta,
                    NivelSeguranca = Enum.Parse<NivelSeguranca>(vm.NivelSeguranca ?? "Publico"),
                    TokenSeguranca = vm.TokenSeguranca ?? string.Empty,
                    Descricao = vm.Descricao,
                    Usuario = user.Usuario,
                    EmpresaContratante = user.EmpresaContratante ?? string.Empty
                };

                // Call service and construct ResultadoOperacaoDto manually
                await _documentoService.CreateAsync(dto);

                return Json(new ResultadoOperacaoDto
                {
                    Sucesso = true,
                    Mensagem = "Upload realizado com sucesso!",
                    Dados = new { fileName = newFileName }
                });
            }
            catch (Exception ex)
            {
                return Json(new ResultadoOperacaoDto 
                { 
                    Sucesso = false, 
                    Mensagem = $"Erro ao fazer upload: {ex.Message}" 
                });
            }
        }
    }
}