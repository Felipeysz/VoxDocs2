using VoxDocs.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoxDocs.Data.Repositories
{
    public interface IUsuarioRepository
    {
        // User CRUD
        Task<UserModel> CriarUsuarioAsync(UserModel usuario);
        Task<UserModel> ObterUsuarioPorIdAsync(Guid id);
        Task AtualizarUsuarioAsync(UserModel usuario);
        Task ExcluirUsuarioAsync(Guid userId);
        
        // User queries
        Task<UserModel> ObterUsuarioPorNomeAsync(string nome);
        Task<UserModel> ObterUsuarioPorEmailAsync(string email);
        Task<UserModel> ObterUsuarioPorEmailOuNomeAsync(string email, string nome);
        Task<IEnumerable<UserModel>> ObterTodosUsuariosAsync();
        Task<IEnumerable<UserModel>> ObterUsuariosPorPlanoAsync(string planoPago);
        Task<IEnumerable<UserModel>> ObterUsuariosPorEmpresaAsync(string empresaNome);
        
        // Password operations
        Task SalvarTokenRedefinicaoSenhaAsync(Guid userId, string token);
        Task<UserModel> ObterUsuarioPorTokenRedefinicaoAsync(string token);
        Task AtualizarSenhaAsync(Guid userId, string novaSenhaHash);
        Task RemoverTokenRedefinicaoSenhaAsync(Guid userId);
        
        // Statistics
        Task<ArmazenamentoUsuarioModel> ObterArmazenamentoUsuarioAsync(Guid userId);
        Task<int> ContarUsuariosAtivosAsync();
        Task<int> ContarAdministradoresAsync();
        Task<IEnumerable<UserModel>> ObterUsuariosRecentesAsync(int quantidade = 5);
    }
}