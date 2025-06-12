using VoxDocs.DTO;
using System.IO;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public interface IDocumentoBusinessRules
    {
        // Métodos existentes
        Task<bool> ValidateTokenDocumentoAsync(string nomeArquivo, string token);
        Task<DocumentoDto> GetByIdAsync(Guid id, string? token = null);
        Task DeleteAsync(Guid id, string? token);
        Task<IEnumerable<DocumentoDto>> GetAllAsync();
        Task<IEnumerable<DocumentoDto>> GetBySubPastaAsync(string subPasta);
        Task<IEnumerable<DocumentoDto>> GetByPastaPrincipalAsync(string pastaPrincipal);
        Task<DocumentoDto> CreateAsync(DocumentoCriacaoDto dto);
        Task<DocumentoDto> UpdateAsync(DocumentoAtualizacaoDto dto);
        Task<DocumentoEstatisticasDto> GetEstatisticasEmpresaAsync(string empresa);
        Task IncrementarAcessoAsync(Guid id);
        Task<bool> ArquivoExisteAsync(string nomeArquivo);
        Task<(Stream stream, string contentType)> DownloadDocumentoProtegidoAsync(string nomeArquivo, string token = null);

        // Métodos adicionais
        void ValidateDocumentCreation(DocumentoCriacaoDto dto);
        void ValidateDocumentUpdate(DocumentoAtualizacaoDto dto, DocumentoModel doc);
        string GenerateTokenHash(string token);
        DateTime ConvertToBrasiliaTime(DateTime utcTime);
        string GetErrorMessage(string operation, Exception ex);
        void ValidateTokenSecurity(string token, string? tokenSeguranca);
        DocumentoDto MapToResponseDto(DocumentoModel doc);
    }
}