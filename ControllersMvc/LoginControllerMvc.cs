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
using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Controllers
{
    public class LoginMvcController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginMvcController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() => View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["LoginError"] = "Por favor, preencha todos os campos corretamente.";
                return View(model);
            }

            try
            {
                // Chama a API
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
                    if (response.StatusCode == HttpStatusCode.NotFound)
                        TempData["LoginError"] = "Conta Inexistente.";
                    else if (response.StatusCode == HttpStatusCode.Unauthorized)
                        TempData["LoginError"] = "Usuário ou senha inválidos.";
                    else
                        TempData["LoginError"] = "Erro ao fazer login.";

                    return View(model);
                }

                // Lê o JSON de resposta
                var payload = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(payload);
                if (!doc.RootElement.TryGetProperty("Token", out var tokElem)
                && !doc.RootElement.TryGetProperty("token", out tokElem))
                {
                    TempData["LoginError"] = "Resposta inválida do servidor (token não encontrado).";
                    return View(model);
                }

                var bearer = tokElem.GetString()!;

                // Decodifica JWT apenas para pegar o ValidTo (expiração do token)
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(bearer);

                // Prepara script que grava o token puro no localStorage + redireciona
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
                // Captura qualquer outro tipo de erro (network, serialização, etc)
                TempData["LoginError"] = $"ERROR: {ex.Message}";
                return View(model);
            }
        }

    }
}
