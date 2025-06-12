using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
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
        public async Task<IActionResult> UploadDocumentos()
        {
            // 🟢 Obter o ID do usuário autenticado das claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return RedirectToAction("Error", "Home");

            // 🟢 Converter o ID para inteiro
            if (!int.TryParse(userIdClaim, out int userId))
                return RedirectToAction("Error", "Home");

            // 🟢 Buscar o usuário pelo ID
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return RedirectToAction("Error", "Home");

            // 🟢 Continuar com a lógica existente
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
        public async Task<JsonResult> UploadDocumentos([FromForm] UploadDocumentoViewModel vm)
        {
            // 🟢 Obter o ID do usuário autenticado das claims
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return Json(new { success = false, message = "Usuário não autenticado." });

            // 🟢 Converter o ID para inteiro
            if (!int.TryParse(userIdClaim, out int userId))
                return Json(new { success = false, message = "ID de usuário inválido." });

            // 🟢 Buscar o usuário pelo ID
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return Json(new { success = false, message = "Usuário não encontrado." });

            // 🟢 Continuar com a lógica existente
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToArray();
                return Json(new { success = false, message = "Erro de validação.", errors });
            }

            if (vm.NivelSeguranca != "Publico" && string.IsNullOrWhiteSpace(vm.TokenSeguranca))
            {
                return Json(new
                {
                    success = false,
                    message = "Token de segurança obrigatório para nível Restrito ou Confidencial.",
                    errors = new[] { "Token de Segurança é obrigatório." }
                });
            }

            if (vm.NivelSeguranca == "Confidencial" &&
                !User.HasClaim("PermissionAccount", "admin"))
            {
                return Json(new
                {
                    success = false,
                    message = "Apenas admins podem criar documentos confidenciais."
                });
            }

            try
            {
                var pasta = await _pastaService.GetByIdAsync(vm.SelectedPastaPrincipalId);
                var sub = await _subPastaService.GetByIdAsync(vm.SelectedSubPastaId);

                if (pasta == null || sub == null ||
                    pasta.EmpresaContratante != (user.EmpresaContratante ?? string.Empty) ||
                    sub.EmpresaContratante != (user.EmpresaContratante ?? string.Empty))
                {
                    return Json(new { success = false, message = "Categoria ou subcategoria inválida." });
                }

                var empresaPrefix = (user.EmpresaContratante ?? "EMP").Length >= 3 
                    ? (user.EmpresaContratante ?? "EMP").Substring(0, 3).ToUpper() 
                    : "EMP";
                var pastaPrefix = pasta.NomePastaPrincipal.Substring(0, 3).ToUpper();
                var datePrefix = DateTime.UtcNow.ToString("ddMMyyyy");
                var subPrefix = sub.NomeSubPasta.Substring(0, 3).ToUpper();
                var nomeOriginal = Path.GetFileNameWithoutExtension(vm.Arquivo.FileName);

                var ultimoNome = nomeOriginal.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? nomeOriginal;
                var ext = Path.GetExtension(vm.Arquivo.FileName);
                var name = $"{empresaPrefix}_{pastaPrefix}{datePrefix}{subPrefix}_{ultimoNome}{ext}";

                // ✅ Verificação de duplicidade (sem considerar a data)
                var baseNameWithoutDate = Regex.Replace(name, @"\d{8}", ""); // Remove a data de 8 dígitos
                var baseNameWithoutDateAndExtension = Path.GetFileNameWithoutExtension(baseNameWithoutDate);
                var extension = Path.GetExtension(name);

                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                bool blobExists = false;

                await foreach (var blob in containerClient.GetBlobsAsync())
                {
                    var blobName = blob.Name;
                    var blobBaseName = Path.GetFileNameWithoutExtension(blobName);
                    var blobExt = Path.GetExtension(blobName);

                    if (blobBaseName == baseNameWithoutDateAndExtension && blobExt == extension)
                    {
                        blobExists = true;
                        break;
                    }
                }

                if (blobExists)
                {
                    return Json(new 
                    { 
                        success = false, 
                        message = "Já existe um documento com esse nome. Por favor, renomeie o arquivo e tente novamente." 
                    });
                }

                using var ms = new MemoryStream();
                await vm.Arquivo.CopyToAsync(ms);
                ms.Position = 0;

                var dto = new DocumentoDto
                {
                    Arquivo = new FormFile(ms, 0, ms.Length, vm.Arquivo.Name, name)
                    {
                        Headers = vm.Arquivo.Headers,
                        ContentType = vm.Arquivo.ContentType
                    },
                    NomePastaPrincipal = pasta.NomePastaPrincipal,
                    NomeSubPasta = sub.NomeSubPasta,
                    NivelSeguranca = vm.NivelSeguranca ?? "Publico",
                    TokenSeguranca = vm.TokenSeguranca ?? string.Empty,
                    Descrição = vm.Descricao,
                    Usuario = user.Usuario,
                    EmpresaContratante = user.EmpresaContratante ?? string.Empty
                };

                await _documentoService.CreateAsync(dto);
                return Json(new { success = true, message = "Upload realizado com sucesso!" });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("BlobAlreadyExists") || ex.Message.Contains("already exists"))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Já existe um documento com esse nome. Por favor, renomeie o arquivo e tente novamente."
                    });
                }

                return Json(new { success = false, message = "Erro ao fazer upload: " + ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<JsonResult> GetSubPastasByPastaPrincipal(string nomePastaPrincipal)
        {
            var subPastas = await _subPastaService.GetSubChildrenAsync(nomePastaPrincipal);
            var result = subPastas.Select(s => new { s.Id, s.NomeSubPasta }).ToList();
            return Json(result);
        }
    }
}