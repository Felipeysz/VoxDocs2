using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VoxDocs.DTO;
using VoxDocs.Services;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace VoxDocs.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(
            ILogger<UserController> logger,
            IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] DTORegistrarUsuario registerDto)
        {
            try
            {
                var (user, adminLimit, userLimit) = await _userService.RegisterUserAsync(registerDto);

                return Ok(new
                {
                    user.Id,
                    user.Usuario,
                    user.Email,
                    user.PermissaoConta,
                    LimiteAdmin = adminLimit,
                    LimiteUsuario = userLimit,
                    mensagem = "Usuário criado com sucesso!"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário");
                return StatusCode(500, new { mensagem = "Erro interno ao registrar usuário." });
            }
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] DTOLoginUsuario loginDto)
        {
            try
            {
                var principal = await _userService.AuthenticateUserAsync(loginDto);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = false,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2),
                        AllowRefresh = true
                    });

                return Ok(new
                {
                    usuario = loginDto.Usuario,
                    mensagem = "Login realizado com sucesso!"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar login");
                return StatusCode(500, new { mensagem = "Erro interno ao realizar login." });
            }
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { mensagem = "Logout realizado com sucesso." });
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuários");
                return StatusCode(500, new { mensagem = "Erro interno ao obter usuários." });
            }
        }

        [HttpGet("{userId}"), Authorize]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuário");
                return StatusCode(500, new { mensagem = "Erro interno ao obter usuário." });
            }
        }

        [HttpPut, Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] DTOAtualizarUsuario updateDto)
        {
            try
            {
                await _userService.UpdateUserAsync(updateDto);
                return Ok(new { mensagem = "Usuário atualizado com sucesso!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuário");
                return StatusCode(500, new { mensagem = "Erro interno ao atualizar usuário." });
            }
        }

        [HttpDelete("{userId}"), Authorize]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
            {
                await _userService.DeleteUserAsync(userId);
                return Ok(new { mensagem = "Usuário deletado com sucesso!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar usuário");
                return StatusCode(500, new { mensagem = "Erro interno ao deletar usuário." });
            }
        }

        [HttpPost("request-reset"), AllowAnonymous]
        public async Task<IActionResult> RequestPasswordReset([FromBody] string email)
        {
            try
            {
                await _userService.RequestPasswordResetAsync(email);
                return Ok(new { mensagem = "Se o email existir em nosso sistema, um link de redefinição será enviado." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao solicitar redefinição de senha");
                return StatusCode(500, new { mensagem = "Erro interno ao solicitar redefinição de senha." });
            }
        }

        [HttpPost("reset-with-token"), AllowAnonymous]
        public async Task<IActionResult> ResetPasswordWithToken([FromBody] (string token, string novaSenha) resetDto)
        {
            try
            {
                await _userService.ResetPasswordWithTokenAsync(resetDto.token, resetDto.novaSenha);
                return Ok(new { mensagem = "Senha redefinida com sucesso!" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao redefinir senha");
                return StatusCode(500, new { mensagem = "Erro interno ao redefinir senha." });
            }
        }

        [HttpPost("change-password"), Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] (string username, string senhaAntiga, string novaSenha) changeDto)
        {
            try
            {
                await _userService.ChangePasswordAsync(changeDto.username, changeDto.senhaAntiga, changeDto.novaSenha);
                return Ok(new { mensagem = "Senha alterada com sucesso!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar senha");
                return StatusCode(500, new { mensagem = "Erro interno ao alterar senha." });
            }
        }

        [HttpGet("check-email"), AllowAnonymous]
        public async Task<IActionResult> IsEmailAvailable(string email, Guid? excludeUserId = null)
        {
            try
            {
                var isAvailable = await _userService.IsEmailAvailableAsync(email, excludeUserId);
                return Ok(new { disponivel = isAvailable });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar disponibilidade de email");
                return StatusCode(500, new { mensagem = "Erro interno ao verificar email." });
            }
        }

        [HttpGet("check-username"), AllowAnonymous]
        public async Task<IActionResult> IsUsernameAvailable(string username, Guid? excludeUserId = null)
        {
            try
            {
                var isAvailable = await _userService.IsUsernameAvailableAsync(username, excludeUserId);
                return Ok(new { disponivel = isAvailable });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar disponibilidade de usuário");
                return StatusCode(500, new { mensagem = "Erro interno ao verificar usuário." });
            }
        }

        [HttpGet("storage/{userId}"), Authorize]
        public async Task<IActionResult> GetUserStorageInfo(Guid userId)
        {
            try
            {
                var storageInfo = await _userService.GetUserStorageInfoAsync(userId);
                return Ok(storageInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter informações de armazenamento");
                return StatusCode(500, new { mensagem = "Erro interno ao obter informações de armazenamento." });
            }
        }

        [HttpGet("admin-stats"), Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAdminStatistics()
        {
            try
            {
                var stats = await _userService.GetAdminStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas administrativas");
                return StatusCode(500, new { mensagem = "Erro interno ao obter estatísticas administrativas." });
            }
        }

        [HttpPost("toggle-status/{userId}"), Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ToggleUserStatus(Guid userId, [FromBody] bool ativo)
        {
            try
            {
                await _userService.ToggleUserStatusAsync(userId, ativo);
                return Ok(new { mensagem = $"Status do usuário alterado para {(ativo ? "ativo" : "inativo")}" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar status do usuário");
                return StatusCode(500, new { mensagem = "Erro interno ao alterar status do usuário." });
            }
        }
    }
}