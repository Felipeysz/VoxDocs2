using VoxDocs.Data;
using VoxDocs.Models;
using VoxDocs.DTO;
using Azure.Storage.Blobs;
using VoxDocs.BusinessRules;
using VoxDocs.Data.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace VoxDocs.Services
{

    public class DocumentosPastasService : IDocumentosPastasService
    {
        private readonly VoxDocsContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;
        private readonly IDocumentoBusinessRules _documentoBusinessRules;
        private readonly IPastaPrincipalBusinessRules _pastaPrincipalBusinessRules;
        private readonly ISubPastaBusinessRules _subPastaBusinessRules;
        private readonly IPastaPrincipalRepository _pastaPrincipalRepository;
        private readonly ISubPastaRepository _subPastaRepository;
        private readonly IDocumentoOfflineBusinessRules _offlineBR;
        private readonly IEmpresasContratanteBusinessRules _empresasContratanteBusinessRules;
        private readonly IMemoryCache _cache;
        private const string CachePrefix = "OfflineDocs_";

        public DocumentosPastasService(
            VoxDocsContext context,
            IConfiguration configuration,
            IDocumentoBusinessRules documentoBusinessRules,
            IPastaPrincipalBusinessRules pastaPrincipalBusinessRules,
            IPastaPrincipalRepository pastaPrincipalRepository,
            ISubPastaBusinessRules subPastaBusinessRules,
            ISubPastaRepository subPastaRepository,
            IDocumentoOfflineBusinessRules offlineBR,
            IMemoryCache cache,
            IEmpresasContratanteBusinessRules empresasContratanteBusinessRules)
        {
            _context = context;
            var connectionString = configuration["AzureBlobStorage:ConnectionString"];
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = configuration["AzureBlobStorage:ContainerName"];
            _documentoBusinessRules = documentoBusinessRules;
            _pastaPrincipalRepository = pastaPrincipalRepository;
            _pastaPrincipalBusinessRules = pastaPrincipalBusinessRules;
            _subPastaBusinessRules = subPastaBusinessRules;
            _subPastaRepository = subPastaRepository;
            _offlineBR = offlineBR;
            _cache = cache;
            _empresasContratanteBusinessRules = empresasContratanteBusinessRules;
        }

        #region Documento Methods

        public async Task<bool> ValidateTokenDocumentoAsync(string nomeArquivo, string token)
        {
            return await _documentoBusinessRules.ValidateTokenDocumentoAsync(nomeArquivo, token);
        }

        public async Task<DocumentoDto> GetDocumentoByIdAsync(Guid id, string? token = null)
        {
            var doc = await _documentoBusinessRules.GetByIdAsync(id, token);
            return MapDocumentoToDto(doc);
        }

        public async Task<IEnumerable<DocumentoDto>> GetAllDocumentosAsync()
        {
            var docs = await _documentoBusinessRules.GetAllAsync();
            return docs.Select(MapDocumentoToDto);
        }

        public async Task<IEnumerable<DocumentoDto>> GetDocumentosBySubPastaAsync(string subPasta)
        {
            var docs = await _documentoBusinessRules.GetBySubPastaAsync(subPasta);
            return docs.Select(MapDocumentoToDto);
        }

        public async Task<IEnumerable<DocumentoDto>> GetDocumentosByPastaPrincipalAsync(string pastaPrincipal)
        {
            var docs = await _documentoBusinessRules.GetByPastaPrincipalAsync(pastaPrincipal);
            return docs.Select(MapDocumentoToDto);
        }

        public async Task<DocumentoDto> CreateDocumentoAsync(DocumentoCriacaoDto dto)
        {
            try
            {
                using var stream = dto.Arquivo.OpenReadStream();

                var doc = new DocumentoModel
                {
                    NomeArquivo = dto.Arquivo.FileName,
                    UsuarioCriador = dto.Usuario ?? throw new ArgumentNullException(nameof(dto.Usuario)),
                    DataCriacao = DateTime.UtcNow,
                    Empresa = dto.EmpresaContratante ?? throw new ArgumentNullException(nameof(dto.EmpresaContratante)),
                    NomePastaPrincipal = dto.NomePastaPrincipal ?? throw new ArgumentNullException(nameof(dto.NomePastaPrincipal)),
                    NomeSubPasta = dto.NomeSubPasta ?? throw new ArgumentNullException(nameof(dto.NomeSubPasta)),
                    TamanhoArquivo = dto.Arquivo.Length,
                    NivelSeguranca = dto.NivelSeguranca.ToString(),
                    TokenSeguranca = dto.TokenSeguranca,
                    Descrição = dto.Descricao
                };

                var createdDoc = await _documentoBusinessRules.CreateAsync(doc, stream, dto.Usuario);

                // Upload do arquivo para o Blob Storage
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                await containerClient.CreateIfNotExistsAsync();
                var blobClient = containerClient.GetBlobClient(createdDoc.NomeArquivo);

                stream.Position = 0;
                await blobClient.UploadAsync(stream, overwrite: false);

                createdDoc.UrlArquivo = blobClient.Uri.ToString();
                await _context.SaveChangesAsync();

                return MapDocumentoToDto(createdDoc);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar documento: {ex.Message}");
                throw new Exception(_documentoBusinessRules.GetErrorMessage("Create", ex));
            }
        }

        public async Task DeleteDocumentoAsync(Guid id, string? token)
        {
            var doc = await _documentoBusinessRules.GetByIdAsync(id, token);

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(doc.NomeArquivo);
            await blobClient.DeleteIfExistsAsync();

            await _documentoBusinessRules.DeleteAsync(id, token);
        }

        public async Task<DocumentoDto> UpdateDocumentoAsync(DocumentoAtualizacaoDto dto)
        {
            var existingDoc = await _documentoBusinessRules.GetByIdAsync(dto.Id, dto.TokenSeguranca);

            existingDoc.UsuarioUltimaAlteracao = dto.UsuarioUltimaAlteracao;
            existingDoc.DataUltimaAlteracao = DateTime.UtcNow;

            if (dto.NivelSeguranca.HasValue)
            {
                existingDoc.NivelSeguranca = dto.NivelSeguranca.Value.ToString();
            }

            if (!string.IsNullOrEmpty(dto.TokenSeguranca))
            {
                existingDoc.TokenSeguranca = dto.TokenSeguranca;
            }

            if (!string.IsNullOrEmpty(dto.Descricao))
            {
                existingDoc.Descrição = dto.Descricao;
            }

            Stream novoArquivoStream = null;
            if (dto.NovoArquivo != null)
            {
                novoArquivoStream = dto.NovoArquivo.OpenReadStream();
                existingDoc.NomeArquivo = dto.NovoArquivo.FileName;
                existingDoc.TamanhoArquivo = dto.NovoArquivo.Length;
            }

            var updatedDoc = await _documentoBusinessRules.UpdateAsync(existingDoc, novoArquivoStream);

            if (dto.NovoArquivo != null)
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var oldBlobClient = containerClient.GetBlobClient(existingDoc.NomeArquivo);
                await oldBlobClient.DeleteIfExistsAsync();

                var newBlobClient = containerClient.GetBlobClient(updatedDoc.NomeArquivo);
                novoArquivoStream.Position = 0;
                await newBlobClient.UploadAsync(novoArquivoStream, overwrite: true);

                updatedDoc.UrlArquivo = newBlobClient.Uri.ToString();
                await _context.SaveChangesAsync();
            }

            return MapDocumentoToDto(updatedDoc);
        }

        public async Task<DocumentoEstatisticasDto> GetEstatisticasEmpresaAsync(string empresa)
        {
            var estatisticas = await _documentoBusinessRules.GetEstatisticasEmpresaAsync(empresa);

            var docs = await _documentoBusinessRules.GetAllAsync();
            var docsEmpresa = docs.Where(d => d.Empresa == empresa).ToList();

            return new DocumentoEstatisticasDto
            {
                EmpresaContratante = estatisticas.EmpresaContratante,
                QuantidadeDocumentos = estatisticas.QuantidadeDocumentos,
                TamanhoTotalGb = estatisticas.TamanhoTotalGb,
                Publicos = docsEmpresa.Count(d => d.NivelSeguranca == "Publico"),
                Restritos = docsEmpresa.Count(d => d.NivelSeguranca == "Restrito"),
                Confidenciais = docsEmpresa.Count(d => d.NivelSeguranca == "Confidencial")
            };
        }

        public async Task IncrementarAcessoDocumentoAsync(Guid id)
        {
            await _documentoBusinessRules.IncrementarAcessoAsync(id);
        }

        public async Task<bool> ArquivoExisteAsync(string nomeArquivo)
        {
            return await _documentoBusinessRules.ArquivoExisteAsync(nomeArquivo);
        }

        public async Task<(Stream stream, string contentType)> DownloadDocumentoProtegidoAsync(string nomeArquivo, string token = null)
        {
            await _documentoBusinessRules.ValidateTokenDocumentoAsync(nomeArquivo, token);

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(nomeArquivo);

            if (!await blobClient.ExistsAsync())
                throw new FileNotFoundException(DocumentoBusinessRules.ArquivoNaoEncontradoMsg);

            var downloadInfo = await blobClient.DownloadAsync();
            var contentType = downloadInfo.Value.ContentType ?? "application/octet-stream";
            return (downloadInfo.Value.Content, contentType);
        }


        private DocumentoDto MapDocumentoToDto(DocumentoModel model)
        {
            if (model == null) return null;

            return new DocumentoDto
            {
                Id = model.Id,
                NomeArquivo = model.NomeArquivo,
                UrlArquivo = model.UrlArquivo,
                UsuarioCriador = model.UsuarioCriador,
                DataCriacao = model.DataCriacao,
                UsuarioUltimaAlteracao = model.UsuarioUltimaAlteracao,
                DataUltimaAlteracao = model.DataUltimaAlteracao ?? DateTime.MinValue,
                EmpresaContratante = model.Empresa,
                NomePastaPrincipal = model.NomePastaPrincipal,
                NomeSubPasta = model.NomeSubPasta,
                TamanhoArquivo = model.TamanhoArquivo,
                NivelSeguranca = Enum.Parse<NivelSeguranca>(model.NivelSeguranca),
                Descricao = model.Descrição
            };
        }

        #endregion

        #region PastaPrincipal Methods

        public async Task<IEnumerable<DTOPastaPrincipal>> GetAllPastasPrincipaisAsync()
        {
            var pastas = await _pastaPrincipalRepository.GetAllAsync();
            return MapPastasToDTO(pastas);
        }

        public async Task<DTOPastaPrincipal> GetPastaPrincipalByNameAsync(string nomePasta)
        {
            var result = await _pastaPrincipalBusinessRules.ValidateGetByNameAsync(nomePasta);
            if (!result.Success)
            {
                throw new CustomException(result.ErrorMessage, result.StatusCode);
            }
            return MapPastaToDTO(result.Data);
        }

        public async Task<IEnumerable<DTOPastaPrincipal>> GetPastasPrincipaisByEmpresaAsync(string empresaContratante)
        {
            var result = await _pastaPrincipalBusinessRules.ValidateGetByEmpresaAsync(empresaContratante);
            if (!result.Success)
            {
                throw new CustomException(result.ErrorMessage, result.StatusCode);
            }
            return MapPastasToDTO(result.Data);
        }

        public async Task<DTOPastaPrincipal?> GetPastaPrincipalByIdAsync(Guid id)
        {
            var result = await _pastaPrincipalBusinessRules.ValidateGetByIdAsync(id);
            if (!result.Success && result.StatusCode != 404)
            {
                throw new CustomException(result.ErrorMessage, result.StatusCode);
            }
            return result.Data != null ? MapPastaToDTO(result.Data) : null;
        }

        public async Task<DTOPastaPrincipal> CreatePastaPrincipalAsync(DTOPastaPrincipalCreate dto)
        {
            // Validação da empresa usando apenas condições positivas
            var empresaValidation = await _empresasContratanteBusinessRules.ValidarGetByNomeAsync(dto.EmpresaContratante);
            
            if (empresaValidation == null || empresaValidation.Data == null)
            {
                throw new CustomException("Empresa contratante não encontrada", StatusCodes.Status404NotFound);
            }

            // Criação do modelo
            var model = new PastaPrincipalModel
            {
                NomePastaPrincipal = dto.NomePastaPrincipal,
                EmpresaContratante = dto.EmpresaContratante
            };

            // Validação e criação
            var creationResult = await _pastaPrincipalBusinessRules.ValidateAndCreateAsync(model);
            
            if (creationResult == null || creationResult.Success.Equals(false))
            {
                throw new CustomException(
                    creationResult?.ErrorMessage ?? "Falha ao criar pasta", 
                    creationResult?.StatusCode ?? StatusCodes.Status400BadRequest);
            }

            return MapPastaToDTO(creationResult.Data);
        }

        public async Task<bool> DeletePastaPrincipalAsync(Guid id)
        {
            var result = await _pastaPrincipalBusinessRules.ValidateAndDeleteAsync(id);
            if (!result.Success)
            {
                throw new CustomException(result.ErrorMessage, result.StatusCode);
            }
            return result.Data;
        }

        #endregion

        #region Métodos de Mapeamento

        private IEnumerable<DTOPastaPrincipal> MapPastasToDTO(IEnumerable<PastaPrincipalModel> pastas)
        {
            return pastas.Select(p => new DTOPastaPrincipal
            {
                Id = p.Id,
                NomePastaPrincipal = p.NomePastaPrincipal,
                EmpresaContratante = p.EmpresaContratante,
                Quantidade = p.SubPastas?.Count ?? 0
            });
        }

        private DTOPastaPrincipal MapPastaToDTO(PastaPrincipalModel pasta)
        {
            return new DTOPastaPrincipal
            {
                Id = pasta.Id,
                NomePastaPrincipal = pasta.NomePastaPrincipal,
                EmpresaContratante = pasta.EmpresaContratante,
                Quantidade = pasta.SubPastas?.Count ?? 0
            };
        }

        #endregion

        #region SubPasta Methods

        public async Task<IEnumerable<DTOSubPasta>> GetAllSubPastasAsync()
        {
            var subPastas = await _subPastaRepository.GetAllAsync();
            return subPastas.Select(sp => new DTOSubPasta
            {
                Id = sp.Id,
                NomeSubPasta = sp.NomeSubPasta,
                NomePastaPrincipal = sp.NomePastaPrincipal,
                EmpresaContratante = sp.EmpresaContratante,
                Quantidade = sp.Documentos?.Count ?? 0
            });
        }

        public async Task<IEnumerable<DTOSubPasta>> GetSubPastasByEmpresaAsync(string empresa)
        {
            var subPastas = await _subPastaRepository.GetByEmpresaAsync(empresa);
            return subPastas.Select(s => new DTOSubPasta
            {
                Id = s.Id,
                NomeSubPasta = s.NomeSubPasta,
                EmpresaContratante = s.EmpresaContratante
            });
        }

        public async Task<DTOSubPasta?> GetSubPastaByNameAsync(string nomeSubPasta)
        {
            var model = await _subPastaRepository.GetByNameSubPastaAsync(nomeSubPasta);
            return model is null ? null : MapSubPastaToDTO(model);
        }

        public async Task<DTOSubPasta?> GetSubPastaByIdAsync(Guid id)
        {
            var subPasta = await _subPastaRepository.GetByIdAsync(id);
            return subPasta is null ? null : MapSubPastaToDTO(subPasta);
        }

        public async Task<DTOSubPasta> CreateSubPastaAsync(DTOSubPastaCreate dto)
        {
            var subPasta = new SubPastaModel
            {
                NomeSubPasta = dto.NomeSubPasta,
                NomePastaPrincipal = dto.NomePastaPrincipal,
                EmpresaContratante = dto.EmpresaContratante
            };

            var created = await _subPastaBusinessRules.ValidateAndCreateSubPastaAsync(subPasta);
            return MapSubPastaToDTO(created);
        }

        public async Task<bool> DeleteSubPastaAsync(Guid id)
        {
            if (!await _subPastaBusinessRules.CanDeleteSubPastaAsync(id))
                return false;

            return await _subPastaRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<DTOSubPasta>> GetSubPastasByPastaPrincipalAsync(string nomePastaPrincipal)
        {
            var subPastas = await _subPastaRepository.GetSubChildrenAsync(nomePastaPrincipal);
            return subPastas.Select(sp => MapSubPastaToDTO(sp));
        }

        private static DTOSubPasta MapSubPastaToDTO(SubPastaModel model)
        {
            return new DTOSubPasta
            {
                Id = model.Id,
                NomeSubPasta = model.NomeSubPasta,
                NomePastaPrincipal = model.NomePastaPrincipal,
                EmpresaContratante = model.EmpresaContratante,
                Quantidade = model.Documentos?.Count ?? 0
            };
        }

        #endregion

        public class CustomException : Exception
        {
            public int StatusCode { get; }

            public CustomException(string message, int statusCode = 400) : base(message)
            {
                StatusCode = statusCode;
            }
        }
    }
}