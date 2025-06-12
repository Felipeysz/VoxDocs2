using Microsoft.EntityFrameworkCore;
using VoxDocs.Data;
using VoxDocs.Models;
using VoxDocs.DTO;
using Azure.Storage.Blobs;
using System.Security.Cryptography;
using System.Text;
using VoxDocs.BusinessRules;

namespace VoxDocs.Services
{
    public class DocumentoService : IDocumentoService
    {
        private readonly VoxDocsContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly IDocumentoBusinessRules _businessRules;

        public DocumentoService(VoxDocsContext context, IConfiguration configuration, IDocumentoBusinessRules businessRules)
        {
            _context = context;
            var connectionString = configuration["AzureBlobStorage:ConnectionString"];
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = configuration["AzureBlobStorage:ContainerName"];
            _businessRules = businessRules;
        }

        public async Task<bool> ValidateTokenDocumentoAsync(string nomeArquivo, string token)
        {
            return await _businessRules.ValidateTokenDocumentoAsync(nomeArquivo, token);
        }

        public async Task<DocumentoDto> GetByIdAsync(Guid id, string? token = null)
        {
            return await _businessRules.GetByIdAsync(id, token);
        }

        public async Task<IEnumerable<DocumentoDto>> GetAllAsync()
        {
            return await _businessRules.GetAllAsync();
        }

        public async Task<IEnumerable<DocumentoDto>> GetBySubPastaAsync(string subPasta)
        {
            return await _businessRules.GetBySubPastaAsync(subPasta);
        }

        public async Task<IEnumerable<DocumentoDto>> GetByPastaPrincipalAsync(string pastaPrincipal)
        {
            return await _businessRules.GetByPastaPrincipalAsync(pastaPrincipal);
        }

        public async Task<DocumentoDto> CreateAsync(DocumentoCriacaoDto dto)
        {
            try
            {
                // Validação das regras de negócio
                _businessRules.ValidateDocumentCreation(dto);

                // Upload do arquivo para o Blob Storage
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync();

                var blobName = dto.Arquivo.FileName;
                var blobClient = containerClient.GetBlobClient(blobName);

                using (var stream = dto.Arquivo.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: false);
                }

                var url = blobClient.Uri.ToString();

                // Criação do documento no banco de dados
                var doc = new DocumentoModel
                {
                    NomeArquivo = dto.Arquivo.FileName,
                    UrlArquivo = url,
                    UsuarioCriador = dto.Usuario,
                    DataCriacao = _businessRules.ConvertToBrasiliaTime(DateTime.UtcNow),
                    UsuarioUltimaAlteracao = dto.Usuario,
                    DataUltimaAlteracao = _businessRules.ConvertToBrasiliaTime(DateTime.UtcNow),
                    Empresa = dto.EmpresaContratante,
                    NomePastaPrincipal = dto.NomePastaPrincipal,
                    NomeSubPasta = dto.NomeSubPasta,
                    TamanhoArquivo = dto.Arquivo.Length,
                    NivelSeguranca = dto.NivelSeguranca.ToString(),
                    TokenSeguranca = _businessRules.GenerateTokenHash(dto.TokenSeguranca),
                    Descrição = dto.Descricao // Corrigido de 'Descrição' para 'Descricao'
                };

                _context.Documentos.Add(doc);
                await _context.SaveChangesAsync();

                return _businessRules.MapToResponseDto(doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar documento: {ex.Message}");
                throw new Exception(_businessRules.GetErrorMessage("Create", ex));
            }
        }

        public async Task DeleteAsync(Guid id, string? token)
        {
            var doc = await _context.Documentos.FindAsync(id);
            if (doc == null)
                throw new ArgumentException(DocumentoBusinessRules.DocumentoNaoEncontradoMsg);

            // Validação de token para documentos não públicos
            if (doc.NivelSeguranca != "Publico")
            {
                _businessRules.ValidateTokenSecurity(token, doc.TokenSeguranca);
            }

            var blobName = doc.NomeArquivo;

            // Exclusão do blob
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();

            // Exclusão do registro
            _context.Documentos.Remove(doc);
            await _context.SaveChangesAsync();
        }

        public async Task<DocumentoDto> UpdateAsync(DocumentoAtualizacaoDto dto)
        {
            var doc = await _context.Documentos.FindAsync(dto.Id);
            if (doc == null)
            {
                throw new ArgumentException(DocumentoBusinessRules.DocumentoNaoEncontradoMsg);
            }

            // Validação das regras de negócio para atualização
            _businessRules.ValidateDocumentUpdate(dto, doc);

            // Campos que serão atualizados
            doc.UsuarioUltimaAlteracao = dto.UsuarioUltimaAlteracao;
            doc.DataUltimaAlteracao = _businessRules.ConvertToBrasiliaTime(DateTime.UtcNow);
            doc.Descrição = dto.Descricao; // Corrigido de 'Descrição' para 'Descricao'

            // Atualiza NivelSeguranca se fornecido
            if (dto.NivelSeguranca.HasValue)
            {
                doc.NivelSeguranca = dto.NivelSeguranca.Value.ToString();
            }

            // Atualiza TokenSeguranca se fornecido
            if (!string.IsNullOrEmpty(dto.TokenSeguranca))
            {
                doc.TokenSeguranca = _businessRules.GenerateTokenHash(dto.TokenSeguranca);
            }

            // Processa novo arquivo se fornecido
            if (dto.NovoArquivo != null && dto.NovoArquivo.Length > 0)
            {
                // Exclui o blob antigo
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var oldBlobClient = containerClient.GetBlobClient(doc.NomeArquivo);
                await oldBlobClient.DeleteIfExistsAsync();

                // Faz upload do novo blob
                var newBlobClient = containerClient.GetBlobClient(dto.NovoArquivo.FileName);
                using (var stream = dto.NovoArquivo.OpenReadStream())
                {
                    await newBlobClient.UploadAsync(stream, overwrite: true);
                }

                // Atualiza o nome e tamanho do arquivo
                doc.NomeArquivo = dto.NovoArquivo.FileName;
                doc.TamanhoArquivo = dto.NovoArquivo.Length;
            }

            _context.Documentos.Update(doc);
            await _context.SaveChangesAsync();

            return _businessRules.MapToResponseDto(doc);
        }

        public async Task<DocumentoEstatisticasDto> GetEstatisticasEmpresaAsync(string empresa)
        {
            return await _businessRules.GetEstatisticasEmpresaAsync(empresa);
        }

        public async Task IncrementarAcessoAsync(Guid id)
        {
            await _businessRules.IncrementarAcessoAsync(id);
        }

        public async Task<bool> ArquivoExisteAsync(string nomeArquivo)
        {
            return await _businessRules.ArquivoExisteAsync(nomeArquivo);
        }

        public async Task<(Stream stream, string contentType)> DownloadDocumentoProtegidoAsync(string nomeArquivo, string token = null)
        {
            var doc = await _context.Documentos.FirstOrDefaultAsync(d => d.NomeArquivo == nomeArquivo);
            if (doc == null)
                throw new ArgumentException(DocumentoBusinessRules.DocumentoNaoEncontradoMsg);

            // Validação com hash
            if (doc.NivelSeguranca != "Publico")
            {
                _businessRules.ValidateTokenSecurity(token, doc.TokenSeguranca);
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(nomeArquivo);

            if (!await blobClient.ExistsAsync())
                throw new FileNotFoundException(DocumentoBusinessRules.ArquivoNaoEncontradoMsg);
            var downloadInfo = await blobClient.DownloadAsync();
            var contentType = downloadInfo.Value.ContentType ?? "application/octet-stream";
            return (downloadInfo.Value.Content, contentType);
        }
    }
}