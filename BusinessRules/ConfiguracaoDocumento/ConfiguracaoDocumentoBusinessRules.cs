// ConfiguracaoDocumentoBusinessRules.cs
using VoxDocs.Models;
using VoxDocs.Data.Repositories;
using Microsoft.Extensions.Logging;
using System.IO;

namespace VoxDocs.BusinessRules
{
    public class ConfiguracaoDocumentoBusinessRules : IConfiguracaoDocumentoBusinessRules
    {
        private readonly IConfiguracaoDocumentoRepository _repository;
        private readonly ILogger<ConfiguracaoDocumentoBusinessRules> _logger;

        public ConfiguracaoDocumentoBusinessRules(
            IConfiguracaoDocumentoRepository repository,
            ILogger<ConfiguracaoDocumentoBusinessRules> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<BusinessResult<ConfiguracaoDocumentosModel>> ValidateAndGetConfiguracaoAsync()
        {
            try
            {
                var config = await _repository.GetFirstAsync();
                return new BusinessResult<ConfiguracaoDocumentosModel>(config);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter configuração de documentos");
                return new BusinessResult<ConfiguracaoDocumentosModel>(
                    null,
                    false,
                    "Ocorreu um erro ao obter as configurações de documentos.",
                    500);
            }
        }

        public async Task<BusinessResult<ConfiguracaoDocumentosModel>> ValidateAndUpdateConfiguracaoAsync(ConfiguracaoDocumentosModel config)
        {
            try
            {
                // Validações básicas
                if (config == null)
                {
                    return new BusinessResult<ConfiguracaoDocumentosModel>(
                        null,
                        false,
                        "Configuração não pode ser nula.",
                        400);
                }

                if (config.TamanhoMaximoMB <= 0 || config.TamanhoMaximoMB > 100)
                {
                    return new BusinessResult<ConfiguracaoDocumentosModel>(
                        null,
                        false,
                        "Tamanho máximo deve estar entre 1MB e 100MB.",
                        400);
                }

                if (config.DiasArmazenamentoTemporario <= 0 || config.DiasArmazenamentoTemporario > 365)
                {
                    return new BusinessResult<ConfiguracaoDocumentosModel>(
                        null,
                        false,
                        "Dias de armazenamento temporário deve estar entre 1 e 365 dias.",
                        400);
                }

                // Atualiza a configuração
                await _repository.UpdateAsync(config);
                return new BusinessResult<ConfiguracaoDocumentosModel>(config);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar configuração de documentos");
                return new BusinessResult<ConfiguracaoDocumentosModel>(
                    null,
                    false,
                    "Ocorreu um erro ao atualizar as configurações de documentos.",
                    500);
            }
        }

        public async Task<bool> ValidateFileType(string fileName)
        {
            try
            {
                var config = await _repository.GetFirstAsync();
                var extension = Path.GetExtension(fileName)?.ToLower();

                return extension switch
                {
                    ".pdf" => config.PermitirPDF,
                    ".doc" or ".docx" => config.PermitirWord,
                    ".xls" or ".xlsx" => config.PermitirExcel,
                    ".jpg" or ".jpeg" or ".png" or ".gif" => config.PermitirImagens,
                    _ => false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao validar tipo de arquivo: {fileName}");
                return false;
            }
        }

        public async Task<bool> ValidateFileSize(long fileSize)
        {
            try
            {
                var config = await _repository.GetFirstAsync();
                var maxSizeBytes = config.TamanhoMaximoMB * 1024 * 1024;
                return fileSize <= maxSizeBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao validar tamanho do arquivo: {fileSize} bytes");
                return false;
            }
        }
    }
}