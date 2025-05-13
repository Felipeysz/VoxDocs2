using VoxDocs.Models;
using VoxDocs.Data;
using Microsoft.EntityFrameworkCore;

namespace VoxDocs.Services
{
    public class DocumentoService : IDocumentoService
    {
        private readonly VoxDocsContext _context;

        public DocumentoService(VoxDocsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DocumentoModel>> GetAllAsync() =>
            await _context.Documentos.ToListAsync();

        public async Task<DocumentoModel?> GetByIdAsync(int id) =>
            await _context.Documentos.FindAsync(id);

        public async Task<DocumentoModel> CreateAsync(DocumentoModel doc)
        {
            _context.Documentos.Add(doc);
            await _context.SaveChangesAsync();
            return doc;
        }

        public async Task<DocumentoModel> UpdateAsync(DocumentoModel doc)
        {
            _context.Documentos.Update(doc);
            await _context.SaveChangesAsync();
            return doc;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var doc = await _context.Documentos.FindAsync(id);
            if (doc == null) return false;
            _context.Documentos.Remove(doc);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}