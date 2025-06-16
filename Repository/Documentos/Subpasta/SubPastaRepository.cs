using Microsoft.EntityFrameworkCore;
using VoxDocs.Data;
using VoxDocs.Models;

namespace VoxDocs.Data.Repositories
{
    public class SubPastaRepository : ISubPastaRepository
    {
        private readonly VoxDocsContext _context;

        public SubPastaRepository(VoxDocsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubPastaModel>> GetAllAsync()
        {
            return await _context.SubPastas
                .Include(sp => sp.Documentos)
                .ToListAsync();
        }

        public async Task<IEnumerable<SubPastaModel>> GetByEmpresaAsync(string empresa)
        {
            return await _context.SubPastas
                .Where(s => s.EmpresaContratante == empresa)
                .ToListAsync();
        }

        public async Task<SubPastaModel?> GetByNameSubPastaAsync(string nomeSubPasta)
        {
            return await _context.SubPastas
                .FirstOrDefaultAsync(s => s.NomeSubPasta == nomeSubPasta);
        }

        public async Task<SubPastaModel?> GetByIdAsync(Guid id)
        {
            return await _context.SubPastas.FindAsync(id);
        }

        public async Task<SubPastaModel> CreateAsync(SubPastaModel subPasta)
        {
            subPasta.Id = Guid.NewGuid();
            _context.SubPastas.Add(subPasta);
            await _context.SaveChangesAsync();
            return subPasta;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var subPasta = await _context.SubPastas.FindAsync(id);
            if (subPasta == null) return false;

            _context.SubPastas.Remove(subPasta);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SubPastaModel>> GetSubChildrenAsync(string nomePastaPrincipal)
        {
            return await _context.SubPastas
                .Where(sp => sp.NomePastaPrincipal.Trim().ToLower() == nomePastaPrincipal.Trim().ToLower())
                .ToListAsync();
        }

        public async Task<SubPastaModel?> GetByNameAndEmpresaAsync(string nomeSubPasta, string empresaContratante)
        {
            return await _context.SubPastas
                .FirstOrDefaultAsync(s => s.NomeSubPasta == nomeSubPasta && 
                                        s.EmpresaContratante == empresaContratante);
        }
    }
}