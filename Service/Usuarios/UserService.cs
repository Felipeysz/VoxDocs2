using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using VoxDocs.BusinessRules;
using VoxDocs.DTO;
using VoxDocs.Helpers;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public class UserService : IUserService
    {
        private readonly IUserBusinessRules _businessRules;
        private readonly IEmpresasContratanteBusinessRules _businessRulesEmpresa;

        public UserService(IUserBusinessRules businessRules, IEmpresasContratanteBusinessRules businessRulesEmpresa)
        {
            _businessRules = businessRules;
            _businessRulesEmpresa = businessRulesEmpresa;
        }

        public async Task<(DTOUsuarioInfo user, string? adminLimit, string? userLimit)> RegisterUserAsync(DTORegistrarUsuario registerDto)
        {
            // Primeiro verifica se a empresa existe
            var empresa = await _businessRulesEmpresa.ValidarGetByNomeAsync(registerDto.EmpresaContratante);
            if (empresa == null || !empresa.IsValid)
            {
                throw new InvalidOperationException("Empresa inexistente");
            }

            var userModel = new UserModel
            {
                Id = Guid.NewGuid(),
                Usuario = registerDto.Usuario,
                Email = registerDto.Email,
                PermissionAccount = registerDto.PermissaoConta,
                PlanoPago = registerDto.PlanoPago,
                Senha = PasswordHelper.HashPassword(registerDto.Senha),
                EmpresaContratante = registerDto.EmpresaContratante,
                DataCriacao = DateTime.UtcNow,
                Ativo = true
            };

            var (admins, users) = await _businessRules.ValidarLimitesPlanoAsync(userModel);
            
            userModel.LimiteAdmin = registerDto.PermissaoConta == "admin" ? $"{admins + 1}/{userModel.PlanoPago}" : null;
            userModel.LimiteUsuario = registerDto.PermissaoConta == "user" ? $"{users + 1}/{userModel.PlanoPago}" : null;

            var createdUser = await _businessRules.CriarUsuarioAsync(userModel);
            
            return (ToUsuarioInfoDto(createdUser), userModel.LimiteAdmin, userModel.LimiteUsuario);
        }

        public async Task<DTOUsuarioInfo> GetUserByEmailOrUsernameAsync(string email, string username)
        {
            var user = await _businessRules.ObterUsuarioPorEmailOuNomeAsync(email, username);
            return ToUsuarioInfoDto(user);
        }

        public async Task<DTOUsuarioInfo> GetUserByUsernameAsync(string username)
        {
            var user = await _businessRules.ObterUsuarioPorNomeAsync(username);
            return ToUsuarioInfoDto(user);
        }

        public async Task<ClaimsPrincipal> AuthenticateUserAsync(DTOLoginUsuario loginDto)
        {
            var user = await _businessRules.ObterUsuarioPorNomeAsync(loginDto.Usuario);
            
            if (user == null || !user.Ativo || user.Senha != PasswordHelper.HashPassword(loginDto.Senha))
            {
                throw new UnauthorizedAccessException("Credenciais inválidas ou conta inativa.");
            }

            // Update last login - sem verificação de limites
            user.UltimoLogin = DateTime.UtcNow;

            await _businessRules.AtualizarUltimoLoginAsync(user.Id); // Chamada direta ao repositório para evitar validações

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Usuario),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("PermissionAccount", user.PermissionAccount),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(id);
        }
        public async Task<string> GeneratePasswordResetTokenAsync(Guid userId)
        {
            return await _businessRules.GerarTokenRedefinicaoSenhaAsync(userId);
        }

        public async Task RequestPasswordResetAsync(string email)
        {
            await _businessRules.SolicitarRedefinicaoSenhaAsync(email);
        }

        public async Task ResetPasswordWithTokenAsync(string token, string novaSenha)
        {
            await _businessRules.RedefinirSenhaComTokenAsync(
                token, 
                PasswordHelper.HashPassword(novaSenha));
        }

        public async Task ChangePasswordAsync(string username, string senhaAntiga, string novaSenha)
        {
            await _businessRules.AlterarSenhaAsync(
                username,
                PasswordHelper.HashPassword(senhaAntiga),
                PasswordHelper.HashPassword(novaSenha));
        }

        public async Task UpdateUserAsync(DTOAtualizarUsuario updateDto)
        {
            var existingUser = await _businessRules.ObterUsuarioPorNomeAsync(updateDto.Usuario) ?? 
                throw new KeyNotFoundException("Usuário não encontrado.");

            var userModel = new UserModel
            {
                Id = existingUser.Id,
                Usuario = updateDto.Usuario,
                Email = updateDto.Email,
                PermissionAccount = updateDto.PermissaoConta,
                EmpresaContratante = updateDto.EmpresaContratante,
                PlanoPago = updateDto.PlanoPago,
                LimiteAdmin = updateDto.LimiteAdmin,
                LimiteUsuario = updateDto.LimiteUsuario,
                Senha = existingUser.Senha, // Preserve existing password
                DataCriacao = existingUser.DataCriacao,
                UltimoLogin = existingUser.UltimoLogin,
                Ativo = updateDto.Ativo
            };

            await _businessRules.AtualizarUsuarioAsync(userModel);
        }

        public async Task<IEnumerable<DTOUsuarioInfo>> GetAllUsersAsync()
        {
            var users = await _businessRules.ObterTodosUsuariosAsync();
            return users.Select(ToUsuarioInfoDto);
        }

        public async Task<DTOUsuarioInfo> GetUserByIdAsync(Guid userId)
        {
            var user = await _businessRules.ObterUsuarioPorIdAsync(userId);
            return ToUsuarioInfoDto(user);
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            await _businessRules.ExcluirUsuarioAsync(userId);
        }

        public async Task<bool> IsEmailAvailableAsync(string email, Guid? excludeUserId = null)
        {
            try
            {
                var existingUser = await _businessRules.ObterUsuarioPorEmailOuNomeAsync(email, null);
                return existingUser == null || (excludeUserId.HasValue && existingUser.Id == excludeUserId);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsUsernameAvailableAsync(string username, Guid? excludeUserId = null)
        {
            try
            {
                var existingUser = await _businessRules.ObterUsuarioPorNomeAsync(username);
                return existingUser == null || (excludeUserId.HasValue && existingUser.Id == excludeUserId);
            }
            catch
            {
                return false;
            }
        }

        public async Task<DTOArmazenamentoUsuario> GetUserStorageInfoAsync(Guid userId)
        {
            var storageInfo = await _businessRules.ObterArmazenamentoUsuarioAsync(userId);
            return new DTOArmazenamentoUsuario
            {
                UsoArmazenamento = storageInfo?.UsoArmazenamento ?? 0,
                LimiteArmazenamento = storageInfo?.LimiteArmazenamento ?? 0
            };
        }

        public async Task<DTOEstatisticasAdmin> GetAdminStatisticsAsync()
        {
            var stats = await _businessRules.ObterEstatisticasAdminAsync();
            return new DTOEstatisticasAdmin
            {
                TotalUsuarios = stats.TotalUsuarios,
                UsuariosAtivos = stats.UsuariosAtivos,
                TotalAdministradores = stats.TotalAdministradores,
                UsuariosRecentes = stats.UsuariosRecentes?.Select(ToUsuarioInfoDto).ToList() ?? new List<DTOUsuarioInfo>()
            };
        }

        public async Task ToggleUserStatusAsync(Guid userId, bool ativo)
        {
            var user = await _businessRules.ObterUsuarioPorIdAsync(userId) ?? 
                throw new KeyNotFoundException("Usuário não encontrado.");
            
            user.Ativo = ativo;
            await _businessRules.AtualizarUsuarioAsync(user);
        }

        private static DTOUsuarioInfo ToUsuarioInfoDto(UserModel user)
        {
            if (user == null) return null;

            return new DTOUsuarioInfo
            {
                Id = user.Id,
                Usuario = user.Usuario,
                Email = user.Email,
                PermissaoConta = user.PermissionAccount,
                EmpresaContratante = user.EmpresaContratante,
                Plano = user.PlanoPago,
                LimiteAdmin = user.LimiteAdmin,
                LimiteUsuario = user.LimiteUsuario,
                DataCriacao = user.DataCriacao,
                UltimoLogin = user.UltimoLogin,
                Ativo = user.Ativo
            };
        }
    }
}