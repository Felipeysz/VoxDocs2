// PastaPrincipalRepository.cs
using VoxDocs.Models;
using Microsoft.EntityFrameworkCore;
using VoxDocs.Data; // If using Entity Framework

namespace VoxDocs.Services
{
    public class PastaPrincipalRepository : IPastaPrincipalRepository
    {
        private readonly VoxDocsContext _context; // Replace with your actual DbContext

        public PastaPrincipalRepository(VoxDocsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PastaPrincipalModel>> GetAllAsync()
        {
            return await _context.PastaPrincipal
                .Include(p => p.SubPastas) // If you need to include subfolders
                .ToListAsync();
        }

        public async Task<PastaPrincipalModel?> GetByNamePrincipalAsync(string nomePasta)
        {
            return await _context.PastaPrincipal
                .FirstOrDefaultAsync(p => p.NomePastaPrincipal == nomePasta);
        }

        public async Task<IEnumerable<PastaPrincipalModel>> GetByEmpresaAsync(string empresaContratante)
        {
            return await _context.PastaPrincipal
                .Where(p => p.EmpresaContratante == empresaContratante)
                .Include(p => p.SubPastas) // If you need to include subfolders
                .ToListAsync();
        }

        public async Task<PastaPrincipalModel?> GetByIdAsync(Guid id)
        {
            return await _context.PastaPrincipal
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PastaPrincipalModel> CreateAsync(PastaPrincipalModel pasta)
        {
            _context.PastaPrincipal.Add(pasta);
            await _context.SaveChangesAsync();
            return pasta;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var pasta = await _context.PastaPrincipal.FindAsync(id);
            if (pasta == null) return false;

            _context.PastaPrincipal.Remove(pasta);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<PastaPrincipalModel?> GetByNameAndEmpresaAsync(string nomePasta, string empresaContratante)
        {
            return await _context.PastaPrincipal
                .FirstOrDefaultAsync(p => p.NomePastaPrincipal == nomePasta && 
                                    p.EmpresaContratante == empresaContratante);
        }
    }
}