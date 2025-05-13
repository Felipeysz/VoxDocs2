using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VoxDocs.Services;

namespace VoxDocs.ControllersMvc
{
    [Authorize]
    public class UploadDocumentoMvcController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAreasDocumentoService _areasDocumentoService;
        private readonly ITipoDocumentoService _tipoDocumentoService;
        private readonly IConfiguration _configuration;

        public UploadDocumentoMvcController(
            IHttpClientFactory httpClientFactory,
            IAreasDocumentoService areasDocumentoService,
            ITipoDocumentoService tipoDocumentoService,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _areasDocumentoService = areasDocumentoService;
            _tipoDocumentoService = tipoDocumentoService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            ViewBag.Areas = await _areasDocumentoService.GetAllAsync();
            ViewBag.Tipos = await _tipoDocumentoService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file, int areaId, int tipoId, string descricao)
        {
            var usuario = User.Identity?.Name ?? "desconhecido";

            if (file == null || areaId == 0 || tipoId == 0)
            {
                TempData["UploadError"] = "Preencha todos os campos.";
                ViewBag.Areas = await _areasDocumentoService.GetAllAsync();
                ViewBag.Tipos = await _tipoDocumentoService.GetAllAsync();
                return View();
            }

            var client = _httpClientFactory.CreateClient();
            var baseUrl = _configuration["ApiSettings:BaseUrl"];
            var apiUrl = $"{baseUrl}/api/UploadDocumentos/upload";

            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StringContent(areaId.ToString()), "areaId");
                content.Add(new StringContent(tipoId.ToString()), "tipoId");
                content.Add(new StringContent(usuario), "usuario");
                content.Add(new StringContent(descricao ?? ""), "descricao");
                var streamContent = new StreamContent(file.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(streamContent, "file", file.FileName);

                var response = await client.PostAsync(apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["UploadSuccess"] = "Arquivo enviado com sucesso!";
                    return RedirectToAction("Upload");
                }
                else
                {
                    TempData["UploadError"] = "Erro ao enviar arquivo.";
                    ViewBag.Areas = await _areasDocumentoService.GetAllAsync();
                    ViewBag.Tipos = await _tipoDocumentoService.GetAllAsync();
                    return View();
                }
            }
        }
    }
}