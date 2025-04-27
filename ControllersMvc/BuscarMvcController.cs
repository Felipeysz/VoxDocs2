using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace VoxDocs.Controllers
{
    [Authorize]  // ← Protege todas as ações do controller :contentReference[oaicite:8]{index=8}
    public class BuscarMvcController : Controller
    {
        private readonly ILogger<BuscarMvcController> _logger;

        public BuscarMvcController(ILogger<BuscarMvcController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Buscar()
        {
            _logger.LogInformation("Acessando página de busca de documentos."); // log de acesso
            ViewData["Title"] = "Buscar Documentos";
            return View();
        }
    }
}
