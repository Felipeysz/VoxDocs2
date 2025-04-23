using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace VoxDocs.Controllers
{
    public class UploadMvcController : Controller
    {
        private readonly ILogger<UploadMvcController> _logger;

        public UploadMvcController(ILogger<UploadMvcController> logger)
        {
            _logger = logger;
        }

        public IActionResult Upload()
        {
            return View("AdminPages/Upload");
        }
    }
}
