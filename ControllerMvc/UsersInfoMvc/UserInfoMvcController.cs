using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoxDocs.DTO;
using System.Text.Json;
using System.Net;

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

            // Adicionar o tokenPermissao no header
            var tokenPermissao = HttpContext.Session.GetString("tokenPermissao");
            if (!string.IsNullOrEmpty(tokenPermissao))
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenPermissao}");
            }

            try
            {
                var response = await client.GetAsync($"/api/User/GetUserByUsername?username={User.Identity.Name}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var userInfo = JsonSerializer.Deserialize<DTOUserInfo>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    // Determinar qual ViewModel usar baseado na permissão do usuário
                    if (userInfo.PermissionAccount.ToLower() == "admin")
                    {
                        // Para admin, buscar informações adicionais
                        var adminResponse = await client.GetAsync($"/api/Admin/GetAdminStats?username={User.Identity.Name}");
                        if (adminResponse.IsSuccessStatusCode)
                        {
                            var adminJson = await adminResponse.Content.ReadAsStringAsync();
                            var adminStats = JsonSerializer.Deserialize<AdminStatsDTO>(adminJson);

                            var adminViewModel = new AdminInfoAccount
                            {
                                UserInfo = userInfo,
                                TotalUsers = adminStats.TotalUsers,
                                ActiveUsers = adminStats.ActiveUsers,
                                RecentUsers = adminStats.RecentUsers
                            };

                            return View("AdminProfile", adminViewModel);
                        }
                    }

                    // Para usuários normais
                    var userResponse = await client.GetAsync($"/api/User/GetUserStorageInfo?username={User.Identity.Name}");
                    var userViewModel = new UserInfoAccount
                    {
                        UserInfo = userInfo,
                        CanCreateFolders = true // Definir conforme regras de negócio
                    };

                    if (userResponse.IsSuccessStatusCode)
                    {
                        var storageJson = await userResponse.Content.ReadAsStringAsync();
                        var storageInfo = JsonSerializer.Deserialize<UserStorageDTO>(storageJson);
                        userViewModel.StorageUsage = storageInfo.StorageUsage;
                        userViewModel.StorageLimit = storageInfo.StorageLimit;
                    }

                    return View("UserProfile", userViewModel);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning($"Usuário {User.Identity.Name} não encontrado na API");
                    TempData["ProfileError"] = "Seu perfil não foi encontrado no sistema.";
                }
                else
                {
                    _logger.LogError($"Erro ao buscar perfil: {response.StatusCode}");
                    TempData["ProfileError"] = "Ocorreu um erro ao carregar seu perfil.";
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro na comunicação com a API");
                TempData["ProfileError"] = "Não foi possível conectar ao servidor de perfis.";
            }

            // Retorno padrão em caso de erro
            return View("UserProfile", new UserInfoAccount
            {
                UserInfo = new DTOUserInfo
                {
                    Usuario = User.Identity.Name,
                    Email = "",
                    PermissionAccount = "user",
                    EmpresaContratante = "",
                    Plano = "basic"
                }
            });
        }
    }
}