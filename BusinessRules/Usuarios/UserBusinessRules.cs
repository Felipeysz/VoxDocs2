using VoxDocs.Data.Repositories;
using VoxDocs.DTO;

namespace VoxDocs.Services
{
    public class UserBusinessRules : IUserBusinessRules
    {
        private readonly IUserRepository _userRepository;
        private readonly IPlanosVoxDocsService _planosService;

        public UserBusinessRules(IUserRepository userRepository, IPlanosVoxDocsService planosService)
        {
            _userRepository = userRepository;
            _planosService = planosService;
        }

        public async Task ValidateUniqueUserAsync(DTORegisterUser registerDto)
        {
            var existingUser = await _userRepository.GetUserByEmailOrUsernameAsync(registerDto.Email, registerDto.Usuario);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Usuário ou email já cadastrado.");
            }
        }

        public async Task ValidateUserExistsAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Usuário não encontrado.");
            }
        }

        public async Task<(int admins, int users)> ValidatePlanLimitAsync(DTORegisterUser registerDto)
        {
            if (string.IsNullOrEmpty(registerDto.PlanoPago))
            return (0, 0);

            var plano = (await _planosService.GetAllPlansAsync())
                .FirstOrDefault(p => p.Nome.Equals(registerDto.PlanoPago, StringComparison.OrdinalIgnoreCase));

            if (plano == null) return (0, 0);

            var existing = await _userRepository.GetUsersByPlanAsync(registerDto.PlanoPago);
            var admins = existing.Count(u => u.PermissionAccount == "admin");
            var users = existing.Count(u => u.PermissionAccount == "user");

            if (registerDto.PermissionAccount == "admin" && admins >= plano.LimiteAdmin)
            {
                throw new InvalidOperationException("Limite de administradores excedido para este plano.");
            }

            if (registerDto.PermissionAccount == "user" && users >= plano.LimiteUsuario)
            {
                throw new InvalidOperationException("Limite de usuários excedido para este plano.");
            }

            return (admins, users);
        }

        // Additional method for update validation
        public async Task ValidatePlanLimitForUpdateAsync(DTOUpdateUser updateDto)
        {
            if (string.IsNullOrEmpty(updateDto.PlanoPago))
                return;

            var plano = (await _planosService.GetAllPlansAsync())
                .FirstOrDefault(p => p.Nome.Equals(updateDto.PlanoPago, StringComparison.OrdinalIgnoreCase));

            if (plano == null) return;

            var existing = await _userRepository.GetUsersByPlanAsync(updateDto.PlanoPago);
            var admins = existing.Count(u => u.PermissionAccount == "admin");
            var users = existing.Count(u => u.PermissionAccount == "user");

            if (updateDto.PermissionAccount == "admin" && admins >= plano.LimiteAdmin)
            {
                throw new InvalidOperationException("Limite de administradores excedido para este plano.");
            }

            if (updateDto.PermissionAccount == "user" && users >= plano.LimiteUsuario)
            {
                throw new InvalidOperationException("Limite de usuários excedido para este plano.");
            }
        }
    }
}