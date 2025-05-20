// Services/PlanosVoxDocsService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VoxDocs.Data;
using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public class PlanosVoxDocsService : IPlanosVoxDocsService
    {
        private readonly VoxDocsContext _context;

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
                .Where(p => EF.Functions.Like(p.Name, pattern))
                .ToListAsync();
        }

        public async Task<PlanosVoxDocsModel> GetPlanByIdAsync(int id)
            => await _context.PlanosVoxDocs.FindAsync(id)
               ?? throw new KeyNotFoundException("Plano não encontrado.");

        public async Task<PlanosVoxDocsModel> CreatePlanAsync(DTOPlanosVoxDocs dto)
        {
            var plan = new PlanosVoxDocsModel
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Duration = dto.Duration,
                Periodicidade = dto.Periodicidade,
                ArmazenamentoDisponivel = dto.ArmazenamentoDisponivel,
                TokensDisponiveis = dto.TokensDisponiveis ?? "Infinito",
                LimiteAdmin = dto.LimiteAdmin,
                LimiteUsuario = dto.LimiteUsuario
            };

            _context.PlanosVoxDocs.Add(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<PlanosVoxDocsModel> UpdatePlanAsync(int id, DTOPlanosVoxDocs dto)
        {
            var plan = await _context.PlanosVoxDocs.FindAsync(id)
                ?? throw new KeyNotFoundException("Plano não encontrado.");

            plan.Name = dto.Name;
            plan.Description = dto.Description;
            plan.Price = dto.Price;
            plan.Duration = dto.Duration;
            plan.Periodicidade = dto.Periodicidade;
            plan.ArmazenamentoDisponivel = dto.ArmazenamentoDisponivel;
            plan.LimiteAdmin = dto.LimiteAdmin;
            plan.LimiteUsuario = dto.LimiteUsuario;

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
        
        public async Task<PlanosVoxDocsModel?> GetPlanByNameAsync(string name)
    => await _context.PlanosVoxDocs
                     .AsNoTracking()
                     .FirstOrDefaultAsync(p => p.Name == name);
    }
}
