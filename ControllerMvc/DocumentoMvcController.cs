using Microsoft.AspNetCore.Mvc;
using VoxDocs.Services;
using VoxDocs.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace VoxDocs.Controllers
{
    [Authorize]
    public class DocumentosMvcController : Controller
    {
        private readonly IDocumentoService _documentoService;
        private readonly AzureBlobService _blobService;
        private readonly IPastaPrincipalService _pastaPrincipalService;
        private readonly ISubPastaService _subPastaService;
        private readonly IUserService _userService;

        public DocumentosMvcController(
            IDocumentoService documentoService,
            AzureBlobService blobService,
            IPastaPrincipalService pastaPrincipalService,
            ISubPastaService subPastaService,
            IUserService userService)
        {
            _documentoService = documentoService;
            _blobService = blobService;
            _pastaPrincipalService = pastaPrincipalService;
            _subPastaService = subPastaService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> UploadDocumentos()
        {
            var user = HttpContext.User;
            bool isAdmin = user.Identity?.IsAuthenticated ?? false && user.HasClaim("PermissionAccount", "admin");

            // Busca o usuário logado
            var username = user.FindFirst(ClaimTypes.Name)?.Value;
            var userModel = await _userService.GetUserByUsernameAsync(username);

            if (userModel == null)
            {
                // Tratar o caso em que o usuário não é encontrado
                return RedirectToAction("Error", "Home");
            }

            // Carrega as listas de Pastas Principais e SubPastas
            var pastasPrincipais = await _pastaPrincipalService.GetAllAsync();
            var subPastas = await _subPastaService.GetAllAsync();

            // Passa as listas e a informação de admin para a View
            ViewBag.PastasPrincipais = pastasPrincipais;
            ViewBag.SubPastas = subPastas;
            ViewBag.IsAdmin = isAdmin;

            // Passa as informações do usuário para a View
            ViewBag.Usuario = userModel.Usuario;
            ViewBag.Empresa = userModel.EmpresaContratante;

            return View();
        }


[HttpPost]
public async Task<IActionResult> UploadDocumentos([FromForm] DocumentoDto dto)
{
    var user = HttpContext.User;
    bool isAdmin = user.Identity?.IsAuthenticated ?? false && user.HasClaim("PermissionAccount", "admin");

    // Busca o usuário logado
    var username = user.FindFirst(ClaimTypes.Name)?.Value;
    var userModel = await _userService.GetUserByUsernameAsync(username);

    if (userModel == null)
    {
        return Json(new { success = false, message = "Usuário não encontrado." });
    }

    // Preenche Usuário e Empresa automaticamente
    dto.Usuario = userModel.Usuario;
    dto.Empresa = userModel.EmpresaContratante;

    // Validação para documentos confidenciais
    if (dto.NivelSeguranca == "Confidencial" && !isAdmin)
    {
        return Json(new { success = false, message = "Apenas administradores podem criar documentos confidenciais." });
    }

    if (!ModelState.IsValid)
    {
        var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();
        return Json(new { success = false, message = "Erro de validação.", errors });
    }

    try
    {
        // --- Renomear o arquivo ---
        var pasta = dto.NomePastaPrincipal?.Substring(0, 3).ToUpper() ?? "XXX";
        var subpasta = dto.NomeSubPasta?.Substring(0, 3).ToUpper() ?? "YYY";
        var hoje = DateTime.UtcNow;
        var dia = hoje.Day.ToString("00");
        var mes = hoje.Month.ToString("00");
        var ano = hoje.Year.ToString();

        // Montar código: XXXDDMMAAAAYYY (sem o ":")
        var code = $"{pasta}{dia}{mes}{ano}{subpasta}";

        // Extrair extensão do arquivo
        var ext = Path.GetExtension(dto.Arquivo.FileName);

        // Novo nome do arquivo
        var fileName = $"{code}{ext}";

        // Verificar se o arquivo já existe
        if (await _documentoService.ArquivoExisteAsync(fileName))
        {
            return Json(new { success = false, message = "O arquivo já existe no sistema." });
        }

        // Upload do arquivo
        string url;
        using (var stream = dto.Arquivo.OpenReadStream())
        {
            url = await _blobService.UploadAsync(fileName, stream);
        }

        // Criar o documento no banco de dados
        await _documentoService.CreateAsync(dto);
        return Json(new { success = true, message = "Upload realizado com sucesso!" });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = $"Erro ao realizar o upload: {ex.Message}" });
    }
}
    }
}