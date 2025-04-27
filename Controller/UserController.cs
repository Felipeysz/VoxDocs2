using System;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VoxDocs.DTO;
using VoxDocs.Services;

namespace VoxDocs.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserService _userService;

        public UserController(
            ILogger<UserController> logger,
            UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Consumes("application/json")]
        public async Task<IActionResult> Register([FromBody] DTOUser userDto)
        {
            _logger.LogInformation("Iniciando registro de usuário {@UserDto}", userDto);

            if (string.IsNullOrEmpty(userDto.Usuario)
                || string.IsNullOrEmpty(userDto.Senha)
                || string.IsNullOrEmpty(userDto.PermissionAccount))
            {
                _logger.LogWarning("Dados incompletos no registro: {@UserDto}", userDto);
                return BadRequest(new { Mensagem = "Usuário, senha e permissão são obrigatórios." });
            }

            try
            {
                var user = await _userService.RegisterUserAsync(userDto);
                _logger.LogInformation("Usuário criado com sucesso: {UserId}", user.Id);

                return Ok(new
                {
                    user.Id,
                    user.Usuario,
                    user.PermissionAccount,
                    Mensagem = "Usuário criado com sucesso!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário {@UserDto}", userDto);
                return StatusCode(500, new
                {
                    Mensagem = "Ocorreu um erro interno ao registrar o usuário.",
                    Detalhes = ex.Message
                });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Consumes("application/json")]
        public async Task<IActionResult> Login([FromBody] DTOUserLogin userLoginDto)
        {
            _logger.LogInformation("Tentativa de login para usuário {User}", userLoginDto.Usuario);

            if (string.IsNullOrWhiteSpace(userLoginDto.Usuario)
                || string.IsNullOrWhiteSpace(userLoginDto.Senha))
            {
                _logger.LogWarning("Login inválido: parâmetros ausentes");
                return BadRequest(new { Mensagem = "Usuário e senha são obrigatórios." });
            }

            if (TokenService.IsUserLogged(userLoginDto.Usuario))
            {
                _logger.LogWarning("Usuário já logado: {User}", userLoginDto.Usuario);
                return Conflict(new { Mensagem = "Usuário já está logado. Por favor, faça logout e tente novamente." });
            }

            try
            {
                var (user, token) = await _userService.LoginUserAsync(userLoginDto);

                if (user == null)
                {
                    _logger.LogWarning("Conta inexistente para usuário {User}", userLoginDto.Usuario);
                    return NotFound(new { Mensagem = "Conta inexistente." });
                }

                TokenService.AddToken(user.Usuario, token);
                _logger.LogInformation("Login bem-sucedido para usuário {User}", user.Usuario);

                return Ok(new
                {
                    Token = token,
                    Usuario = user.Usuario,
                    Mensagem = "Login realizado com sucesso!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao tentar login para usuário {User}", userLoginDto.Usuario);
                return StatusCode(500, new
                {
                    Mensagem = "Ocorreu um erro interno no servidor.",
                    Detalhes = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            _logger.LogInformation("Iniciando logout");

            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                _logger.LogWarning("Logout inválido: token ausente ou malformado");
                return BadRequest(new { Mensagem = "Token inválido." });
            }

            var token = authHeader.Substring("Bearer ".Length);
            var removed = TokenService.RemoveToken(token);

            if (removed)
            {
                _logger.LogInformation("Logout realizado com sucesso");
                return Ok(new { Mensagem = "Logout realizado com sucesso." });
            }
            else
            {
                _logger.LogWarning("Logout falhou: token não encontrado ou expirado");
                return NotFound(new { Mensagem = "Token não encontrado ou já expirado." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            _logger.LogInformation("Consultando lista de usuários");

            var users = await _userService.GetUsersAsync();
            var result = users.Select(u => new { u.Id, u.Usuario, u.PermissionAccount });
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] DTOUser userDto)
        {
            var permission = User.Claims.FirstOrDefault(c => c.Type == "PermissionAccount")?.Value;
            _logger.LogInformation("Solicitação de atualização de usuário {UserId} por {Permission}", userDto.Id, permission);

            if (permission != "admin")
            {
                _logger.LogWarning("Acesso negado para atualizar usuário: {UserId}", userDto.Id);
                return Forbid("Somente administradores podem atualizar usuários.");
            }

            try
            {
                var user = await _userService.UpdateUserAsync(userDto);
                if (user == null)
                {
                    _logger.LogWarning("Usuário não encontrado para atualização: {UserId}", userDto.Id);
                    return NotFound(new { Mensagem = "Usuário não encontrado." });
                }

                _logger.LogInformation("Usuário atualizado com sucesso: {UserId}", userDto.Id);
                return Ok(new
                {
                    user.Id,
                    user.Usuario,
                    user.PermissionAccount,
                    Mensagem = "Usuário atualizado com sucesso!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuário {UserId}", userDto.Id);
                return StatusCode(500, new
                {
                    Mensagem = "Ocorreu um erro interno ao atualizar o usuário.",
                    Detalhes = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var permission = User.Claims.FirstOrDefault(c => c.Type == "PermissionAccount")?.Value;
            _logger.LogInformation("Solicitação de exclusão de usuário {UserId} por {Permission}", id, permission);

            if (permission != "admin")
            {
                _logger.LogWarning("Acesso negado para deletar usuário: {UserId}", id);
                return Forbid("Somente administradores podem deletar usuários.");
            }

            var exists = (await _userService.GetUsersAsync()).Any(u => u.Id == id);
            if (!exists)
            {
                _logger.LogWarning("Tentativa de deletar usuário inexistente: {UserId}", id);
                return NotFound(new { Mensagem = "Usuário não encontrado." });
            }

            await _userService.DeleteUserAsync(id);
            _logger.LogInformation("Usuário deletado com sucesso: {UserId}", id);
            return Ok(new { Mensagem = $"Usuário com ID {id} deletado com sucesso!" });
        }

        [HttpGet]
        public IActionResult ListActiveTokens()
        {
            _logger.LogInformation("Listando tokens ativos");

            var activeTokens = TokenService.GetActiveTokens();
            var jwtHandler = new JwtSecurityTokenHandler();

            var tokensList = activeTokens
                .Select(kvp =>
                {
                    var token = kvp.Value;
                    var jwt = jwtHandler.ReadToken(token) as JwtSecurityToken;
                    if (jwt == null) return null;

                    var remaining = jwt.ValidTo - DateTime.UtcNow;
                    var formatted = $"{remaining.Hours}h {remaining.Minutes}m {remaining.Seconds}s";

                    return new DTOActiveToken
                    {
                        Usuario = kvp.Key,
                        Token = token,
                        TempoRestante = formatted
                    };
                })
                .Where(x => x != null)
                .ToList();

            return Ok(tokensList);
        }

        [HttpGet]
        public IActionResult GetUserBearerToken()
        {
            _logger.LogInformation("Obtendo token bearer do usuário");

            var activeTokens = TokenService.GetActiveTokens();
            var jwtHandler = new JwtSecurityTokenHandler();

            var tokensList = activeTokens
                .Select(kvp =>
                {
                    var token = kvp.Value;
                    var jwt = jwtHandler.ReadToken(token) as JwtSecurityToken;
                    if (jwt == null) return null;

                    var permissionAccount = jwt.Claims.FirstOrDefault(c => c.Type == "PermissionAccount")?.Value;
                    if (permissionAccount == null) return null;

                    return new DTOUserPermission
                    {
                        Usuario = kvp.Key,
                        PermissionAccount = permissionAccount
                    };
                })
                .Where(x => x != null)
                .ToList();

            return Ok(tokensList);
        }
    }
}
