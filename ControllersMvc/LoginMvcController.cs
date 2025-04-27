using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;             // ← ILogger
using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Controllers
{
    public class LoginMvcController : Controller
    {
        private readonly ILogger<LoginMvcController> _logger;      // ← Injeção de ILogger
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginMvcController(
            ILogger<LoginMvcController> logger,                  // ← DI do logger :contentReference[oaicite:3]{index=3}
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            _logger.LogInformation("Acessando página de Login.");  // log de acesso :contentReference[oaicite:4]{index=4}
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _logger.LogInformation("Tentativa de login para usuário {User}", model.Usuario); // log de entrada :contentReference[oaicite:5]{index=5}

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState inválido ao fazer login: {@ModelState}", ModelState); // log de alerta
                TempData["LoginError"] = "Por favor, preencha todos os campos corretamente.";
                return View(model);
            }

            try
            {
                var client  = _httpClientFactory.CreateClient("VoxDocsApi");
                var dto     = new DTOUserLogin { Usuario = model.Usuario, Senha = model.Senha };
                var content = new StringContent(
                    JsonSerializer.Serialize(dto),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync("/api/User/Login", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Resposta da API Login retornou {StatusCode}", response.StatusCode); // log de alerta :contentReference[oaicite:6]{index=6}
                    if (response.StatusCode == HttpStatusCode.NotFound)
                        TempData["LoginError"] = "Conta Inexistente.";
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        TempData["LoginError"] = "Usuário ou senha inválidos.";
                    else
                        TempData["LoginError"] = "Erro ao fazer login.";

                    return View(model);
                }

                var payload = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(payload);
                if (!doc.RootElement.TryGetProperty("Token", out var tokElem))
                {
                    _logger.LogError("Token não encontrado na resposta da API."); // log de erro
                    TempData["LoginError"] = "Resposta inválida do servidor (token não encontrado).";
                    return View(model);
                }

                var bearer = tokElem.GetString()!;
                var jwt     = new JwtSecurityTokenHandler().ReadJwtToken(bearer);

                _logger.LogInformation("Login bem-sucedido para usuário {User}, expirará em {Exp}", model.Usuario, jwt.ValidTo); // log de sucesso

                var jsBearer  = JsonSerializer.Serialize(bearer);
                var jsExp     = JsonSerializer.Serialize(jwt.ValidTo.ToString("o"));
                var urlBuscar = Url.Action("Buscar", "BuscarMvc")!;

                var script = $@"
                    <script>
                      localStorage.setItem('Bearer_Token', {jsBearer});
                      localStorage.setItem('TokenExpiration', {jsExp});
                      window.location = '{urlBuscar}';
                    </script>";

                return Content(script, "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exceção ao processar login para usuário {User}", model.Usuario); // log de exceção
                TempData["LoginError"] = $"ERROR: {ex.Message}";
                return View(model);
            }
        }
    }
}
