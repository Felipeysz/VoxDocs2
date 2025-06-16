// IConfiguracaoDocumentoService.cs
using VoxDocs.DTO;
using VoxDocs.BusinessRules;

namespace VoxDocs.Services
{
    public interface IConfiguracaoDocumentoService
    {
        Task<BusinessResult<DTOConfiguracaoDocumentos>> GetConfiguracoesAsync();
        Task<BusinessResult<DTOConfiguracaoDocumentos>> SalvarConfiguracoesAsync(DTOConfiguracaoDocumentos dto);
        Task<bool> ValidarTipoArquivoAsync(string fileName);
        Task<bool> ValidarTamanhoArquivoAsync(long fileSize);
    }
}