using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Controllers
{
    public class LoginMvcController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public LoginMvcController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["LoginError"] = "Por favor, preencha todos os campos corretamente."; // Passa erro de validação
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("VoxDocsApi");
            var dto = new DTOUserLogin { Usuario = model.Usuario, Senha = model.Senha };
            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");


            var response = await client.PostAsync("/api/User/Login", content);
            if (!response.IsSuccessStatusCode)
            {
                TempData["LoginError"] = "Usuário ou senha inválidos."; // Erro de autenticação
                return View(model);
            }

            // lê o payload bruto
            var payload = await response.Content.ReadAsStringAsync();
            Console.WriteLine("[Login] payload: " + payload);

            using var doc = JsonDocument.Parse(payload);
            JsonElement tokElem;
            if (!doc.RootElement.TryGetProperty("Token", out tokElem) &&
                !doc.RootElement.TryGetProperty("token", out tokElem))
            {
                TempData["LoginError"] = "Resposta inválida do servidor (token não encontrado)."; // Erro na resposta
                return View(model);
            }

            var token = tokElem.GetString();
            // salva JWT
            HttpContext.Session.SetString("JWToken", token);

            // extrai expiração do próprio JWT
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var expiresUtc = jwt.ValidTo; // já em UTC

            // salva expiração em ISO (round‑trip)
            HttpContext.Session.SetString("TokenExpiration", expiresUtc.ToString("o"));

            // redireciona para a página de busca
            return RedirectToAction("Buscar", "BuscarMvc");
        }
    }
}
