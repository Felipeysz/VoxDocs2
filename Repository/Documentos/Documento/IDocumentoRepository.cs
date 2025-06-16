// Repository/Interfaces/IDocumentoRepository.cs
using VoxDocs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoxDocs.Repository
{
    public interface IDocumentoRepository
    {
        Task<DocumentoModel> GetByIdAsync(Guid id);
        Task<IEnumerable<DocumentoModel>> GetAllAsync();
        Task<IEnumerable<DocumentoModel>> GetBySubPastaAsync(string subPasta);
        Task<IEnumerable<DocumentoModel>> GetByPastaPrincipalAsync(string pastaPrincipal);
        Task AddAsync(DocumentoModel documento);
        Task UpdateAsync(DocumentoModel documento);
        Task DeleteAsync(DocumentoModel documento);
        Task<bool> ArquivoExisteAsync(string nomeArquivo);
        Task IncrementarAcessoAsync(Guid id);
    }
}