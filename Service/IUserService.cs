using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public interface IUserService
    {
        Task<UserModel> RegisterUserAsync(DTOUser userDto);
        Task<ClaimsPrincipal> ValidateUserAsync(DTOUserLogin userLoginDto);
        Task<List<UserModel>> GetUsersAsync();
        Task<UserModel> UpdateUserAsync(DTOUser userDto);
        Task DeleteUserAsync(int id);
        Task<UserModel> GetUserByIdAsync(int id);
        Task<UserModel> GetUserByUsernameAsync(string username);
        Task<bool> UpdatePasswordAsync(string usuario, string senhaAtual, string novaSenha);
        Task<UserModel> GetUserByEmailAsync(string email);
        Task SavePasswordResetTokenAsync(int userId, string token);
        Task<int?> GetUserIdByResetTokenAsync(string token);
        Task UpdatePasswordByIdAsync(int userId, string novaSenha);
        Task InvalidatePasswordResetTokenAsync(string token);
        Task<DateTime?> GetPasswordResetTokenExpirationAsync(string token);
    }
}
