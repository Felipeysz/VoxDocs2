// Repository/Concrete/DocumentoRepository.cs
using VoxDocs.Data;
using VoxDocs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace VoxDocs.Repository
{
    public class DocumentoRepository : IDocumentoRepository
    {
        private readonly VoxDocsContext _context;

        public DocumentoRepository(VoxDocsContext context)
        {
            _context = context;
        }

        public async Task<DocumentoModel> GetByIdAsync(int id)
        {
            return await _context.Documentos.FindAsync(id);
        }

        public async Task<IEnumerable<DocumentoModel>> GetAllAsync()
        {
            return await _context.Documentos.ToListAsync();
        }

        public async Task<IEnumerable<DocumentoModel>> GetBySubPastaAsync(string subPasta)
        {
            return await _context.Documentos
                .Where(d => d.NomeSubPasta == subPasta)
                .ToListAsync();
        }

        public async Task<IEnumerable<DocumentoModel>> GetByPastaPrincipalAsync(string pastaPrincipal)
        {
            return await _context.Documentos
                .Where(d => d.NomePastaPrincipal == pastaPrincipal)
                .ToListAsync();
        }

        public async Task AddAsync(DocumentoModel documento)
        {
            await _context.Documentos.AddAsync(documento);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DocumentoModel documento)
        {
            _context.Entry(documento).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(DocumentoModel documento)
        {
            _context.Documentos.Remove(documento);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ArquivoExisteAsync(string nomeArquivo)
        {
            return await _context.Documentos
                .AnyAsync(d => d.NomeArquivo == nomeArquivo);
        }

        public async Task IncrementarAcessoAsync(int id)
        {
            var documento = await _context.Documentos.FindAsync(id);
            if (documento != null)
            {
                documento.ContadorAcessos++;
                await _context.SaveChangesAsync();
            }
        }
    }
}