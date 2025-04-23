using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace VoxDocs.Controllers
{
    public class DashboardMvcController : Controller
    {
        private readonly ILogger<DashboardMvcController> _logger;

        public DashboardMvcController(ILogger<DashboardMvcController> logger)
        {
            _logger = logger;
        }

        public IActionResult Dashboard()
        {
            return View("AdminPages/Dashboard");
        }
    }
}
