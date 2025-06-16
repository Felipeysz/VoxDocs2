// Controllers/PerfilMvcController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoxDocs.Services;
using VoxDocs.ViewModels;

namespace VoxDocs.Controllers
{
    [Authorize]
    public class PerfilMvcController : Controller
    {
        private readonly ILogger<PerfilMvcController> _logger;
        private readonly IUserService _userService;

        public PerfilMvcController(
            ILogger<PerfilMvcController> logger,
            IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> MeuPerfil()
        {
            try
            {
                var username = User.Identity.Name;
                var userDto = await _userService.GetUserByUsernameAsync(username);

                var viewModel = new PerfilViewModel
                {
                    Id = userDto.Id,
                    Usuario = userDto.Usuario,
                    Email = userDto.Email,
                    EmpresaContratante = userDto.EmpresaContratante,
                    Plano = userDto.Plano,
                    PermissaoConta = userDto.PermissaoConta,
                    DataCriacao = userDto.DataCriacao,
                    UltimoLogin = userDto.UltimoLogin,
                    Ativo = userDto.Ativo
                };

                return View(viewModel);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar perfil do usuário");
                TempData["ErrorMessage"] = "Não foi possível carregar os dados do perfil.";
                return View(new PerfilViewModel { Usuario = User.Identity.Name });
            }
        }
    }
}