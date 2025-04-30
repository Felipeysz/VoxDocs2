// Services/IUserService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public interface IUserService
    {
        Task<UserModel> RegisterUserAsync(DTOUser userDto);
        Task<(UserModel user, string token)> LoginUserAsync(DTOUserLogin userLoginDto);
        Task<List<UserModel>> GetUsersAsync();
        Task<UserModel> UpdateUserAsync(DTOUser userDto);
        Task DeleteUserAsync(int id);
    }
}
