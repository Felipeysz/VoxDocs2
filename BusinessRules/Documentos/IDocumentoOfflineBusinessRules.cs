using VoxDocs.Models;
using System.Security.Claims;
using VoxDocs.BusinessRules;

namespace VoxDocs.BusinessRules
{
    public interface IDocumentoOfflineBusinessRules
    {
        Task<BusinessResult<IEnumerable<DocumentoModel>>> GetDocumentsForOfflineAsync(string empresa, ClaimsPrincipal user);
        Task<BusinessResult<bool>> ValidateDocumentForOfflineAccess(Guid documentId, ClaimsPrincipal user);
    }
}