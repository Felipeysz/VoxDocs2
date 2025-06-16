using System.Security.Claims;
using VoxDocs.DTO;

namespace VoxDocs.Services
{
    public interface IDocumentosOfflineService
    {
        Task<IEnumerable<DocumentoDto>> GetDocumentsForOfflineAsync(string empresa, ClaimsPrincipal user);
        Task<bool> ValidateDocumentForOfflineAccess(Guid documentId, ClaimsPrincipal user);
        Task CacheUserDocumentsAsync(string userId, IEnumerable<DocumentoDto> documents);
        Task<IEnumerable<DocumentoDto>> GetCachedUserDocumentsAsync(string userId);
    }
}