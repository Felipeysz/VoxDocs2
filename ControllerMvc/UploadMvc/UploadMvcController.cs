using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
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
        private readonly IDocumentosPastasService _documentoService;
        private readonly IUserService _userService;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public UploadMvcController(
            IDocumentosPastasService documentoService,
            IUserService userService,
            IConfiguration configuration,
            BlobServiceClient blobServiceClient)
        {
            _documentoService = documentoService;
            _userService = userService;
            _blobServiceClient = blobServiceClient;
            _containerName = configuration["AzureBlobStorage:ContainerName"] ?? "voxdocuments";
        }

        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                return RedirectToAction("Error", "Home");

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return RedirectToAction("Error", "Home");

            var pastasPrincipais = await _documentoService.GetPastasPrincipaisByEmpresaAsync(user.EmpresaContratante ?? string.Empty);
            var subPastas = await _documentoService.GetSubPastasByEmpresaAsync(user.EmpresaContratante ?? string.Empty);

            var vm = new DocumentoCreateViewModel
            {
                NivelSeguranca = NivelSeguranca.Publico.ToString()
            };

            ViewBag.PastaPrincipais = pastasPrincipais;
            ViewBag.SubPastas = subPastas;
            ViewBag.Usuario = user.Usuario;
            ViewBag.Empresa = user.EmpresaContratante;
            ViewBag.IsAdmin = User.HasClaim("PermissionAccount", "admin");
            
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(DocumentoCreateViewModel vm)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                return Unauthorized();

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                var pastasPrincipais = await _documentoService.GetPastasPrincipaisByEmpresaAsync(user.EmpresaContratante ?? string.Empty);
                var subPastas = await _documentoService.GetSubPastasByEmpresaAsync(user.EmpresaContratante ?? string.Empty);

                ViewBag.PastaPrincipais = pastasPrincipais;
                ViewBag.SubPastas = subPastas;
                ViewBag.Usuario = user.Usuario;
                ViewBag.Empresa = user.EmpresaContratante;
                ViewBag.IsAdmin = User.HasClaim("PermissionAccount", "admin");
                
                return View(vm);
            }

            try
            {
                var pasta = await _documentoService.GetPastaPrincipalByIdAsync(vm.SelectedPastaPrincipalId);
                var subPasta = await _documentoService.GetSubPastaByIdAsync(vm.SelectedSubPastaId);

                if (pasta == null || subPasta == null || 
                    pasta.EmpresaContratante != user.EmpresaContratante || 
                    subPasta.EmpresaContratante != user.EmpresaContratante)
                {
                    ModelState.AddModelError("", "Categoria ou subcategoria inválida.");
                    return View(vm);
                }

                if (vm.NivelSeguranca == NivelSeguranca.Confidencial.ToString() && !User.HasClaim("PermissionAccount", "admin"))
                {
                    ModelState.AddModelError("", "Apenas administradores podem criar documentos confidenciais.");
                    return View(vm);
                }

                var dto = new DocumentoCriacaoDto
                {
                    Arquivo = vm.Arquivo,
                    NomePastaPrincipal = pasta.NomePastaPrincipal,
                    NomeSubPasta = subPasta.NomeSubPasta,
                    NivelSeguranca = Enum.Parse<NivelSeguranca>(vm.NivelSeguranca),
                    TokenSeguranca = vm.TokenSeguranca,
                    Descricao = vm.Descricao,
                    Usuario = user.Usuario,
                    EmpresaContratante = user.EmpresaContratante ?? string.Empty
                };

                await _documentoService.CreateDocumentoAsync(dto);

                TempData["SuccessMessage"] = "Documento criado com sucesso!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao criar documento: {ex.Message}");
                
                var pastasPrincipais = await _documentoService.GetPastasPrincipaisByEmpresaAsync(user.EmpresaContratante ?? string.Empty);
                var subPastas = await _documentoService.GetSubPastasByEmpresaAsync(user.EmpresaContratante ?? string.Empty);

                ViewBag.PastaPrincipais = pastasPrincipais;
                ViewBag.SubPastas = subPastas;
                ViewBag.Usuario = user.Usuario;
                ViewBag.Empresa = user.EmpresaContratante;
                ViewBag.IsAdmin = User.HasClaim("PermissionAccount", "admin");
                
                return View(vm);
            }
        }
    }
}