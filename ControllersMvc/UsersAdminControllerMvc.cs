using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace VoxDocs.Controllers
{
    public class UsersAdminMvcController : Controller
    {
        private readonly ILogger<UsersAdminMvcController> _logger;

        public UsersAdminMvcController(ILogger<UsersAdminMvcController> logger)
        {
            _logger = logger;
        }

        public IActionResult UsersAdmin()
        {
            return View("AdminPages/UsersAdmin");
        }
    }
}
