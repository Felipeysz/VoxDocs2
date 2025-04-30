using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace VoxDocs.Controllers
{
    [Authorize]
    public class BuscarMvcController : Controller
    {
        private readonly ILogger<BuscarMvcController> _logger;
        public BuscarMvcController(ILogger<BuscarMvcController> logger)
            => _logger = logger;

        [HttpGet]
        public IActionResult Buscar()
        {
            ViewData["Title"] = "Buscar Documentos";
            return View();
        }
    }
}
