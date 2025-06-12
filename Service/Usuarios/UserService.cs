using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using VoxDocs.Data;
using VoxDocs.DTO;
using VoxDocs.Helpers;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public class UserService : IUserService
    {
        private readonly VoxDocsContext _context;
        private readonly IPlanosVoxDocsService _planosService;

        public UserService(
            VoxDocsContext context,
            IPlanosVoxDocsService planosService)
        {
            _context = context;
            _planosService = planosService;
        }

        public async Task<(UserModel user, string? limiteAdmin, string? limiteUsuario)> RegisterAsync(DTORegisterUser dto)
        {

            var plano = (await _planosService.GetAllPlansAsync())
                .FirstOrDefault(p => p.Nome.Equals(dto.PlanoPago, StringComparison.OrdinalIgnoreCase));

            var existing = await _context.Users
                .Where(u => u.PlanoPago == dto.PlanoPago)
                .ToListAsync();

            var admins = existing.Count(u => u.PermissionAccount == "admin");
            var users = existing.Count(u => u.PermissionAccount == "user");

            string? limAdmin = plano is null ? null :
                dto.PermissionAccount == "admin"
                    ? $"{admins + 1}/{plano.LimiteAdmin}" : null;

            string? limUsuario = plano is null ? null :
                dto.PermissionAccount == "user"
                    ? $"{users + 1}/{plano.LimiteUsuario}" : null;

            var user = new UserModel
            {
                Id = Guid.NewGuid(),
                Usuario = dto.Usuario,
                Email = dto.Email,
                Senha = PasswordHelper.HashPassword(dto.Senha),
                PermissionAccount = dto.PermissionAccount,
                EmpresaContratante = dto.EmpresaContratante,
                PlanoPago = dto.PlanoPago,
                LimiteAdmin = limAdmin,
                LimiteUsuario = limUsuario
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return (user, limAdmin, limUsuario);
        }

        public async Task<UserModel> GetUserByEmailOrUsername(string email, string usuario)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email || u.Usuario == usuario)
                ?? throw new KeyNotFoundException("Usuário não encontrado.");

            return user;
        }

        public async Task<UserModel> GetUserByNameAsync(string usuario)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Usuario == usuario)
                ?? throw new KeyNotFoundException("Usuário não encontrado.");

            return user;
        }

        public async Task<ClaimsPrincipal> AuthenticateAsync(DTOLoginUser dto)
        {

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Usuario == dto.Usuario)
                ?? throw new KeyNotFoundException("Conta inexistente.");

            if (user.Senha != PasswordHelper.HashPassword(dto.Senha))
                throw new UnauthorizedAccessException("Senha incorreta.");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Usuario),
                new Claim("PermissionAccount", user.PermissionAccount)
            };

            var id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(id);
        }

        public async Task<string> GenerateResetTokenAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId)
                ?? throw new KeyNotFoundException("Usuário não encontrado.");

            var token = Guid.NewGuid().ToString("N");
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiration = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();
            return token;
        }

        public async Task ResetPasswordAsync(DTOResetPasswordWithToken dto)
        {

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.PasswordResetToken == dto.Token &&
                    u.PasswordResetTokenExpiration > DateTime.UtcNow)
                ?? throw new KeyNotFoundException("Token inválido ou expirado.");

            if (dto.NovaSenha == null)
                return;

            user.Senha = PasswordHelper.HashPassword(dto.NovaSenha);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiration = null;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DTOUpdateUser dto)
        {

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Usuario == dto.Usuario)
                ?? throw new KeyNotFoundException("Usuário não encontrado.");

            user.Email = dto.Email;
            user.PermissionAccount = dto.PermissionAccount;
            user.EmpresaContratante = dto.EmpresaContratante;
            user.PlanoPago = dto.PlanoPago;

            await _context.SaveChangesAsync();
        }

        public async Task<List<UserModel>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<UserModel> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id)
                ?? throw new KeyNotFoundException("Usuário não encontrado.");
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id)
                ?? throw new KeyNotFoundException("Usuário não encontrado.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

    }
}