using Microsoft.AspNetCore.Mvc;
using VoxDocs.Models;
using VoxDocs.Services;
using VoxDocs.DTO;
using System.Threading.Tasks;

namespace VoxDocs.Controllers
{
    public class DocumentosMvcController : Controller
    {
        private readonly IDocumentoService _documentoService;
        private readonly AzureBlobService _blobService;

        public DocumentosMvcController(IDocumentoService documentoService, AzureBlobService blobService)
        {
            _documentoService = documentoService;
            _blobService = blobService;
        }

        [HttpGet]
        public IActionResult UploadDocumentos()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocumentos(DocumentoDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var fileName = $"{Guid.NewGuid()}_{dto.Arquivo.FileName}";
            string url;
            using (var stream = dto.Arquivo.OpenReadStream())
            {
                url = await _blobService.UploadAsync(fileName, stream);
            }

            await _documentoService.CreateAsync(dto);
            return RedirectToAction("UploadDocumentos");
        }
    }
}