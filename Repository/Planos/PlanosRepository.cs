using Microsoft.EntityFrameworkCore;
using VoxDocs.Data;
using VoxDocs.Interfaces;
using VoxDocs.Models;

namespace VoxDocs.Repositories
{
    public class PlanosVoxDocsRepository : IPlanosVoxDocsRepository
    {
        private readonly VoxDocsContext _context;

        public PlanosVoxDocsRepository(VoxDocsContext context)
        {
            _context = context;
        }

        public async Task<PlanosVoxDocsModel> GetPlanByNameAndPeriodicidadeAsync(string nome, string periodicidade)
        {
            return await _context.PlanosVoxDocs
                .FirstOrDefaultAsync(p => 
                    p.Nome.Trim().ToLower() == nome.Trim().ToLower() &&
                    p.Periodicidade.Trim().ToLower() == periodicidade.Trim().ToLower());
        }

        public async Task<List<PlanosVoxDocsModel>> GetAllPlansAsync()
        {
            return await _context.PlanosVoxDocs.ToListAsync();
        }

        public async Task<List<PlanosVoxDocsModel>> GetPlansByCategoryAsync(string categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria))
                return new List<PlanosVoxDocsModel>();

            var pattern = $"%{categoria.Trim()}%";
            return await _context.PlanosVoxDocs
                .Where(p => EF.Functions.Like(p.Nome, pattern))
                .ToListAsync();
        }

        public async Task<PlanosVoxDocsModel> GetPlanByIdAsync(int id)
        {
            return await _context.PlanosVoxDocs.FindAsync(id)
                   ?? throw new KeyNotFoundException("Plano não encontrado.");
        }

        public async Task<PlanosVoxDocsModel> CreatePlanAsync(PlanosVoxDocsModel plan)
        {
            _context.PlanosVoxDocs.Add(plan);
            await _context.SaveChangesAsync();
            return plan;
        }

        public async Task<PlanosVoxDocsModel> UpdatePlanAsync(PlanosVoxDocsModel plan)
        {
            _context.PlanosVoxDocs.Update(plan);
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