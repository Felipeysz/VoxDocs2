// Services/UserService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using VoxDocs.Data;
using VoxDocs.DTO;
using VoxDocs.Helpers;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public class UserService : IUserService
    {
        private readonly VoxDocsContext _context;
        private readonly string _jwtSecretKey;

        public UserService(VoxDocsContext context, IConfiguration configuration)
        {
            _context = context;
            _jwtSecretKey = configuration["Jwt:Key"];            
        }

        public async Task<UserModel> RegisterUserAsync(DTOUser userDto)
        {
            var user = new UserModel
            {
                Usuario = userDto.Usuario,
                Senha = PasswordHelper.HashPassword(userDto.Senha),
                PermissionAccount = userDto.PermissionAccount
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<(UserModel user, string token)> LoginUserAsync(DTOUserLogin dto)
        {
            // 1) Verifica existência de usuário
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Usuario == dto.Usuario);
            if (user == null)
                throw new KeyNotFoundException("Conta inexistente");

            // 2) Verifica senha
            var hashed = PasswordHelper.HashPassword(dto.Senha);
            if (user.Senha != hashed)
                throw new UnauthorizedAccessException("Senha incorreta");

            // 3) Gera JWT
            var token = GenerateJwtToken(user);
            return (user, token);
        }

        public async Task<List<UserModel>> GetUsersAsync()
            => await _context.Users.ToListAsync();

        public async Task<UserModel> UpdateUserAsync(DTOUser dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Usuario == dto.Usuario);
            if (user == null)
                throw new KeyNotFoundException("Usuário não encontrado");

            if (!string.IsNullOrWhiteSpace(dto.Senha))
                user.Senha = PasswordHelper.HashPassword(dto.Senha);
            if (!string.IsNullOrWhiteSpace(dto.PermissionAccount))
                user.PermissionAccount = dto.PermissionAccount;

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                throw new KeyNotFoundException("Usuário não encontrado");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        private string GenerateJwtToken(UserModel user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Usuario),
                new Claim("PermissionAccount", user.PermissionAccount)
            };
            var token = new JwtSecurityToken(
                issuer: "VoxDocs",
                audience: "VoxDocs",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
