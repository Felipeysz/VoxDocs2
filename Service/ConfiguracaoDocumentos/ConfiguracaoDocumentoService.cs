// ConfiguracaoDocumentoService.cs
using VoxDocs.DTO;
using VoxDocs.Models;
using VoxDocs.BusinessRules;

namespace VoxDocs.Services
{
    public class ConfiguracaoDocumentoService : IConfiguracaoDocumentoService
    {
        private readonly IConfiguracaoDocumentoBusinessRules _businessRules;

        public ConfiguracaoDocumentoService(IConfiguracaoDocumentoBusinessRules businessRules)
        {
            _businessRules = businessRules;
        }

        public async Task<BusinessResult<DTOConfiguracaoDocumentos>> GetConfiguracoesAsync()
        {
            var result = await _businessRules.ValidateAndGetConfiguracaoAsync();
            
            if (!result.Success)
                return new BusinessResult<DTOConfiguracaoDocumentos>(
                    null,
                    result.Success,
                    result.ErrorMessage,
                    result.StatusCode);

            return new BusinessResult<DTOConfiguracaoDocumentos>(MapToDTO(result.Data));
        }

        public async Task<BusinessResult<DTOConfiguracaoDocumentos>> SalvarConfiguracoesAsync(DTOConfiguracaoDocumentos dto)
        {
            var model = MapToModel(dto);
            
            var result = await _businessRules.ValidateAndUpdateConfiguracaoAsync(model);
            
            if (!result.Success)
                return new BusinessResult<DTOConfiguracaoDocumentos>(
                    null,
                    result.Success,
                    result.ErrorMessage,
                    result.StatusCode);

            return new BusinessResult<DTOConfiguracaoDocumentos>(MapToDTO(result.Data));
        }

        public async Task<bool> ValidarTipoArquivoAsync(string fileName)
        {
            return await _businessRules.ValidateFileType(fileName);
        }

        public async Task<bool> ValidarTamanhoArquivoAsync(long fileSize)
        {
            return await _businessRules.ValidateFileSize(fileSize);
        }

        private DTOConfiguracaoDocumentos MapToDTO(ConfiguracaoDocumentosModel model)
        {
            if (model == null) return null;
            
            return new DTOConfiguracaoDocumentos
            {
                PermitirPDF = model.PermitirPDF,
                PermitirWord = model.PermitirWord,
                PermitirExcel = model.PermitirExcel,
                PermitirImagens = model.PermitirImagens,
                TamanhoMaximoMB = model.TamanhoMaximoMB,
                DiasArmazenamentoTemporario = model.DiasArmazenamentoTemporario
            };
        }

        private ConfiguracaoDocumentosModel MapToModel(DTOConfiguracaoDocumentos dto)
        {
            if (dto == null) return null;
            
            return new ConfiguracaoDocumentosModel
            {
                PermitirPDF = dto.PermitirPDF,
                PermitirWord = dto.PermitirWord,
                PermitirExcel = dto.PermitirExcel,
                PermitirImagens = dto.PermitirImagens,
                TamanhoMaximoMB = dto.TamanhoMaximoMB,
                DiasArmazenamentoTemporario = dto.DiasArmazenamentoTemporario
            };
        }
    }
}