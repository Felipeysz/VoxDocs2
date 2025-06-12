using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using VoxDocs.DTO;
using VoxDocs.Services;
using Microsoft.AspNetCore.Authentication;

namespace VoxDocs.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IPlanosVoxDocsService _planosService;
        private readonly IEmpresasContratanteService _empresasService;

        public UserController(
            ILogger<UserController> logger,
            IUserService userService,
            IConfiguration configuration,
            IPlanosVoxDocsService planosService,
            IEmpresasContratanteService empresasService)
        {
            _logger = logger;
            _userService = userService;
            _configuration = configuration;
            _planosService = planosService;
            _empresasService = empresasService;
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] DTORegisterUser dto)
        {
            _logger.LogInformation("Register attempt for {@User}", dto);

            try
            {
                var (user, limiteAdmin, limiteUsuario) = await _userService.RegisterAsync(dto);

                return Ok(new
                {
                    user.Id,
                    user.Usuario,
                    user.Email,
                    user.PermissionAccount,
                    LimiteUsuario = limiteUsuario,
                    LimiteAdmin = limiteAdmin,
                    mensagem = "Usuário criado com sucesso!"
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new { mensagem = "Erro interno.", detalhes = ex.Message });
            }
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] DTOLoginUser dto)
        {
            _logger.LogInformation("Tentativa de login para {Usuário}", dto.Usuario);
            try
            {
                var principal = await _userService.AuthenticateAsync(dto);

                var authProps = new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2),
                    AllowRefresh = true
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProps
                );

                return Ok(new
                {
                    usuario = dto.Usuario,
                    mensagem = "Login realizado com sucesso!"
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return NotFound(new { mensagem = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return Unauthorized(new { mensagem = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new { mensagem = "Erro interno.", detalhes = ex.Message });
            }
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { mensagem = "Logout realizado com sucesso." });
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var list = await _userService.GetUsersAsync();
            return Ok(list.Select(u => new { u.Id, u.Usuario, u.PermissionAccount }));
        }

        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] DTOUpdateUser dto)
        {
            _logger.LogInformation("Update attempt for {UserId}", id);
            try
            {
                var loggedUser = User.Identity?.Name;
                var isAdmin = User.IsInRole("admin");

                if (!isAdmin && dto.Usuario != loggedUser)
                    return Forbid("Você não tem permissão para editar este usuário.");

                dto.Usuario = dto.Usuario.Trim();

                await _userService.UpdateAsync(dto);

                return Ok(new
                {
                    mensagem = "Usuário atualizado com sucesso!"
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return NotFound(new { mensagem = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new { mensagem = "Erro interno.", detalhes = ex.Message });
            }
        }

        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var loggedUser = User.Identity?.Name;
                var isAdmin = User.IsInRole("admin");

                if (!isAdmin)
                {
                    var user = await _userService.GetUserByIdAsync(id);
                    if (user == null || user.Usuario != loggedUser)
                        return Forbid("Você não tem permissão para excluir este usuário.");
                }

                await _userService.DeleteUserAsync(id);

                return Ok(new { mensagem = $"Usuário com ID {id} deletado com sucesso!" });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return NotFound(new { mensagem = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500, new { mensagem = "Erro interno.", detalhes = ex.Message });
            }
        }
    }
}