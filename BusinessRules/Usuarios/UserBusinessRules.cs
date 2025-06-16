using VoxDocs.Data.Repositories;
using VoxDocs.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using VoxDocs.BusinessRules;
using System.Collections.Generic;

namespace VoxDocs.Services
{
    public class UserBusinessRules : IUserBusinessRules
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPlanosVoxDocsBusinessRules _planosBusinessRules;

        public UserBusinessRules(
            IUsuarioRepository usuarioRepository,
            IPlanosVoxDocsBusinessRules planosBusinessRules)
        {
            _usuarioRepository = usuarioRepository;
            _planosBusinessRules = planosBusinessRules;
        }

        public async Task<UserModel> CriarUsuarioAsync(UserModel usuario)
        {
            await ValidarUsuarioUnicoAsync(usuario);
            await ValidarLimitesPlanoAsync(usuario);
            return await _usuarioRepository.CriarUsuarioAsync(usuario);
        }

        public async Task<UserModel> ObterUsuarioPorEmailOuNomeAsync(string email, string username)
        {
            return await _usuarioRepository.ObterUsuarioPorEmailOuNomeAsync(email, username);
        }

        public async Task<UserModel> ObterUsuarioPorNomeAsync(string username)
        {
            return await _usuarioRepository.ObterUsuarioPorNomeAsync(username);
        }

        public async Task<UserModel> ObterUsuarioPorIdAsync(Guid userId)
        {
            return await _usuarioRepository.ObterUsuarioPorIdAsync(userId);
        }

        public async Task<IEnumerable<UserModel>> ObterTodosUsuariosAsync()
        {
            return await _usuarioRepository.ObterTodosUsuariosAsync();
        }

        public async Task AtualizarUsuarioAsync(UserModel usuario)
        {
            await ValidarLimitesPlanoAtualizacaoAsync(usuario);
            await _usuarioRepository.AtualizarUsuarioAsync(usuario);
        }

        public async Task ExcluirUsuarioAsync(Guid userId)
        {
            await _usuarioRepository.ExcluirUsuarioAsync(userId);
        }

        public async Task<string> GerarTokenRedefinicaoSenhaAsync(Guid userId)
        {
            var token = Guid.NewGuid().ToString();
            await _usuarioRepository.SalvarTokenRedefinicaoSenhaAsync(userId, token);
            return token;
        }

        public async Task SolicitarRedefinicaoSenhaAsync(string email)
        {
            var user = await _usuarioRepository.ObterUsuarioPorEmailAsync(email);
            if (user == null)
            {
                throw new KeyNotFoundException("Usuário não encontrado.");
            }

            var token = await GerarTokenRedefinicaoSenhaAsync(user.Id);
            // Implementar envio de email aqui
        }

        public async Task RedefinirSenhaComTokenAsync(string token, string novaSenhaHash)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorTokenRedefinicaoAsync(token);
            
            if (usuario == null || usuario.PasswordResetTokenExpiration < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Token inválido ou expirado.");
            }

            await _usuarioRepository.AtualizarSenhaAsync(usuario.Id, novaSenhaHash);
        }

        public async Task AlterarSenhaAsync(string username, string senhaAntigaHash, string novaSenhaHash)
        {
            var user = await ObterUsuarioPorNomeAsync(username);
            if (user == null || user.Senha != senhaAntigaHash)
            {
                throw new UnauthorizedAccessException("Credenciais inválidas.");
            }

            await _usuarioRepository.AtualizarSenhaAsync(user.Id, novaSenhaHash);
        }

        public async Task<ArmazenamentoUsuarioModel> ObterArmazenamentoUsuarioAsync(Guid userId)
        {
            return await _usuarioRepository.ObterArmazenamentoUsuarioAsync(userId);
        }

        public async Task<EstatisticasAdminModel> ObterEstatisticasAdminAsync()
        {
            var todosUsuarios = await _usuarioRepository.ObterTodosUsuariosAsync();
            var usuariosRecentes = await _usuarioRepository.ObterUsuariosRecentesAsync();

            return new EstatisticasAdminModel
            {
                TotalUsuarios = todosUsuarios.Count(),
                UsuariosAtivos = await _usuarioRepository.ContarUsuariosAtivosAsync(),
                TotalAdministradores = await _usuarioRepository.ContarAdministradoresAsync(),
                UsuariosRecentes = usuariosRecentes.ToList()
            };
        }

        public async Task ValidarUsuarioUnicoAsync(UserModel usuario)
        {
            var usuarioExistente = await _usuarioRepository.ObterUsuarioPorEmailOuNomeAsync(usuario.Email, usuario.Usuario);
            if (usuarioExistente != null)
            {
                throw new InvalidOperationException("Já existe um usuário com este email ou nome de usuário.");
            }
        }

        public async Task ValidarUsuarioExisteAsync(Guid idUsuario)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorIdAsync(idUsuario);
            if (usuario == null)
            {
                throw new KeyNotFoundException("Usuário não encontrado.");
            }
        }

        public async Task<(int administradores, int usuarios)> ValidarLimitesPlanoAsync(UserModel usuario)
        {
            if (string.IsNullOrEmpty(usuario.PlanoPago))
                return (0, 0);

            var (plano, error) = await _planosBusinessRules.GetPlanByNameWithValidationAsync(usuario.PlanoPago);
            if (error != null || plano == null)
                return (0, 0);

            var usuariosExistentes = await _usuarioRepository.ObterUsuariosPorPlanoAsync(usuario.PlanoPago);
            var administradores = usuariosExistentes.Count(u => u.PermissionAccount == "admin");
            var usuarios = usuariosExistentes.Count(u => u.PermissionAccount == "user");

            if (usuario.PermissionAccount == "admin" && administradores >= plano.LimiteAdmin)
            {
                throw new InvalidOperationException("Limite de administradores excedido para este plano.");
            }

            if (usuario.PermissionAccount == "user" && usuarios >= plano.LimiteUsuario)
            {
                throw new InvalidOperationException("Limite de usuários excedido para este plano.");
            }

            return (administradores, usuarios);
        }

        public async Task AtualizarUltimoLoginAsync(Guid userId)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorIdAsync(userId);
            if (usuario != null)
            {
                usuario.UltimoLogin = DateTime.UtcNow;
                await _usuarioRepository.AtualizarUsuarioAsync(usuario);
            }
        }

        public async Task ValidarLimitesPlanoAtualizacaoAsync(UserModel usuario)
        {
            if (string.IsNullOrEmpty(usuario.PlanoPago))
                return;

            var (plano, error) = await _planosBusinessRules.GetPlanByNameWithValidationAsync(usuario.PlanoPago);
            if (error != null || plano == null)
                return;

            var usuariosExistentes = await _usuarioRepository.ObterUsuariosPorPlanoAsync(usuario.PlanoPago);
            var administradores = usuariosExistentes.Count(u => u.PermissionAccount == "admin");
            var usuarios = usuariosExistentes.Count(u => u.PermissionAccount == "user");

            if (usuario.PermissionAccount == "admin" && administradores >= plano.LimiteAdmin)
            {
                throw new InvalidOperationException("Limite de administradores excedido para este plano.");
            }

            if (usuario.PermissionAccount == "user" && usuarios >= plano.LimiteUsuario)
            {
                throw new InvalidOperationException("Limite de usuários excedido para este plano.");
            }
        }
    }
}