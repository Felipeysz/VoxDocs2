using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoxDocs.Data;
using VoxDocs.Models;

namespace VoxDocs.Data.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly VoxDocsContext _context;

        public UsuarioRepository(VoxDocsContext context)
        {
            _context = context;
        }

        public async Task<UserModel> CriarUsuarioAsync(UserModel usuario)
        {
            await _context.Users.AddAsync(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<UserModel> ObterUsuarioPorIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task AtualizarUsuarioAsync(UserModel usuario)
        {
            _context.Users.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task ExcluirUsuarioAsync(Guid userId)
        {
            var usuario = await _context.Users.FindAsync(userId);
            if (usuario != null)
            {
                _context.Users.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UserModel> ObterUsuarioPorNomeAsync(string nome)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Usuario == nome);
        }

        public async Task<UserModel> ObterUsuarioPorEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserModel> ObterUsuarioPorEmailOuNomeAsync(string email, string nome)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email || u.Usuario == nome);
        }

        public async Task<IEnumerable<UserModel>> ObterTodosUsuariosAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<IEnumerable<UserModel>> ObterUsuariosPorPlanoAsync(string planoPago)
        {
            return await _context.Users
                .Where(u => u.PlanoPago == planoPago)
                .ToListAsync();
        }

        public async Task<IEnumerable<UserModel>> ObterUsuariosPorEmpresaAsync(string empresaNome)
        {
            return await _context.Users
                .Where(u => u.EmpresaContratante == empresaNome)
                .ToListAsync();
        }

        public async Task SalvarTokenRedefinicaoSenhaAsync(Guid userId, string token)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.PasswordResetToken = token;
                user.PasswordResetTokenExpiration = DateTime.UtcNow.AddHours(24);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UserModel> ObterUsuarioPorTokenRedefinicaoAsync(string token)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.PasswordResetToken == token && 
                                        u.PasswordResetTokenExpiration > DateTime.UtcNow);
        }

        public async Task AtualizarSenhaAsync(Guid userId, string novaSenhaHash)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Senha = novaSenhaHash;
                user.PasswordResetToken = null;
                user.PasswordResetTokenExpiration = null;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoverTokenRedefinicaoSenhaAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.PasswordResetToken = null;
                user.PasswordResetTokenExpiration = null;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ArmazenamentoUsuarioModel> ObterArmazenamentoUsuarioAsync(Guid userId)
        {
            // Verifica se existe registro para o usuário
            var armazenamento = await _context.UserStorage
                .FirstOrDefaultAsync(s => s.UserId == userId);

            // Se não existir, cria um novo com valores padrão
            if (armazenamento == null)
            {
                // Obter o usuário para verificar o plano
                var usuario = await _context.Users.FindAsync(userId);
                if (usuario == null)
                    throw new KeyNotFoundException("Usuário não encontrado.");

                // Obter o plano do usuário para definir o limite padrão
                var plano = await _context.PlanosVoxDocs
                    .FirstOrDefaultAsync(p => p.Nome == usuario.PlanoPago);

                armazenamento = new ArmazenamentoUsuarioModel
                {
                    UserId = userId,
                    UsoArmazenamento = 0,
                    LimiteArmazenamento = plano?.ArmazenamentoDisponivel ?? 10 // Default 10MB se plano não encontrado
                };

                // Salva o novo registro
                await _context.UserStorage.AddAsync(armazenamento);
                await _context.SaveChangesAsync();
            }

            return armazenamento;
        }

        public async Task<int> ContarUsuariosAtivosAsync()
        {
            return await _context.Users.CountAsync(u => u.Ativo);
        }

        public async Task<int> ContarAdministradoresAsync()
        {
            return await _context.Users.CountAsync(u => u.PermissionAccount == "admin");
        }

        public async Task<IEnumerable<UserModel>> ObterUsuariosRecentesAsync(int quantidade = 5)
        {
            return await _context.Users
                .OrderByDescending(u => u.DataCriacao)
                .Take(quantidade)
                .ToListAsync();
        }
    }
}