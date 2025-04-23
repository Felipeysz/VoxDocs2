using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoxDocs.DTO;
using VoxDocs.Services;

namespace VoxDocs.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]  // ← Protege toda a controller
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // Registro de usuário - PÚBLICO, não precisa de token
        [HttpPost]
        [AllowAnonymous] // Permite acesso sem autenticação
        [Consumes("application/json")] // (opcional, mas bom para deixar claro que é JSON)
        public async Task<IActionResult> Register([FromBody] DTOUser userDto)
        {
            if (string.IsNullOrEmpty(userDto.Usuario)
            || string.IsNullOrEmpty(userDto.Senha)
            || string.IsNullOrEmpty(userDto.PermissionAccount))
                return BadRequest("Usuário, senha e permissão são obrigatórios.");

            var user = await _userService.RegisterUserAsync(userDto);

            return Ok(new
            {
                user.Id,
                user.Usuario,
                user.PermissionAccount,
                Mensagem = "Usuário criado com sucesso!"
            });
        }


        // POST api/User/Login
        [HttpPost]
        [AllowAnonymous]
        [Consumes("application/json")]
        public async Task<IActionResult> Login([FromBody] DTOUserLogin userLoginDto)
        {
            // 1) Validação de entrada
            if (string.IsNullOrWhiteSpace(userLoginDto.Usuario) ||
                string.IsNullOrWhiteSpace(userLoginDto.Senha))
            {
                return BadRequest(new
                {
                    Mensagem = "Usuário e senha são obrigatórios."
                });
            }

            // 2) Verifica se já está logado
            if (TokenService.IsUserLogged(userLoginDto.Usuario))
            {
                return Conflict(new
                {
                    Mensagem = "Usuário já está logado. Por favor, tente novamente."
                });
            }

            try
            {
                // 3) Tenta autenticar
                var (user, token) = await _userService.LoginUserAsync(userLoginDto);

                if (user == null)
                {
                    // Usuário não encontrado
                    return NotFound(new
                    {
                        Mensagem = "Conta Inexistente"
                    });
                }

                // 4) Adiciona token à sessão (ou cache)
                TokenService.AddToken(user.Usuario, token);

                // 5) Retorna sucesso com token
                return Ok(new
                {
                    Token = token,
                    Usuario = user.Usuario,
                    Mensagem = "Login realizado com sucesso!"
                });
            }
            catch (Exception ex)
            {
                // 6) Erro inesperado
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
            // Pega o token enviado no Authorization
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith("Bearer "))
                return BadRequest("Token inválido.");

            var token = authHeader.Substring("Bearer ".Length);

            var removed = TokenService.RemoveToken(token);

            if (removed)
                return Ok(new { Mensagem = "Logout realizado com sucesso." });
            else
                return NotFound(new { Mensagem = "Token não encontrado ou já expirado." });
        }

        // A partir daqui, TODOS exigem um Bearer válido:

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();
            var result = users.Select(u => new { u.Id, u.Usuario, u.PermissionAccount });
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] DTOUser userDto)
        {
            var permission = User.Claims.FirstOrDefault(c => c.Type == "PermissionAccount")?.Value;
            if (permission != "admin")
                return Forbid("Somente administradores podem atualizar usuários.");

            var user = await _userService.UpdateUserAsync(userDto);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            return Ok(new
            {
                user.Id,
                user.Usuario,
                user.PermissionAccount,
                Mensagem = "Usuário atualizado com sucesso!"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var permission = User.Claims.FirstOrDefault(c => c.Type == "PermissionAccount")?.Value;
            if (permission != "admin")
                return Forbid("Somente administradores podem deletar usuários.");

            var users = await _userService.GetUsersAsync();
            if (!users.Any(u => u.Id == id))
                return NotFound("Usuário não encontrado.");

            await _userService.DeleteUserAsync(id);
            return Ok(new { Mensagem = $"Usuário com ID {id} deletado com sucesso!" });
        }

        [HttpGet]
        public IActionResult ListActiveTokens()
        {
            var activeTokens = TokenService.GetActiveTokens();
            var jwtHandler = new JwtSecurityTokenHandler();

            var tokensList = activeTokens
                .Select(kvp =>
                {
                    var token = kvp.Value; // Agora pegamos só o token (o Value)
                    var jwt = jwtHandler.ReadToken(token) as JwtSecurityToken;
                    if (jwt == null) return null;

                    var userName = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                    var remaining = jwt.ValidTo - DateTime.UtcNow;
                    var formatted = $"{remaining.Hours}h {remaining.Minutes}m {remaining.Seconds}s";

                    return new DTOActiveToken
                    {
                        Usuario = kvp.Key,   // Aqui podemos usar o kvp.Key para pegar o nome do usuário
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
            var activeTokens = TokenService.GetActiveTokens();
            var jwtHandler = new JwtSecurityTokenHandler();

            var tokensList = activeTokens
                .Select(kvp =>
                {
                    var token = kvp.Value; // Pegamos o token (o Value)
                    var jwt = jwtHandler.ReadToken(token) as JwtSecurityToken;
                    if (jwt == null) return null;

                    var userName = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                    var permissionAccount = jwt.Claims.FirstOrDefault(c => c.Type == "PermissionAccount")?.Value; // Acesso à claim de permission

                    if (userName == null || permissionAccount == null) return null;

                    var remaining = jwt.ValidTo - DateTime.UtcNow;
                    var formatted = $"{remaining.Hours}h {remaining.Minutes}m {remaining.Seconds}s";

                    return new DTOUserPermission
                    {
                        Usuario = kvp.Key,   // Aqui usamos o kvp.Key para pegar o nome do usuário
                        PermissionAccount = permissionAccount // Preenchemos a permissão a partir da claim
                    };
                })
                .Where(x => x != null)
                .ToList();

            return Ok(tokensList);
        }
    }
}
