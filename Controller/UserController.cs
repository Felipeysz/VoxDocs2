using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoxDocs.DTO;
using VoxDocs.Services;
using VoxDocs.BusinessRules;

namespace VoxDocs.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly UserBusinessRules _rules;
        private readonly IConfiguration _configuration;

        public UserController(
            ILogger<UserController> logger,
            IUserService userService,
            IConfiguration configuration,
            UserBusinessRules rules)
        {
            _logger = logger;
            _userService = userService;
            _configuration = configuration;
            _rules = rules;
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] DTOUser dto)
        {
            _logger.LogInformation("Register attempt for {@User}", dto);
            try
            {
                _rules.ValidateRegister(dto);
                var user = await _userService.RegisterUserAsync(dto);
                return Ok(new
                {
                    user.Id,
                    user.Usuario,
                    user.Email,
                    user.PermissionAccount,
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
        public async Task<IActionResult> Login([FromBody] DTOUserLogin dto)
        {
            _logger.LogInformation("Login attempt for {User}", dto.Usuario);
            try
            {
                _rules.ValidateLoginDto(dto);
                var principal = await _userService.ValidateUserAsync(dto);

                var user = await _userService.GetUserByUsernameAsync(dto.Usuario);
                if (user == null)
                    throw new KeyNotFoundException("Usuário não encontrado.");

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
                    usuario = user.Usuario,
                    email = user.Email,
                    permissionAccount = user.PermissionAccount
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
        public async Task<IActionResult> UpdateUser(int id, [FromBody] DTOUser dto)
        {
            _logger.LogInformation("Update attempt for {UserId}", id);
            try
            {
                var loggedUser = User.Identity?.Name;
                var isAdmin = User.IsInRole("admin");

                if (!isAdmin && dto.Usuario != loggedUser)
                    return Forbid("Você não tem permissão para editar este usuário.");

                dto.Usuario = dto.Usuario.Trim();
                _rules.ValidateUpdate(dto);

                var updatedUser = await _userService.UpdateUserAsync(dto);
                return Ok(new
                {
                    updatedUser.Id,
                    updatedUser.Usuario,
                    updatedUser.PermissionAccount,
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

                _rules.ValidateDelete(id);
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

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> UpdatePassword([FromBody] DTOUserLoginPasswordChange dto)
        {
            try
            {
                var result = await _userService.UpdatePasswordAsync(dto.Usuario, dto.SenhaAntiga, dto.NovaSenha);
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
                return StatusCode(500, new { mensagem = "Erro interno.", detalhes = ex.Message });
            }
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> GeneratePasswordResetLink([FromBody] string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
                return NotFound(new { mensagem = "Conta inexistente ou e-mail incorreto." });

            var token = Guid.NewGuid().ToString("N");
            await _userService.SavePasswordResetTokenAsync(user.Id, token);

            var frontendBaseUrl = _configuration.GetValue<string>("ApiSettings:BaseUrl");
            var resetLink = $"{frontendBaseUrl}/LinkRedefinirSenha?token={token}";

            return Ok(new { mensagem = "Link de redefinição gerado.", link = resetLink });
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ResetPasswordWithToken([FromBody] DTOResetPasswordWithToken dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Token) || string.IsNullOrWhiteSpace(dto?.NovaSenha))
                return BadRequest(new { mensagem = "Token e nova senha são obrigatórios." });

            var userId = await _userService.GetUserIdByResetTokenAsync(dto.Token);
            if (userId == null)
                return BadRequest(new { mensagem = "Token inválido ou expirado." });

            await _userService.UpdatePasswordByIdAsync(userId.Value, dto.NovaSenha);
            await _userService.InvalidatePasswordResetTokenAsync(dto.Token);

            return Ok(new { mensagem = "Senha redefinida com sucesso!" });
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> GetTokenExpiration(string token)
        {
            var expiration = await _userService.GetPasswordResetTokenExpirationAsync(token);
            if (expiration == null)
                return NotFound();

            return Ok(new { expiration = expiration.Value.ToString("o") });
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest(new { mensagem = "Usuário não informado." });

            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null)
                return NotFound(new { mensagem = "Usuário não encontrado." });

            // Retorna apenas as informações necessárias para o perfil
            var dto = new UserInfoDTO
            {
                Usuario = user.Usuario,
                Email = user.Email,
                PermissionAccount = user.PermissionAccount
            };
            return Ok(dto);
        }
    }
}