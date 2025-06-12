using Microsoft.EntityFrameworkCore;
using VoxDocs.Data;
using VoxDocs.Models;

namespace VoxDocs.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly VoxDocsContext _context;

        public UserRepository(VoxDocsContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(UserModel user) => await _context.Users.AddAsync(user);

        public async Task<UserModel> GetUserByIdAsync(Guid id) => await _context.Users.FindAsync(id);

        public async Task<UserModel> GetUserByUsernameAsync(string username) => 
            await _context.Users.FirstOrDefaultAsync(u => u.Usuario == username);

        public async Task<UserModel> GetUserByEmailAsync(string email) => // New implementation
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<UserModel> GetUserByEmailOrUsernameAsync(string email, string username) => 
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email || u.Usuario == username);

        public async Task<List<UserModel>> GetAllUsersAsync() => await _context.Users.ToListAsync();

        public async Task<List<UserModel>> GetUsersByPlanAsync(string planoPago) => 
            await _context.Users.Where(u => u.PlanoPago == planoPago).ToListAsync();

        public async Task UpdateUserAsync(UserModel user) => _context.Users.Update(user);

        public async Task DeleteUserAsync(UserModel user) => _context.Users.Remove(user);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<UserModel> GetUserByResetTokenAsync(string token) => 
            await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == token);
    }
}