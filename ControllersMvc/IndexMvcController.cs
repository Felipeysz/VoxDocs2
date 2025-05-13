using Microsoft.AspNetCore.Mvc;

namespace VoxDocs.Controllers
{
    public class IndexMvcController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "Pagina Principal";
            return View();
        }
    }
}
