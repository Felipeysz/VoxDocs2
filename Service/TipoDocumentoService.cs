using VoxDocs.Models;
using VoxDocs.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoxDocs.Services
{
    public class TipoDocumentoService : ITipoDocumentoService
    {
        private readonly VoxDocsContext _context;

        public TipoDocumentoService(VoxDocsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoDocumentoModel>> GetAllAsync() =>
            await _context.TiposDocumento.ToListAsync();

        public async Task<TipoDocumentoModel?> GetByIdAsync(int id) =>
            await _context.TiposDocumento.FindAsync(id);

        public async Task<TipoDocumentoModel> CreateAsync(TipoDocumentoModel tipo)
        {
            _context.TiposDocumento.Add(tipo);
            await _context.SaveChangesAsync();
            return tipo;
        }

        public async Task<TipoDocumentoModel> UpdateAsync(TipoDocumentoModel tipo)
        {
            _context.TiposDocumento.Update(tipo);
            await _context.SaveChangesAsync();
            return tipo;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tipo = await _context.TiposDocumento.FindAsync(id);
            if (tipo == null) return false;
            _context.TiposDocumento.Remove(tipo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}