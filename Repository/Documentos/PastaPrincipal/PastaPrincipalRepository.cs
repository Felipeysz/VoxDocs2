using Microsoft.EntityFrameworkCore;
using VoxDocs.Data;
using VoxDocs.Models;

namespace VoxDocs.Services
{

    public class PastaPrincipalRepository : IPastaPrincipalRepository
    {
        private readonly VoxDocsContext _context;

        public PastaPrincipalRepository(VoxDocsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PastaPrincipalModel>> GetAllAsync()
        {
            return await _context.PastaPrincipal
                .Include(p => p.SubPastas)
                .ToListAsync();
        }

        public async Task<PastaPrincipalModel> GetByNamePrincipalAsync(string nomePasta)
        {
            return await _context.PastaPrincipal
                .FirstOrDefaultAsync(p => p.NomePastaPrincipal == nomePasta);
        }

        public async Task<IEnumerable<PastaPrincipalModel>> GetByEmpresaAsync(string empresaContratante)
        {
            return await _context.PastaPrincipal
                .Include(p => p.SubPastas)
                .Where(p => p.EmpresaContratante == empresaContratante)
                .ToListAsync();
        }

        public async Task<PastaPrincipalModel?> GetByIdAsync(int id)
        {
            return await _context.PastaPrincipal.FindAsync(id);
        }

        public async Task<PastaPrincipalModel> CreateAsync(PastaPrincipalModel pasta)
        {
            _context.PastaPrincipal.Add(pasta);
            await _context.SaveChangesAsync();
            return pasta;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pasta = await _context.PastaPrincipal.FindAsync(id);
            if (pasta == null) return false;

            _context.PastaPrincipal.Remove(pasta);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}