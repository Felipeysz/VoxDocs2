using VoxDocs.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoxDocs.Services
{
    public interface IUserBusinessRules
    {
        // User CRUD operations
        Task AtualizarUltimoLoginAsync(Guid userId);
        Task<UserModel> CriarUsuarioAsync(UserModel usuario);
        Task<UserModel> ObterUsuarioPorEmailOuNomeAsync(string email, string username);
        Task<UserModel> ObterUsuarioPorNomeAsync(string username);
        Task<UserModel> ObterUsuarioPorIdAsync(Guid userId);
        Task<IEnumerable<UserModel>> ObterTodosUsuariosAsync();
        Task AtualizarUsuarioAsync(UserModel usuario);
        Task ExcluirUsuarioAsync(Guid userId);

        // Password operations
        Task<string> GerarTokenRedefinicaoSenhaAsync(Guid userId);
        Task SolicitarRedefinicaoSenhaAsync(string email);
        Task RedefinirSenhaComTokenAsync(string token, string novaSenhaHash);
        Task AlterarSenhaAsync(string username, string senhaAntigaHash, string novaSenhaHash);

        // Storage and statistics
        Task<ArmazenamentoUsuarioModel> ObterArmazenamentoUsuarioAsync(Guid userId);
        Task<EstatisticasAdminModel> ObterEstatisticasAdminAsync();

        // Validation methods
        Task ValidarUsuarioUnicoAsync(UserModel usuario);
        Task ValidarUsuarioExisteAsync(Guid idUsuario);
        Task<(int administradores, int usuarios)> ValidarLimitesPlanoAsync(UserModel usuario);
        Task ValidarLimitesPlanoAtualizacaoAsync(UserModel usuario);
    }
}