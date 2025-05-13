using System.Threading.Tasks;
using VoxDocs.Models;
using System.Collections.Generic;

namespace VoxDocs.Services
{
    public interface IDocumentoUploadService
    {
        Task<DocumentoUploadModel> GetByNomeArquivoAsync(string nomeArquivo);
        Task<DocumentoUploadModel> CreateAsync(DocumentoUploadModel doc);
        Task<DocumentoUploadModel> UpdateAsync(DocumentoUploadModel doc);
        Task<IEnumerable<DocumentoUploadModel>> GetAllAsync();
        Task<DocumentoUploadModel> GetByIdAsync(int id); // <-- ADICIONE ESTA LINHA
    }
}