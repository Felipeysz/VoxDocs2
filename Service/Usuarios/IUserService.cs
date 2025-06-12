using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public interface IUserService
    {
        // User Registration
        Task<(UserModel user, string? adminLimit, string? userLimit)> RegisterUserAsync(DTORegisterUser registerDto);
        
        // Authentication
        Task<ClaimsPrincipal> AuthenticateUserAsync(DTOLoginUser loginDto);
        
        // User Retrieval
        Task<UserModel> GetUserByIdAsync(Guid userId);
        Task<UserModel> GetUserByEmailOrUsernameAsync(string email, string username);
        Task<UserModel> GetUserByUsernameAsync(string username);
        Task<IEnumerable<UserModel>> GetAllUsersAsync();
        
        // Password Management
        Task<string> GeneratePasswordResetTokenAsync(Guid userId);
        Task RequestPasswordResetAsync(DTOResetPassword resetRequestDto);
        Task ResetPasswordWithTokenAsync(DTOResetPasswordWithToken resetDto);
        Task ChangePasswordAsync(DTOUserLoginPasswordChange changeDto);
        
        // User Management
        Task UpdateUserAsync(DTOUpdateUser updateDto);
        Task DeleteUserAsync(Guid userId);
        
        // Validation
        Task<bool> IsEmailAvailableAsync(string email);
        Task<bool> IsUsernameAvailableAsync(string username);
    }
}