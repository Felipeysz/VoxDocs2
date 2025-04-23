using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using VoxDocs.DTO;

namespace VoxDocs.Controllers
{
    public class NavBarMvcController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public NavBarMvcController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Navbar()
        {
            var navBarInfo = new NavBarInfoDTO
            {
                Usuario = "UsuárioTeste",
                PermissionAccount = "user"
            };

            try
            {
                string? bearerToken = null;

                // Pegando o Bearer Token do Header da requisição
                if (Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    bearerToken = authHeader.ToString().Replace("Bearer ", "");
                    Console.WriteLine($"[DEBUG] Bearer token encontrado: {bearerToken}");
                }
                else
                {
                    Console.WriteLine("[DEBUG] Authorization header não encontrado.");
                }

                if (!string.IsNullOrEmpty(bearerToken))
                {
                    var client = _httpClientFactory.CreateClient();

                    client.BaseAddress = new Uri("http://localhost:5151/"); // Defina a base aqui
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                    Console.WriteLine("[DEBUG] Enviando GET para /api/User/GetUserBearerToken...");
                    var response = await client.GetAsync("api/User/GetUserBearerToken"); // apenas o final da URL

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("[DEBUG] Requisição com sucesso.");
                        var responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"[DEBUG] Conteúdo da resposta: {responseContent}");

                        var users = JsonSerializer.Deserialize<List<NavBarInfoDTO>>(responseContent);

                        if (users != null && users.Count > 0)
                        {
                            navBarInfo.Usuario = users[0].Usuario;
                            navBarInfo.PermissionAccount = users[0].PermissionAccount;
                            Console.WriteLine($"[DEBUG] Usuário: {navBarInfo.Usuario}, Permissão: {navBarInfo.PermissionAccount}");
                        }
                        else
                        {
                            Console.WriteLine("[DEBUG] Resposta vazia ou formato inválido.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[DEBUG] Erro na requisição. Status: {response.StatusCode}, Motivo: {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[DEBUG] Exceção ao buscar usuário da Navbar: " + ex.Message);
            }

            return PartialView("_NavBarPartial", navBarInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "LoginMvc");
        }
    }
}
