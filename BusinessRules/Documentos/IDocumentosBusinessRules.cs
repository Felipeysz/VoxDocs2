// Interfaces
using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.BusinessRules
{
    public interface IPastaPrincipalBusinessRules
    {
        Task<BusinessResult<PastaPrincipalModel>> ValidateAndCreateAsync(PastaPrincipalModel model);
        Task<BusinessResult<bool>> ValidateAndDeleteAsync(Guid id);
        Task<BusinessResult<PastaPrincipalModel>> ValidateGetByIdAsync(Guid id);
        Task<BusinessResult<PastaPrincipalModel>> ValidateGetByNameAsync(string nomePasta);
        Task<BusinessResult<IEnumerable<PastaPrincipalModel>>> ValidateGetByEmpresaAsync(string empresaContratante);
    }

    public interface IDocumentoBusinessRules
    {
        // Mensagens de erro públicas
        const string DocumentoNaoEncontradoMsg = "Documento não encontrado.";
        const string TokenObrigatorioMsg = "Token de segurança é obrigatório para este documento.";
        const string TokenInvalidoMsg = "Token de segurança inválido.";
        const string ArquivoNaoEncontradoMsg = "Arquivo não encontrado no storage.";

        // Métodos principais
        Task<bool> ValidateTokenDocumentoAsync(string nomeArquivo, string token);
        Task<DocumentoModel> GetByIdAsync(Guid id, string? token = null);
        Task DeleteAsync(Guid id, string? token);
        Task<IEnumerable<DocumentoModel>> GetAllAsync();
        Task<IEnumerable<DocumentoModel>> GetBySubPastaAsync(string subPasta);
        Task<IEnumerable<DocumentoModel>> GetByPastaPrincipalAsync(string pastaPrincipal);
        Task<DocumentoModel> CreateAsync(DocumentoModel documento, Stream arquivoStream, string usuario);
        Task<DocumentoModel> UpdateAsync(DocumentoModel documento, Stream novoArquivoStream = null);
        Task<DocumentoEstatisticas> GetEstatisticasEmpresaAsync(string empresa);
        Task IncrementarAcessoAsync(Guid id);
        Task<bool> ArquivoExisteAsync(string nomeArquivo);
        Task<(Stream stream, string contentType)> DownloadDocumentoProtegidoAsync(string nomeArquivo, string token = null);

        // Métodos auxiliares
        string GenerateTokenHash(string token);
        DateTime ConvertToBrasiliaTime(DateTime utcTime);
        void ValidateDocumentCreation(DocumentoModel documento, Stream arquivoStream);
        string GetErrorMessage(string operation, Exception ex);

        // Métodos de validação
        void ValidateTokenSecurity(string token, string? tokenSeguranca);
    }

    public interface ISubPastaBusinessRules
    {
        Task<SubPastaModel> ValidateAndCreateSubPastaAsync(SubPastaModel subPasta);
        Task<bool> CanDeleteSubPastaAsync(Guid id);
        Task<bool> IsSubPastaNameUniqueAsync(string nomeSubPasta, string empresaContratante);
        Task<bool> DoesPastaPrincipalExistAsync(string nomePastaPrincipal, string empresaContratante);
        Task ValidateSubPastaAsync(SubPastaModel subPasta);
    }
}