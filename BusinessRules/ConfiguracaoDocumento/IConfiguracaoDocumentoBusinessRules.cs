// IConfiguracaoDocumentoBusinessRules.cs
using VoxDocs.Models;
using VoxDocs.BusinessRules;

namespace VoxDocs.BusinessRules
{
    public interface IConfiguracaoDocumentoBusinessRules
    {
        Task<BusinessResult<ConfiguracaoDocumentosModel>> ValidateAndGetConfiguracaoAsync();
        Task<BusinessResult<ConfiguracaoDocumentosModel>> ValidateAndUpdateConfiguracaoAsync(ConfiguracaoDocumentosModel config);
        Task<bool> ValidateFileType(string fileName);
        Task<bool> ValidateFileSize(long fileSize);
    }
}