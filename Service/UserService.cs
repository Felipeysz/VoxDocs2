using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VoxDocs.Data;
using VoxDocs.DTO;
using VoxDocs.Helpers;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public class UserService
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

        public async Task<(UserModel user, string token)> LoginUserAsync(DTOUserLogin userLoginDto)
        {
            string senhaCriptografada = PasswordHelper.HashPassword(userLoginDto.Senha);

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Usuario == userLoginDto.Usuario &&
                u.Senha == senhaCriptografada
            );

            if (user == null)
                return (null, null);

            var token = GenerateJwtToken(user);
            return (user, token);
        }


        public async Task<List<UserModel>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<UserModel> UpdateUserAsync(DTOUser userDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Usuario == userDto.Usuario);

            if (user == null)
                return null;

            if (!string.IsNullOrEmpty(userDto.Senha))
                user.Senha = PasswordHelper.HashPassword(userDto.Senha);

            if (!string.IsNullOrEmpty(userDto.PermissionAccount))
                user.PermissionAccount = userDto.PermissionAccount;

            await _context.SaveChangesAsync();
            return user;
        }

        private string GenerateJwtToken(UserModel user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
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

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
