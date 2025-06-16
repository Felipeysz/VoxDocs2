using VoxDocs.Data;
using VoxDocs.Models;
using VoxDocs.DTO;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using VoxDocs.BusinessRules;

namespace VoxDocs.Services
{
    public class DocumentosOfflineService : IDocumentosOfflineService
    {
        private readonly VoxDocsContext _context;
        private readonly IDocumentoOfflineBusinessRules _offlineBR;
        private readonly IMemoryCache _cache;
        private const string CachePrefix = "OfflineDocs_";

        public DocumentosOfflineService(
            VoxDocsContext context,
            IDocumentoOfflineBusinessRules offlineBR,
            IMemoryCache cache)
        {
            _context = context;
            _offlineBR = offlineBR;
            _cache = cache;
        }

        public async Task<IEnumerable<DocumentoDto>> GetDocumentsForOfflineAsync(string empresa, ClaimsPrincipal user)
        {
            var result = await _offlineBR.GetDocumentsForOfflineAsync(empresa, user);
            
            if (!result.Success)
            {
                return Enumerable.Empty<DocumentoDto>();
            }

            // Mapeia os modelos para DTOs
            var documentosDto = result.Data.Select(MapDocumentoToDto).ToList();
            
            // Armazena em cache para acesso offline posterior
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            await CacheUserDocumentsAsync(userId, documentosDto);
            
            return documentosDto;
        }

        public async Task<bool> ValidateDocumentForOfflineAccess(Guid documentId, ClaimsPrincipal user)
        {
            var result = await _offlineBR.ValidateDocumentForOfflineAccess(documentId, user);
            return result.Success && result.Data;
        }

        public Task CacheUserDocumentsAsync(string userId, IEnumerable<DocumentoDto> documents)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(6)) // Expira após 6 horas de inatividade
                .SetPriority(CacheItemPriority.High);
            
            _cache.Set($"{CachePrefix}{userId}", documents.ToList(), cacheEntryOptions);
            
            return Task.CompletedTask;
        }

        public Task<IEnumerable<DocumentoDto>> GetCachedUserDocumentsAsync(string userId)
        {
            if (_cache.TryGetValue($"{CachePrefix}{userId}", out List<DocumentoDto> documents))
            {
                return Task.FromResult<IEnumerable<DocumentoDto>>(documents);
            }
            
            return Task.FromResult(Enumerable.Empty<DocumentoDto>());
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
    }

    public class CustomException : Exception
    {
        public int StatusCode { get; }

        public CustomException(string message, int statusCode = 400) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}