using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using VoxDocs.DTO;

namespace VoxDocs.Services
{
    public interface IUserService
    {
        // User registration and authentication
        Task<(DTOUsuarioInfo user, string? adminLimit, string? userLimit)> RegisterUserAsync(DTORegistrarUsuario registerDto);
        Task<ClaimsPrincipal> AuthenticateUserAsync(DTOLoginUsuario loginDto);
        
        // User retrieval
        Task<DTOUsuarioInfo> GetUserByEmailOrUsernameAsync(string email, string username);
        Task<DTOUsuarioInfo> GetUserByUsernameAsync(string username);
        Task<DTOUsuarioInfo> GetUserByIdAsync(Guid userId);
        Task<IEnumerable<DTOUsuarioInfo>> GetAllUsersAsync();
        
        // Password management
        Task<string> GeneratePasswordResetTokenAsync(Guid userId);
        Task RequestPasswordResetAsync(string email);
        Task ResetPasswordWithTokenAsync(string token, string novaSenha);
        Task ChangePasswordAsync(string username, string senhaAntiga, string novaSenha);
        
        // User management
        Task UpdateUserAsync(DTOAtualizarUsuario updateDto);
        Task DeleteUserAsync(Guid userId);
        Task ToggleUserStatusAsync(Guid userId, bool ativo);
        
        // Validation and checks
        Task<bool> IsEmailAvailableAsync(string email, Guid? excludeUserId = null);
        Task<bool> IsUsernameAvailableAsync(string username, Guid? excludeUserId = null);
        
        // Storage and admin features
        Task<DTOArmazenamentoUsuario> GetUserStorageInfoAsync(Guid userId);
        Task<DTOEstatisticasAdmin> GetAdminStatisticsAsync();
    }
}