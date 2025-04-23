using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

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
            // if (string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken")))
            // {
            //     // Se não tiver token, redireciona para o login
            //     return RedirectToAction("Login", "LoginMvc");
            // }

            // // Se tiver token, renderiza normalmente a página de busca
            // ViewBag.TokenExpiration = HttpContext.Session.GetString("TokenExpiration");
            return View();
        }
    }
}
