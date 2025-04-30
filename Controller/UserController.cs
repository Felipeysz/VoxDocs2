using System;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VoxDocs.DTO;
using VoxDocs.Services;
using VoxDocs.BusinessRules;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace VoxDocs.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly UserBusinessRules _rules;

        public UserController(
            ILogger<UserController> logger,
            IUserService userService,
            UserBusinessRules rules)
        {
            _logger = logger;
            _userService = userService;
            _rules = rules;
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] DTOUser dto)
        {
            _logger.LogInformation("Register attempt for {@User}", dto);

            try
            {
                // Chama a validação das regras de negócios
                _rules.ValidateRegister(dto);

                var user = await _userService.RegisterUserAsync(dto);

                return Ok(new
                {
                    user.Id,
                    user.Usuario,
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

                // Verifica se já existe um usuário logado com sessão ativa
                if (TokenService.IsUserLogged(dto.Usuario))
                {
                    _logger.LogWarning("Login rejeitado: conta já está ativa para {User}", dto.Usuario);
                    return BadRequest(new { mensagem = "Já existe um usuário utilizando essa conta (Conta ativa no momento)." });
                }

                var (user, token) = await _userService.LoginUserAsync(dto);

                // Tenta adicionar o token se ainda não houver um ativo
                if (!TokenService.AddTokenIfNotExists(user.Usuario, token))
                {
                    _logger.LogWarning("Token já existente para {User}", user.Usuario);
                    return BadRequest(new { mensagem = "Já existe um usuário utilizando essa conta (Conta ativa no momento)." });
                }

                return Ok(new
                {
                    token,
                    user,
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
                return BadRequest(new { mensagem = ex.Message });
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

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Logout()
        {
            var auth = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(auth) || !auth.StartsWith("Bearer "))
                return BadRequest(new { mensagem = "Token inválido." });

            var removed = TokenService.RemoveToken(auth.Substring(7));
            if (!removed)
                return NotFound(new { mensagem = "Token não encontrado ou expirado." });

            return Ok(new { mensagem = "Logout realizado com sucesso." });
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var list = await _userService.GetUsersAsync();
            return Ok(list.Select(u => new { u.Id, u.Usuario, u.PermissionAccount }));
        }

        [HttpPut("{id}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] DTOUser dto)
        {
            _logger.LogInformation("Update attempt for {UserId}", id);
            try
            {
                dto.Id = id;
                _rules.ValidateUpdate(dto);

                var user = await _userService.UpdateUserAsync(dto);
                return Ok(new { user.Id, user.Usuario, user.PermissionAccount, mensagem = "Usuário atualizado com sucesso!" });
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

        [HttpDelete("{id}"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
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

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ListActiveTokens()
        {
            var user = HttpContext.User;

            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { mensagem = "Token ausente ou inválido." });
            }

            if (!user.IsInRole("admin") && !user.HasClaim("PermissionAccount", "admin"))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Somente contas admin têm permissão para verificar os tokens válidos.");
            }

            var tokens = TokenService.GetActiveTokens();
            var dto = new List<DTOActiveToken>();
            var handler = new JwtSecurityTokenHandler();

            foreach (var kvp in tokens)
            {
                var token = kvp.Value;
                if (!_rules.ValidateActiveToken(token)) continue;

                var jwt = handler.ReadToken(token) as JwtSecurityToken;
                if (jwt == null) continue;

                var rem = jwt.ValidTo - DateTime.UtcNow;

                dto.Add(new DTOActiveToken
                {
                    Usuario = kvp.Key,
                    Token = token,
                    TempoRestante = $"{rem.Hours}h {rem.Minutes}m"
                });
            }

            if (dto.Count == 0)
            {
                return NotFound("Sem tokens ativos.");
            }

            return Ok(dto);
        }

    }
}
