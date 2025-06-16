// IPastaPrincipalRepository.cs
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public interface IPastaPrincipalRepository
    {
        Task<IEnumerable<PastaPrincipalModel>> GetAllAsync();
        Task<PastaPrincipalModel?> GetByNamePrincipalAsync(string nomePasta);
        Task<IEnumerable<PastaPrincipalModel>> GetByEmpresaAsync(string empresaContratante);
        Task<PastaPrincipalModel?> GetByIdAsync(Guid id);
        Task<PastaPrincipalModel> CreateAsync(PastaPrincipalModel pasta);
        Task<bool> DeleteAsync(Guid id);
        Task<PastaPrincipalModel?> GetByNameAndEmpresaAsync(string nomePasta, string empresaContratante);
    }
}