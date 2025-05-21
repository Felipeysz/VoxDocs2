using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VoxDocs.Data;
using VoxDocs.Models;
using VoxDocs.DTO;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace VoxDocs.Services
{
    public class DocumentoService : IDocumentoService
    {
        private readonly VoxDocsContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public DocumentoService(VoxDocsContext context, IConfiguration configuration)
        {
            _context = context;
            var ConnectionString = configuration["AzureBlobStorage:ConnectionString"];
            _blobServiceClient = new BlobServiceClient(ConnectionString);
            _containerName = configuration["AzureBlobStorage:ContainerName"];
        }

        public async Task<DTODocumentoCreate> GetByIdAsync(int id, string? token = null)
        {
            var doc = await _context.Documentos.FindAsync(id);
            if (doc != null && doc.NivelSeguranca != "Publico" && doc.TokenSeguranca != token)
                throw new UnauthorizedAccessException("Token de segurança inválido");

            return doc != null ? MapToDTO(doc) : null;
        }

        public async Task<IEnumerable<DTODocumentoCreate>> GetAllAsync()
        {
            var documentos = await _context.Documentos.ToListAsync();
            return documentos.Select(MapToDTO);
        }

        public async Task<IEnumerable<DTODocumentoCreate>> GetBySubPastaAsync(string subPasta)
        {
            var documentos = await _context.Documentos
                .Where(d => d.NomeSubPasta == subPasta)
                .ToListAsync();
            return documentos.Select(MapToDTO);
        }

        public async Task<IEnumerable<DTODocumentoCreate>> GetByPastaPrincipalAsync(string pastaPrincipal)
        {
            var documentos = await _context.Documentos
                .Where(d => d.NomePastaPrincipal == pastaPrincipal)
                .ToListAsync();
            return documentos.Select(MapToDTO);
        }

        public async Task<DTODocumentoCreate> CreateAsync(DocumentoDto dto)
        {
            try
            {
                // Extrair extensão do arquivo
                var ext = Path.GetExtension(dto.Arquivo.FileName);

                // Nome base do arquivo (sem extensão)
                var nomeBase = Path.GetFileNameWithoutExtension(dto.Arquivo.FileName);

                // Novo nome do arquivo (sem o ":")
                var fileName = $"{nomeBase}{ext}";

                // Verificar se o arquivo já existe e adicionar sufixo numérico se necessário
                int contador = 1;
                while (await _context.Documentos.AnyAsync(d => d.NomeArquivo == fileName))
                {
                    fileName = $"{nomeBase}_{contador}{ext}";
                    contador++;
                }

                // Upload do arquivo
                string url;
                using (var stream = dto.Arquivo.OpenReadStream())
                {
                    url = await UploadFileAsync(fileName, stream);
                }

                // Criar o documento
                var doc = new DocumentoModel
                {
                    NomeArquivo = fileName,
                    UrlArquivo = url,
                    UsuarioCriador = dto.Usuario,
                    DataCriacao = DateTime.UtcNow,
                    UsuarioUltimaAlteracao = dto.Usuario,
                    DataUltimaAlteracao = DateTime.UtcNow,
                    Empresa = dto.Empresa,
                    NomePastaPrincipal = dto.NomePastaPrincipal,
                    NomeSubPasta = dto.NomeSubPasta,
                    TamanhoArquivo = dto.Arquivo.Length,
                    NivelSeguranca = dto.NivelSeguranca,
                    TokenSeguranca = dto.TokenSeguranca,
                    Descrição = dto.Descrição // Garantir que a descrição seja preenchida
                };

                _context.Documentos.Add(doc);
                await _context.SaveChangesAsync();

                return MapToDTO(doc);
            }
            catch (Exception ex)
            {
                // Log do erro para depuração
                Console.WriteLine($"Erro ao salvar o documento: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw; // Re-lança a exceção para ser tratada no controller
            }
        }
        public async Task<DTODocumentoCreate> UpdateAsync(DTODocumentoCreate dto)
        {
            var doc = await _context.Documentos.FindAsync(dto.Id);
            if (doc == null) return null;

            doc.NomeArquivo = dto.NomeArquivo;
            doc.UrlArquivo = dto.UrlArquivo;
            doc.UsuarioCriador = dto.UsuarioCriador;
            doc.DataCriacao = dto.DataCriacao;
            doc.UsuarioUltimaAlteracao = dto.UsuarioUltimaAlteracao;
            doc.DataUltimaAlteracao = dto.DataUltimaAlteracao;
            doc.Empresa = dto.Empresa;
            doc.NomePastaPrincipal = dto.NomePastaPrincipal;
            doc.NomeSubPasta = dto.NomeSubPasta;
            doc.TamanhoArquivo = dto.TamanhoArquivo;
            doc.NivelSeguranca = dto.NivelSeguranca;
            doc.TokenSeguranca = dto.TokenSeguranca;
            doc.Descrição = dto.Descrição;

            _context.Documentos.Update(doc);
            await _context.SaveChangesAsync();

            return MapToDTO(doc);
        }

        public async Task DeleteAsync(int id)
        {
            var doc = await _context.Documentos.FindAsync(id);
            if (doc != null)
            {
                _context.Documentos.Remove(doc);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<DTOQuantidadeDocumentoEmpresa> GetEstatisticasEmpresaAsync(string empresa)
        {
            var documentos = await _context.Documentos
                .Where(d => d.Empresa == empresa)
                .ToListAsync();

            return new DTOQuantidadeDocumentoEmpresa
            {
                NomeEmpresa = empresa,
                Quantidade = documentos.Count,
                TamanhoTotalGb = documentos.Sum(d => d.TamanhoArquivo) / 1024.0 / 1024.0 / 1024.0
            };
        }

        public async Task<DTOAcessosDocumento> GetAcessosDocumentoAsync(int id, int dias)
        {
            var doc = await _context.Documentos.FindAsync(id);
            if (doc == null) return null;

            return new DTOAcessosDocumento
            {
                NomeArquivo = doc.NomeArquivo,
                NomeSubPasta = doc.NomeSubPasta,
                NomePastaPrincipal = doc.NomePastaPrincipal,
                QuantidadeAcessos = doc.ContadorAcessos
            };
        }

        public async Task IncrementarAcessoAsync(int id)
        {
            var doc = await _context.Documentos.FindAsync(id);
            if (doc != null)
            {
                doc.ContadorAcessos++;
                await _context.SaveChangesAsync();
            }
        }

       private async Task<string> UploadFileAsync(string fileName, Stream fileStream)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            // Cria o container se não existir, mantendo a política padrão (sem acesso público)
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, overwrite: true);

            return blobClient.Uri.ToString();
        }

        public async Task<bool> ArquivoExisteAsync(string nomeArquivo)
        {
            return await _context.Documentos.AnyAsync(d => d.NomeArquivo == nomeArquivo);
        }

        private DTODocumentoCreate MapToDTO(DocumentoModel doc)
        {
            return new DTODocumentoCreate
            {
                Id = doc.Id,
                NomeArquivo = doc.NomeArquivo,
                UrlArquivo = doc.UrlArquivo,
                UsuarioCriador = doc.UsuarioCriador,
                DataCriacao = doc.DataCriacao,
                UsuarioUltimaAlteracao = doc.UsuarioUltimaAlteracao,
                DataUltimaAlteracao = doc.DataUltimaAlteracao,
                Empresa = doc.Empresa,
                NomePastaPrincipal = doc.NomePastaPrincipal,
                NomeSubPasta = doc.NomeSubPasta,
                TamanhoArquivo = doc.TamanhoArquivo,
                NivelSeguranca = doc.NivelSeguranca,
                TokenSeguranca = doc.TokenSeguranca,
                Descrição = doc.Descrição
            };
        }
    }
}