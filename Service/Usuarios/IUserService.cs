using System.Security.Claims;
using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public interface IUserService
    {
        Task<(UserModel user, string? limiteAdmin, string? limiteUsuario)> RegisterAsync(DTORegisterUser dto);
        Task<ClaimsPrincipal> AuthenticateAsync(DTOLoginUser dto);
        Task<string> GenerateResetTokenAsync(int userId);
        Task ResetPasswordAsync(DTOResetPasswordWithToken dto);
        Task UpdateAsync(DTOUpdateUser dto);
        Task<List<UserModel>> GetUsersAsync();
        Task<UserModel> GetUserByIdAsync(int id);
        Task<UserModel> GetUserByNameAsync(string usuario); // ✅ Novo método
        Task<UserModel> GetUserByEmailOrUsername(string email, string usuario);
        Task DeleteUserAsync(int id);
    }
}
