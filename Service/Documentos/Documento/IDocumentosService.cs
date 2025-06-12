using System.Collections.Generic;
using System.Threading.Tasks;
using VoxDocs.DTO;

namespace VoxDocs.Services
{
    public interface IDocumentoService
    {
        Task<DocumentoDto> GetByIdAsync(Guid id, string? token = null);
        Task<IEnumerable<DocumentoDto>> GetAllAsync();
        Task<IEnumerable<DocumentoDto>> GetBySubPastaAsync(string subPasta);
        Task<IEnumerable<DocumentoDto>> GetByPastaPrincipalAsync(string pastaPrincipal);
        Task<DocumentoDto> CreateAsync(DocumentoCriacaoDto dto);
        Task DeleteAsync(Guid id, string? token);
        Task<DocumentoDto> UpdateAsync(DocumentoAtualizacaoDto dto);
        Task<bool> ValidateTokenDocumentoAsync(string nomeArquivo, string token);
        Task<(Stream stream, string contentType)> DownloadDocumentoProtegidoAsync(string nomeArquivo, string token);
        Task IncrementarAcessoAsync(Guid id);
    }
}