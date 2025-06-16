using VoxDocs.Models;
using VoxDocs.DTO;
using System.Security.Claims;
using VoxDocs.Repository;

namespace VoxDocs.BusinessRules
{
    public class DocumentoOfflineBusinessRules : IDocumentoOfflineBusinessRules
    {
        private readonly IDocumentoRepository _documentoRepository;
        private readonly ILogger<DocumentoOfflineBusinessRules> _logger;

        public DocumentoOfflineBusinessRules(
            IDocumentoRepository documentoRepository,
            ILogger<DocumentoOfflineBusinessRules> logger)
        {
            _documentoRepository = documentoRepository;
            _logger = logger;
        }

        public async Task<BusinessResult<IEnumerable<DocumentoModel>>> GetDocumentsForOfflineAsync(string empresa, ClaimsPrincipal user)
        {
            try
            {
                var documentos = (await _documentoRepository.GetAllAsync())
                    .Where(d => d.Empresa == empresa)
                    .ToList();

                // Filtra documentos com base nas permissões do usuário
                var filteredDocs = documentos.Where(d => 
                    d.NivelSeguranca == "Publico" ||
                    (d.NivelSeguranca == "Restrito" && user.HasClaim("PermissionLevel", "Restrito")) ||
                    (d.NivelSeguranca == "Confidencial" && user.HasClaim("PermissionAccount", "admin"))
                );

                return new BusinessResult<IEnumerable<DocumentoModel>>(filteredDocs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar documentos para modo offline");
                return new BusinessResult<IEnumerable<DocumentoModel>>(
                    null,
                    false,
                    "Erro ao carregar documentos para modo offline",
                    500);
            }
        }

        public async Task<BusinessResult<bool>> ValidateDocumentForOfflineAccess(Guid documentId, ClaimsPrincipal user)
        {
            try
            {
                var documento = await _documentoRepository.GetByIdAsync(documentId);
                if (documento == null)
                {
                    return new BusinessResult<bool>(false, false, "Documento não encontrado", 404);
                }

                bool hasAccess = documento.NivelSeguranca switch
                {
                    "Publico" => true,
                    "Restrito" => user.HasClaim("PermissionLevel", "Restrito"),
                    "Confidencial" => user.HasClaim("PermissionAccount", "admin"),
                    _ => false
                };

                return new BusinessResult<bool>(hasAccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao validar acesso offline para documento {documentId}");
                return new BusinessResult<bool>(false, false, "Erro ao validar acesso", 500);
            }
        }
    }
}