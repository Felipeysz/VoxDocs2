using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace VoxDocs.Controllers
{
    public class BuscarMvcController : Controller
    {
        private readonly ILogger<BuscarMvcController> _logger;

        public BuscarMvcController(ILogger<BuscarMvcController> logger)
        {
            _logger = logger;
        }

        public IActionResult Buscar()
        {
            // A verificação de token passa a ser feita no JS da própria View
            ViewData["Title"] = "Buscar Documentos";
            return View();
        }
    }
}
