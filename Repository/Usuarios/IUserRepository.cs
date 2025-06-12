using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoxDocs.Models;

namespace VoxDocs.Data.Repositories
{
    public interface IUserRepository
    {
        Task AddUserAsync(UserModel user);
        Task<UserModel> GetUserByIdAsync(Guid id);
        Task<UserModel> GetUserByUsernameAsync(string username);
        Task<UserModel> GetUserByEmailAsync(string email); // New method
        Task<UserModel> GetUserByEmailOrUsernameAsync(string email, string username);
        Task<List<UserModel>> GetAllUsersAsync();
        Task<List<UserModel>> GetUsersByPlanAsync(string planoPago);
        Task UpdateUserAsync(UserModel user);
        Task DeleteUserAsync(UserModel user);
        Task SaveChangesAsync();
        Task<UserModel> GetUserByResetTokenAsync(string token);
    }
}