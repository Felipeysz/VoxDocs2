using System.Threading.Tasks;
using VoxDocs.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VoxDocs.Data;

namespace VoxDocs.Services
{
    public class DocumentoUploadService : IDocumentoUploadService
    {
        private readonly VoxDocsContext _context;

        public DocumentoUploadService(VoxDocsContext context)
        {
            _context = context;
        }

        public async Task<DocumentoUploadModel> GetByNomeArquivoAsync(string nomeArquivo)
        {
            return await _context.DocumentosUploads
                .FirstOrDefaultAsync(d => d.NomeArquivo == nomeArquivo);
        }

        public async Task<DocumentoUploadModel> CreateAsync(DocumentoUploadModel doc)
        {
            _context.DocumentosUploads.Add(doc);
            await _context.SaveChangesAsync();
            return doc;
        }

        public async Task<DocumentoUploadModel> UpdateAsync(DocumentoUploadModel doc)
        {
            _context.DocumentosUploads.Update(doc);
            await _context.SaveChangesAsync();
            return doc;
        }

        public async Task<IEnumerable<DocumentoUploadModel>> GetAllAsync()
        {
            return await _context.DocumentosUploads.ToListAsync();
        }

        public async Task<DocumentoUploadModel> GetByIdAsync(int id)
        {
            return await _context.DocumentosUploads.FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}