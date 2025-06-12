using VoxDocs.Data;
using VoxDocs.Models;
using VoxDocs.DTO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using VoxDocs.Services;

namespace VoxDocs.BusinessRules
{
    public class DocumentoBusinessRules : IDocumentoBusinessRules
    {
        private readonly VoxDocsContext _context;

        // Mensagens de erro públicas
        public const string DocumentoNaoEncontradoMsg = "Documento não encontrado.";
        public const string TokenObrigatorioMsg = "Token de segurança é obrigatório para este documento.";
        public const string TokenInvalidoMsg = "Token de segurança inválido.";
        public const string ArquivoNaoEncontradoMsg = "Arquivo não encontrado no storage.";

        public DocumentoBusinessRules(VoxDocsContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidateTokenDocumentoAsync(string nomeArquivo, string token)
        {
            if (string.IsNullOrWhiteSpace(nomeArquivo))
                throw new ArgumentException("Nome do arquivo não pode ser vazio.");

            var doc = await _context.Documentos
                .FirstOrDefaultAsync(d => d.NomeArquivo.ToLower() == nomeArquivo.ToLower());

            if (doc == null)
                throw new ArgumentException(DocumentoNaoEncontradoMsg);

            if (doc.NivelSeguranca == "Publico")
                return true;

            ValidateTokenSecurity(token, doc.TokenSeguranca);

            return true;
        }

        public async Task<DocumentoDto> GetByIdAsync(Guid id, string? token = null)
        {
            var doc = await _context.Documentos.FindAsync(id);
            if (doc == null)
                throw new ArgumentException(DocumentoNaoEncontradoMsg);

            if (doc.NivelSeguranca != "Publico")
            {
                ValidateTokenSecurity(token, doc.TokenSeguranca);
            }

            return MapToResponseDto(doc);
        }

        public async Task DeleteAsync(Guid id, string? token)
        {
            var doc = await _context.Documentos.FindAsync(id);
            if (doc == null)
                throw new ArgumentException(DocumentoNaoEncontradoMsg);

            if (doc.NivelSeguranca != "Publico")
            {
                ValidateTokenSecurity(token, doc.TokenSeguranca);
            }

            _context.Documentos.Remove(doc);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DocumentoDto>> GetAllAsync()
        {
            var documentos = await _context.Documentos.ToListAsync();
            return documentos.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<DocumentoDto>> GetBySubPastaAsync(string subPasta)
        {
            if (string.IsNullOrWhiteSpace(subPasta))
                throw new ArgumentException("Nome da subpasta não pode ser vazio.");

            var documentos = await _context.Documentos
                .Where(d => d.NomeSubPasta == subPasta)
                .ToListAsync();

            return documentos.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<DocumentoDto>> GetByPastaPrincipalAsync(string pastaPrincipal)
        {
            if (string.IsNullOrWhiteSpace(pastaPrincipal))
                throw new ArgumentException("Nome da pasta principal não pode ser vazio.");

            var documentos = await _context.Documentos
                .Where(d => d.NomePastaPrincipal == pastaPrincipal)
                .ToListAsync();

            return documentos.Select(MapToResponseDto);
        }

        public async Task<DocumentoDto> CreateAsync(DocumentoCriacaoDto dto)
        {
            ValidateDocumentoDto(dto);

            if (await ArquivoExisteAsync(dto.Arquivo.FileName))
                throw new InvalidOperationException("Já existe um documento com este nome.");

            if (dto.NivelSeguranca != NivelSeguranca.Publico && string.IsNullOrWhiteSpace(dto.TokenSeguranca))
                throw new ArgumentException("Token de segurança é obrigatório para documentos não públicos.");

            var novoDocumento = new DocumentoModel
            {
                NomeArquivo = dto.Arquivo.FileName,
                UsuarioCriador = dto.Usuario,
                DataCriacao = DateTime.UtcNow,
                UsuarioUltimaAlteracao = dto.Usuario,
                DataUltimaAlteracao = DateTime.UtcNow,
                Empresa = dto.EmpresaContratante,
                NomePastaPrincipal = dto.NomePastaPrincipal,
                NomeSubPasta = dto.NomeSubPasta,
                NivelSeguranca = dto.NivelSeguranca.ToString(),
                TokenSeguranca = GenerateTokenHash(dto.TokenSeguranca),
                Descrição = dto.Descricao,
                TamanhoArquivo = dto.Arquivo.Length,
            };

            _context.Documentos.Add(novoDocumento);
            await _context.SaveChangesAsync();

            return MapToResponseDto(novoDocumento);
        }

        public async Task<DocumentoDto> UpdateAsync(DocumentoAtualizacaoDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var doc = await _context.Documentos.FindAsync(dto.Id);
            if (doc == null)
                throw new ArgumentException(DocumentoNaoEncontradoMsg);

            if (!string.IsNullOrWhiteSpace(dto.Descricao))
            {
                if (dto.Descricao.Length > 500)
                    throw new ArgumentException("Descrição não pode ter mais que 500 caracteres.");

                doc.Descrição = dto.Descricao;
            }

            if (dto.NovoArquivo != null)
            {
                doc.NomeArquivo = dto.NovoArquivo.FileName;
                doc.TamanhoArquivo = dto.NovoArquivo.Length;
            }

            if (dto.NivelSeguranca.HasValue)
            {
                doc.NivelSeguranca = dto.NivelSeguranca.Value.ToString();
            }

            if (!string.IsNullOrEmpty(dto.TokenSeguranca))
            {
                doc.TokenSeguranca = GenerateTokenHash(dto.TokenSeguranca);
            }

            doc.UsuarioUltimaAlteracao = dto.UsuarioUltimaAlteracao;
            doc.DataUltimaAlteracao = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponseDto(doc);
        }

        public async Task<DocumentoEstatisticasDto> GetEstatisticasEmpresaAsync(string empresa)
        {
            if (string.IsNullOrWhiteSpace(empresa))
                throw new ArgumentException("Nome da empresa não pode ser vazio.");

            var documentos = await _context.Documentos
                .Where(d => d.Empresa == empresa)
                .ToListAsync();

            return new DocumentoEstatisticasDto
            {
                EmpresaContratante = empresa,
                QuantidadeDocumentos = documentos.Count,
                TamanhoTotalGb = documentos.Sum(d => d.TamanhoArquivo) / 1024.0 / 1024.0 / 1024.0
            };
        }

        public async Task IncrementarAcessoAsync(Guid id)
        {
            var doc = await _context.Documentos.FindAsync(id);
            if (doc == null)
                throw new ArgumentException(DocumentoNaoEncontradoMsg);

            doc.ContadorAcessos++;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ArquivoExisteAsync(string nomeArquivo)
        {
            if (string.IsNullOrWhiteSpace(nomeArquivo))
                throw new ArgumentException("Nome do arquivo não pode ser vazio.");

            return await _context.Documentos.AnyAsync(d => d.NomeArquivo == nomeArquivo);
        }

        public async Task<(Stream stream, string contentType)> DownloadDocumentoProtegidoAsync(string nomeArquivo, string token = null)
        {
            if (string.IsNullOrWhiteSpace(nomeArquivo))
                throw new ArgumentException("Nome do arquivo não pode ser vazio.");

            var doc = await _context.Documentos.FirstOrDefaultAsync(d => d.NomeArquivo == nomeArquivo);
            if (doc == null)
                throw new ArgumentException(DocumentoNaoEncontradoMsg);

            if (doc.NivelSeguranca != "Publico")
            {
                ValidateTokenSecurity(token, doc.TokenSeguranca);
            }

            return (Stream.Null, "application/octet-stream");
        }

        // Métodos adicionais necessários para o serviço
        public string GenerateTokenHash(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(token);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public DateTime ConvertToBrasiliaTime(DateTime utcTime)
        {
            TimeZoneInfo brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, brasiliaTimeZone);
        }

        public void ValidateDocumentCreation(DocumentoCriacaoDto dto)
        {
            ValidateDocumentoDto(dto);

            if (dto.NivelSeguranca != NivelSeguranca.Publico && string.IsNullOrWhiteSpace(dto.TokenSeguranca))
                throw new ArgumentException("Token de segurança é obrigatório para documentos não públicos.");
        }

        public void ValidateDocumentUpdate(DocumentoAtualizacaoDto dto, DocumentoModel doc)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (!string.IsNullOrWhiteSpace(dto.Descricao) && dto.Descricao.Length > 500)
                throw new ArgumentException("Descrição não pode ter mais que 500 caracteres.");
        }

        public string GetErrorMessage(string operation, Exception ex)
        {
            return $"Erro ao {operation} documento: {ex.Message}";
        }

        #region Private Methods

        private void ValidateTokenSecurity(string token, string storedTokenHash)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException(TokenObrigatorioMsg);

            token = token.Trim();
            string hashedToken = GenerateTokenHash(token);

            if (storedTokenHash != hashedToken)
                throw new UnauthorizedAccessException(TokenInvalidoMsg);
        }

        private void ValidateDocumentoDto(DocumentoCriacaoDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Usuario))
                throw new ArgumentException("Usuário não pode ser vazio.");

            if (string.IsNullOrWhiteSpace(dto.EmpresaContratante))
                throw new ArgumentException("Empresa contratante não pode ser vazia.");

            if (string.IsNullOrWhiteSpace(dto.NomePastaPrincipal))
                throw new ArgumentException("Nome da pasta principal não pode ser vazio.");

            if (dto.Arquivo == null || dto.Arquivo.Length == 0)
                throw new ArgumentException("Arquivo não pode ser vazio.");

            if (dto.Arquivo.Length > 100 * 1024 * 1024) // 100MB
                throw new ArgumentException("Tamanho máximo do arquivo é 100MB.");

            if (!IsValidFileType(dto.Arquivo.FileName))
                throw new ArgumentException("Tipo de arquivo não suportado.");
        }

        private bool IsValidFileType(string fileName)
        {
            var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".pptx", ".txt" };
            var extension = Path.GetExtension(fileName)?.ToLower();
            return allowedExtensions.Contains(extension);
        }

        private DocumentoDto MapToResponseDto(DocumentoModel doc)
        {
            return new DocumentoDto
            {
                Id = doc.Id,
                NomeArquivo = doc.NomeArquivo,
                UrlArquivo = doc.UrlArquivo,
                UsuarioCriador = doc.UsuarioCriador,
                DataCriacao = doc.DataCriacao,
                UsuarioUltimaAlteracao = doc.UsuarioUltimaAlteracao,
                DataUltimaAlteracao = doc.DataUltimaAlteracao ?? DateTime.MinValue,
                EmpresaContratante = doc.Empresa,
                NomePastaPrincipal = doc.NomePastaPrincipal,
                NomeSubPasta = doc.NomeSubPasta,
                TamanhoArquivo = doc.TamanhoArquivo,
                NivelSeguranca = (NivelSeguranca)Enum.Parse(typeof(NivelSeguranca), doc.NivelSeguranca),
                Descricao = doc.Descrição
            };
        }

        void IDocumentoBusinessRules.ValidateTokenSecurity(string token, string? tokenSeguranca)
        {
            ValidateTokenSecurity(token, tokenSeguranca);
        }

        DocumentoDto IDocumentoBusinessRules.MapToResponseDto(DocumentoModel doc)
        {
            return MapToResponseDto(doc);
        }
        #endregion
    }
}