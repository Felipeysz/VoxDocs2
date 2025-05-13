using VoxDocs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoxDocs.Services
{
    public interface IDocumentoService
    {
        Task<IEnumerable<DocumentoModel>> GetAllAsync();
        Task<DocumentoModel?> GetByIdAsync(int id);
        Task<DocumentoModel> CreateAsync(DocumentoModel doc);
        Task<DocumentoModel> UpdateAsync(DocumentoModel doc);
        Task<bool> DeleteAsync(int id);
    }
}