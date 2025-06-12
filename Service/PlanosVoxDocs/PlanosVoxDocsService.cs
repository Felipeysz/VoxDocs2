// Services/PlanosVoxDocsService.cs
using Microsoft.EntityFrameworkCore;
using VoxDocs.Data;
using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public class PlanosVoxDocsService : IPlanosVoxDocsService
    {
        private readonly VoxDocsContext _context;


        public async Task<PlanosVoxDocsModel> GetPlanByNameAndPeriodicidadeAsync(string nome, string periodicidade)
        {
            return await _context.PlanosVoxDocs
                .FirstOrDefaultAsync(p => 
                    p.Nome.Trim().ToLower() == nome.Trim().ToLower() &&
                    p.Periodicidade.Trim().ToLower() == periodicidade.Trim().ToLower());
        }
        public PlanosVoxDocsService(VoxDocsContext context)
            => _context = context;
        public async Task<List<PlanosVoxDocsModel>> GetAllPlansAsync()
            => await _context.PlanosVoxDocs.ToListAsync();

        public async Task<List<PlanosVoxDocsModel>> GetPlansByCategoryAsync(string categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria))
                return new List<PlanosVoxDocsModel>();

            // Utiliza EF.Functions.Like para melhor tradução pelo EF Core
            var pattern = $"%{categoria.Trim()}%";
            return await _context.PlanosVoxDocs
                .Where(p => EF.Functions.Like(p.Nome, pattern))
                .ToListAsync();
        }

        public async Task<PlanosVoxDocsModel> GetPlanByIdAsync(int id)
            => await _context.PlanosVoxDocs.FindAsync(id)
               ?? throw new KeyNotFoundException("Plano não encontrado.");

        public async Task<PlanosVoxDocsModel> CreatePlanAsync(DTOPlanosVoxDocs dto)
        {
            if (dto.Nome.ToLower() != "gratuito" && dto.Nome.ToLower() != "premium")
                throw new InvalidOperationException("Somente os planos 'Gratuito' e 'Premium' são permitidos.");

            // Lógica de desconto se for Premium
            decimal finalPrice = dto.Preco;
            if (dto.Nome.ToLower() == "premium")
            {
                if (dto.Duracao == 6)
                    finalPrice = Math.Round(dto.Preco * 6 * 0.9m, 2);
                else if (dto.Duracao == 12)
                    finalPrice = Math.Round(dto.Preco * 12 * 0.8m, 2);
                else if (dto.Duracao == 1)
                    finalPrice = dto.Preco;
            }

            var plan = new PlanosVoxDocsModel
            {
                Id = Guid.NewGuid(),
                Nome = dto.Nome,
                Descriçao = dto.Descricao,
                Preco = finalPrice,
                Duracao = dto.Duracao,
                Periodicidade = dto.Periodicidade,
                ArmazenamentoDisponivel = dto.ArmazenamentoDisponivel,
                LimiteAdmin = dto.LimiteAdmin ?? 0,
                LimiteUsuario = dto.LimiteUsuario ?? 0
            };

            _context.PlanosVoxDocs.Add(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<PlanosVoxDocsModel> UpdatePlanAsync(int id, DTOPlanosVoxDocs dto)
        {
            var plan = await _context.PlanosVoxDocs.FindAsync(id)
                ?? throw new KeyNotFoundException("Plano não encontrado.");

            plan.Nome = dto.Nome;
            plan.Descriçao = dto.Descricao;
            plan.Preco = dto.Preco;
            plan.Duracao = dto.Duracao;
            plan.Periodicidade = dto.Periodicidade;
            plan.ArmazenamentoDisponivel = dto.ArmazenamentoDisponivel;
            plan.LimiteAdmin = dto.LimiteAdmin ?? 0;
            plan.LimiteUsuario = dto.LimiteUsuario ?? 0;

            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task DeletePlanAsync(int id)
        {
            var plan = await _context.PlanosVoxDocs.FindAsync(id)
                ?? throw new KeyNotFoundException("Plano não encontrado.");

            _context.PlanosVoxDocs.Remove(plan);
            await _context.SaveChangesAsync();
        }
        
        public async Task<PlanosVoxDocsModel> GetPlanByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            return await _context.PlanosVoxDocs
                .FirstOrDefaultAsync(p => p.Nome == name);
        }
    }
}
