using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using VoxDocs.DTO;
using System.Text.Json;

namespace VoxDocs.Controllers
{
    [Authorize]
    public class UserInfoMvcController : Controller
    {
        private readonly ILogger<UserInfoMvcController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public UserInfoMvcController(
            ILogger<UserInfoMvcController> logger,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> MeuPerfil()
        {
            var client = _httpClientFactory.CreateClient("VoxDocsApi");
            var response = await client.GetAsync($"/api/User/GetUserByUsername?username={User.Identity.Name}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<UserInfoDTO>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(userInfo);
            }
            else
            {
                TempData["ProfileError"] = "Não foi possível carregar os dados do perfil.";
                return View(new UserInfoDTO { Usuario = User.Identity.Name, Email = "", PermissionAccount = "" });
            }
        }
    }
}