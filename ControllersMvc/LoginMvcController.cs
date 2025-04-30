using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Controllers
{
    public class LoginMvcController : Controller
    {
        private readonly ILogger<LoginMvcController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginMvcController(
            ILogger<LoginMvcController> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["LoginError"] = "Preencha todos os campos corretamente.";
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("VoxDocsApi");
            var dto    = new DTOUserLogin { Usuario = model.Usuario, Senha = model.Senha };
            var res    = await client.PostAsJsonAsync("/api/User/Login", dto);

            if (!res.IsSuccessStatusCode)
            {
                TempData["LoginError"] = res.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound  => "Conta inexistente.",
                    System.Net.HttpStatusCode.BadRequest => "Usuário ou senha inválidos.",
                    _                                    => "Erro ao fazer login."
                };
                return View(model);
            }

            var body     = await res.Content.ReadFromJsonAsync<JsonElement>();
            var token    = body.GetProperty("token").GetString();
            var userElem = body.GetProperty("user");
            var userObj  = JsonSerializer.Deserialize<UserModel>(
                userElem.GetRawText(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var jwt  = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var role = jwt.Claims.FirstOrDefault(c => c.Type == "PermissionAccount")?.Value ?? "user";

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userObj.Usuario),
                new Claim(ClaimTypes.NameIdentifier, userObj.Id.ToString()),
                new Claim("PermissionAccount", role),
            };
            var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc   = jwt.ValidTo
                });

            // Salva o token na sessão para usar depois (como no logout ou chamadas API protegidas)
            HttpContext.Session.SetString("JWTToken", token);

            return RedirectToAction("Buscar", "BuscarMvc");
        }

    }
}
