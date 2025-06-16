using VoxDocs.DTO;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoxDocs.Services
{
    public interface IDocumentosPastasService
    {
        // Métodos de Documento
        Task<bool> ValidateTokenDocumentoAsync(string nomeArquivo, string token);
        Task<DocumentoDto> GetDocumentoByIdAsync(Guid id, string? token = null);
        Task<IEnumerable<DocumentoDto>> GetAllDocumentosAsync();
        Task<IEnumerable<DocumentoDto>> GetDocumentosBySubPastaAsync(string subPasta);
        Task<IEnumerable<DocumentoDto>> GetDocumentosByPastaPrincipalAsync(string pastaPrincipal);
        Task<DocumentoDto> CreateDocumentoAsync(DocumentoCriacaoDto dto);
        Task DeleteDocumentoAsync(Guid id, string? token);
        Task<DocumentoDto> UpdateDocumentoAsync(DocumentoAtualizacaoDto dto);
        Task<DocumentoEstatisticasDto> GetEstatisticasEmpresaAsync(string empresa);
        Task IncrementarAcessoDocumentoAsync(Guid id);
        Task<bool> ArquivoExisteAsync(string nomeArquivo);
        Task<(Stream stream, string contentType)> DownloadDocumentoProtegidoAsync(string nomeArquivo, string token = null);

        // Métodos de Pasta Principal
        Task<IEnumerable<DTOPastaPrincipal>> GetAllPastasPrincipaisAsync();
        Task<DTOPastaPrincipal> GetPastaPrincipalByNameAsync(string nomePasta);
        Task<IEnumerable<DTOPastaPrincipal>> GetPastasPrincipaisByEmpresaAsync(string empresaContratante);
        Task<DTOPastaPrincipal?> GetPastaPrincipalByIdAsync(Guid id);
        Task<DTOPastaPrincipal> CreatePastaPrincipalAsync(DTOPastaPrincipalCreate dto);
        Task<bool> DeletePastaPrincipalAsync(Guid id);

        // Métodos de SubPasta
        Task<IEnumerable<DTOSubPasta>> GetAllSubPastasAsync();
        Task<IEnumerable<DTOSubPasta>> GetSubPastasByEmpresaAsync(string empresa);
        Task<DTOSubPasta?> GetSubPastaByNameAsync(string nomeSubPasta);
        Task<DTOSubPasta?> GetSubPastaByIdAsync(Guid id);
        Task<DTOSubPasta> CreateSubPastaAsync(DTOSubPastaCreate dto);
        Task<bool> DeleteSubPastaAsync(Guid id);
        Task<IEnumerable<DTOSubPasta>> GetSubPastasByPastaPrincipalAsync(string nomePastaPrincipal);
    }
}