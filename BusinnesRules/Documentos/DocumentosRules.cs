using VoxDocs.Models;
using VoxDocs.DTO;
using VoxDocs.Services;
using System.Threading.Tasks;
using System;

namespace VoxDocs.BusinessRules
{
    public class DocumentoBusinessRules
    {
        private readonly IDocumentoService _documentoService;

        public DocumentoBusinessRules(IDocumentoService documentoService)
        {
            _documentoService = documentoService;
        }

        public async Task ValidateDocumentoUploadAsync(DocumentoDto dto)
        {
            if (dto.Arquivo == null)
                throw new ArgumentException("Arquivo é obrigatório");

            if (string.IsNullOrWhiteSpace(dto.Empresa))
                throw new ArgumentException("Empresa é obrigatória");

            if (string.IsNullOrWhiteSpace(dto.NomePastaPrincipal))
                throw new ArgumentException("Nome da pasta principal é obrigatório");

            if (string.IsNullOrWhiteSpace(dto.NivelSeguranca))
                throw new ArgumentException("Nível de segurança é obrigatório");

            if (!IsValidSecurityLevel(dto.NivelSeguranca))
                throw new ArgumentException("Nível de segurança inválido");

            if (dto.Arquivo.Length > 100 * 1024 * 1024) // 100MB limit
                throw new ArgumentException("Tamanho máximo do arquivo excedido (100MB)");

            if (dto.NivelSeguranca != "Publico" && string.IsNullOrEmpty(dto.TokenSeguranca))
                throw new ArgumentException("Token de segurança é obrigatório para documentos restritos ou confidenciais");
        }

        public async Task ValidateDocumentoUpdateAsync(DocumentoModel documento)
        {
            if (documento == null)
                throw new ArgumentException("Documento inválido");

            if (string.IsNullOrWhiteSpace(documento.NomeArquivo))
                throw new ArgumentException("Nome do arquivo é obrigatório");

            if (string.IsNullOrWhiteSpace(documento.UrlArquivo))
                throw new ArgumentException("URL do arquivo é obrigatória");

            if (!IsValidSecurityLevel(documento.NivelSeguranca))
                throw new ArgumentException("Nível de segurança inválido");
        }

        private bool IsValidSecurityLevel(string nivelSeguranca)
        {
            var validLevels = new[] { "Publico", "Restrito", "Confidencial" };
            return validLevels.Contains(nivelSeguranca);
        }

        public async Task CheckDocumentoExistsAsync(int id)
        {
            var documento = await _documentoService.GetByIdAsync(id);
            if (documento == null)
                throw new ArgumentException("Documento não encontrado");
        }

        public async Task ValidateDocumentoDeleteAsync(int id)
        {
            var documento = await _documentoService.GetByIdAsync(id);
            if (documento == null)
                throw new ArgumentException("Documento não encontrado");

            if (documento.NivelSeguranca == "Confidencial")
                throw new ArgumentException("Documentos com nível de segurança 'Confidencial' não podem ser deletados");
        }

        public async Task ValidateDocumentoAccessAsync(int id, string usuario, string? token)
        {
            var documento = await _documentoService.GetByIdAsync(id);
            if (documento == null)
                throw new ArgumentException("Documento não encontrado");

            if (documento.NivelSeguranca == "Confidencial" && !usuario.EndsWith("@admin.com"))
                throw new UnauthorizedAccessException("Acesso negado a documentos com nível de segurança 'Confidencial'");

            if (documento.NivelSeguranca != "Publico" && documento.TokenSeguranca != token)
                throw new UnauthorizedAccessException("Token de segurança inválido");
        }
    }
}