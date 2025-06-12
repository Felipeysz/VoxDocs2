using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using VoxDocs.Data.Repositories;
using VoxDocs.DTO;
using VoxDocs.Helpers;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserBusinessRules _businessRules;
        private readonly IPlanosVoxDocsService _planosService;

        public UserService(
            IUserRepository userRepository,
            IUserBusinessRules businessRules,
            IPlanosVoxDocsService planosService)
        {
            _userRepository = userRepository;
            _businessRules = businessRules;
            _planosService = planosService;
        }

        public async Task<(UserModel user, string? adminLimit, string? userLimit)> RegisterUserAsync(DTORegisterUser registerDto)
        {
            await _businessRules.ValidateUniqueUserAsync(registerDto);
            var (admins, users) = await _businessRules.ValidatePlanLimitAsync(registerDto);

            var plano = (await _planosService.GetAllPlansAsync())
                .FirstOrDefault(p => p.Nome.Equals(registerDto.PlanoPago, StringComparison.OrdinalIgnoreCase));

            string? adminLimit = null;
            string? userLimit = null;

            if (plano != null)
            {
                if (registerDto.PermissionAccount == "admin") adminLimit = $"{admins + 1}/{plano.LimiteAdmin}";
                if (registerDto.PermissionAccount == "user") userLimit = $"{users + 1}/{plano.LimiteUsuario}";
            }

            var user = new UserModel
            {
                Id = Guid.NewGuid(),
                Usuario = registerDto.Usuario,
                Email = registerDto.Email,
                Senha = PasswordHelper.HashPassword(registerDto.Senha),
                PermissionAccount = registerDto.PermissionAccount,
                EmpresaContratante = registerDto.EmpresaContratante,
                PlanoPago = registerDto.PlanoPago,
                LimiteAdmin = adminLimit,
                LimiteUsuario = userLimit
            };

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return (user, adminLimit, userLimit);
        }

        public async Task<UserModel> GetUserByEmailOrUsernameAsync(string email, string username)
        {
            var user = await _userRepository.GetUserByEmailOrUsernameAsync(email, username);
            return user ?? throw new KeyNotFoundException("Usuário não encontrado.");
        }

        public async Task<UserModel> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            return user ?? throw new KeyNotFoundException("Usuário não encontrado.");
        }

        public async Task<ClaimsPrincipal> AuthenticateUserAsync(DTOLoginUser loginDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(loginDto.Usuario)
                ?? throw new KeyNotFoundException("Conta inexistente.");

            if (user.Senha != PasswordHelper.HashPassword(loginDto.Senha))
                throw new UnauthorizedAccessException("Senha incorreta.");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Usuario),
                new Claim("PermissionAccount", user.PermissionAccount)
            };

            var id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(id);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(Guid userId)
        {
            await _businessRules.ValidateUserExistsAsync(userId);
            var user = await _userRepository.GetUserByIdAsync(userId);

            var token = Guid.NewGuid().ToString("N");
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiration = DateTime.UtcNow.AddHours(1);

            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return token;
        }

        public async Task RequestPasswordResetAsync(DTOResetPassword resetRequestDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(resetRequestDto.Email)
                ?? throw new KeyNotFoundException("Email não cadastrado.");
            
            // This would typically send the reset email with the generated token
            await GeneratePasswordResetTokenAsync(user.Id);
        }

        public async Task ResetPasswordWithTokenAsync(DTOResetPasswordWithToken resetDto)
        {
            var user = await _userRepository.GetUserByResetTokenAsync(resetDto.Token)
                ?? throw new KeyNotFoundException("Token inválido ou expirado.");

            if (user.PasswordResetTokenExpiration < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Token expirado.");

            user.Senha = PasswordHelper.HashPassword(resetDto.NovaSenha);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiration = null;

            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(DTOUserLoginPasswordChange changeDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(changeDto.Usuario)
                ?? throw new KeyNotFoundException("Usuário não encontrado.");

            if (user.Senha != PasswordHelper.HashPassword(changeDto.SenhaAntiga))
                throw new UnauthorizedAccessException("Senha atual incorreta.");

            user.Senha = PasswordHelper.HashPassword(changeDto.NovaSenha);
            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(DTOUpdateUser updateDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(updateDto.Usuario)
                ?? throw new KeyNotFoundException("Usuário não encontrado.");

            await _businessRules.ValidatePlanLimitForUpdateAsync(updateDto);

            user.Email = updateDto.Email;
            user.PermissionAccount = updateDto.PermissionAccount;
            user.EmpresaContratante = updateDto.EmpresaContratante;
            user.PlanoPago = updateDto.PlanoPago;

            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserModel>> GetAllUsersAsync() => 
            await _userRepository.GetAllUsersAsync();

        public async Task<UserModel> GetUserByIdAsync(Guid userId)
        {
            await _businessRules.ValidateUserExistsAsync(userId);
            return await _userRepository.GetUserByIdAsync(userId);
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            await _businessRules.ValidateUserExistsAsync(userId);
            var user = await _userRepository.GetUserByIdAsync(userId);
            
            await _userRepository.DeleteUserAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            return user == null;
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            return user == null;
        }
    }
}