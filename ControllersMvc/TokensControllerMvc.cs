using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace VoxDocs.Controllers
{
    public class TokensMvcController : Controller
    {
        private readonly ILogger<TokensMvcController> _logger;

        public TokensMvcController(ILogger<TokensMvcController> logger)
        {
            _logger = logger;
        }

        public IActionResult Tokens()
        {
            return View("AdminPages/Tokens");
        }
    }
}
