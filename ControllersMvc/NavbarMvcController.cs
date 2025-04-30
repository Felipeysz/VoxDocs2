using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public class NavbarControllerMvc : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<NavbarControllerMvc> _logger;

    public NavbarControllerMvc(IHttpClientFactory httpClientFactory, ILogger<NavbarControllerMvc> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        _logger.LogInformation("Iniciando Logout MVC.");

        // 1. Recupera o token ANTES de limpar tudo
        var token = HttpContext.Session.GetString("JWTToken");

        // 2. Desloga o cookie
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // 3. Limpa a sessão
        HttpContext.Session.Clear();

        // 4. Se havia token, chama a API de logout via Bearer
        if (!string.IsNullOrEmpty(token))
        {
            var client = _httpClientFactory.CreateClient("VoxDocsApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsync("api/User/Logout", null);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Falha ao invalidar token na API: {Reason}", response.ReasonPhrase);
            }
        }
        else
        {
            _logger.LogInformation("Nenhum token JWT para invalidar.");
        }

        return RedirectToAction("Login", "LoginMvc");
    }
}
