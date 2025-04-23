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
            // if (!ModelState.IsValid)
            // {
            //     TempData["LoginError"] = "Por favor, preencha todos os campos corretamente."; // Passa erro de validação
            //     return View(model);
            // }

            // var client = _httpClientFactory.CreateClient("VoxDocsApi");
            // var dto = new DTOUserLogin { Usuario = model.Usuario, Senha = model.Senha };
            // var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");

            // var response = await client.PostAsync("/api/User/Login", content);

            // if (!response.IsSuccessStatusCode)
            // {
            //     // Trata erro específico retornado pela API
            //     if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            //     {
            //         TempData["LoginError"] = "Usuário ou senha inválidos.";
            //     }
            //     else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            //     {
            //         TempData["LoginError"] = "Usuário já está logado. Por favor, tente novamente.";
            //     }
            //     else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            //     {
            //         TempData["LoginError"] = "Campos obrigatórios não preenchidos corretamente.";
            //     }
            //     else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            //     {
            //         // Aqui, capturamos o erro 500 e mostramos a mensagem ao usuário
            //         var payload = await response.Content.ReadAsStringAsync();
            //         var error = JsonSerializer.Deserialize<ErrorResponse>(payload);
            //         TempData["LoginError"] = error?.Mensagem ?? "Ocorreu um erro no servidor.";
            //     }
            //     else
            //     {
            //         TempData["LoginError"] = "Erro desconhecido ao tentar fazer login.";
            //     }
            //     return View(model);
            // }

            // var payloadResponse = await response.Content.ReadAsStringAsync();
            // Console.WriteLine("[Login] payload: " + payloadResponse);

            // using var doc = JsonDocument.Parse(payloadResponse);
            // JsonElement tokenElement;
            // if (!doc.RootElement.TryGetProperty("Token", out tokenElement))
            // {
            //     TempData["LoginError"] = "Resposta inválida do servidor (token não encontrado)."; // Erro na resposta
            //     return View(model);
            // }

            // var token = tokenElement.GetString();
            // HttpContext.Session.SetString("JWToken", token);

            // var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            // var expiresUtc = jwt.ValidTo;
            // HttpContext.Session.SetString("TokenExpiration", expiresUtc.ToString("o"));

            return RedirectToAction("Buscar", "BuscarMvc");
        }

    }
}
