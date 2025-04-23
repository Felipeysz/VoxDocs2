using Microsoft.AspNetCore.Mvc;
using VoxDocs.DTO;

namespace VoxDocs.Controllers
{
    public class NavBarMvcController : Controller
    {
        [HttpGet]
        public IActionResult Navbar()
        {
            // Monta o DTO com as infos que precisar na navbar
            var navBarInfo = new NavBarInfoDTO
            {
                Usuario = "UsuárioTeste",         // substitua por dados reais
                PermissionAccount = "user"        // "admin" para mostrar o Dashboard
            };

            // Retorna a partial (procura em Views/Shared/_NavBarPartial.cshtml)
            return PartialView("_NavBarPartial", navBarInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "LoginMvc");
        }
    }
}
