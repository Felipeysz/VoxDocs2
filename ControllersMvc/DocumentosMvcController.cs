using Microsoft.AspNetCore.Mvc;
using VoxDocs.DTO;
using VoxDocs.Models;
using VoxDocs.Models.ViewModels;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace VoxDocs.ControllersMvc
{
    [Route("[controller]")]
    public class DocumentosMvcController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public DocumentosMvcController(IHttpClientFactory httpClientFactory) =>
            _httpClientFactory = httpClientFactory;

        [HttpGet("Documentos")]
        public async Task<IActionResult> Documentos()
        {
            var client = _httpClientFactory.CreateClient("VoxDocsApi");
            var areas = await client.GetFromJsonAsync<IEnumerable<AreasDocumentoModel>>("api/areasdocumento");
            var tipos = await client.GetFromJsonAsync<IEnumerable<TipoDocumentoModel>>("api/tipodocumento");

            var viewModel = new DocumentosViewModel
            {
                AreasDocumento = areas?.ToList() ?? new List<AreasDocumentoModel>(),
                TiposDocumento = tipos?.ToList() ?? new List<TipoDocumentoModel>()
            };
            return View(viewModel);
        }

        [HttpGet("DocumentosExibir")]
        public async Task<IActionResult> DocumentosExibir(int areaId, int tipoId)
        {
            var client = _httpClientFactory.CreateClient("VoxDocsApi");
            var documentos = await client.GetFromJsonAsync<IEnumerable<DTODocumento>>(
                $"api/documento/filter?areaDocumentoId={areaId}&tipoDocumentoId={tipoId}");

            var areas = await client.GetFromJsonAsync<IEnumerable<AreasDocumentoModel>>("api/areasdocumento");
            var tipos = await client.GetFromJsonAsync<IEnumerable<TipoDocumentoModel>>("api/tipodocumento");

            ViewBag.AreaNome = areas?.FirstOrDefault(a => a.Id == areaId)?.Nome ?? "Área";
            ViewBag.TipoNome = tipos?.FirstOrDefault(t => t.Id == tipoId)?.Nome ?? "Tipo";

            return PartialView("Components/DocumentosExibir", documentos);
        }
    }
}