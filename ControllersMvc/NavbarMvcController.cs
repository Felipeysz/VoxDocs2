using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public class NavbarMvcController : Controller
{
    private readonly ILogger<NavbarMvcController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public NavbarMvcController(ILogger<NavbarMvcController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        _logger.LogInformation("Usuário {User} iniciou logout.", User?.Identity?.Name ?? "desconhecido");

        var client = _httpClientFactory.CreateClient("VoxDocsApi");

        try
        {
            // Chama o logout da API autenticada
            var response = await client.PostAsync("/api/User/Logout", null);
            if (!response.IsSuccessStatusCode)
            {
                TempData["LogoutError"] = "Erro ao sair. Tente novamente.";
                _logger.LogWarning("Erro ao chamar API de logout: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            TempData["LogoutError"] = "Erro ao sair. Tente novamente.";
            _logger.LogError(ex, "Erro ao chamar a API de logout.");
        }

        // Remove o cookie de autenticação
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Apaga todos os cookies da requisição
        foreach (var cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }

        // Limpa a sessão
        HttpContext.Session.Clear();

        // Redireciona para tela de login
        return RedirectToAction("Login", "LoginMvc");
    }
}