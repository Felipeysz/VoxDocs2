using VoxDocs.DTO;

namespace VoxDocs.Services
{
    public interface IPastaPrincipalService
    {
        Task<IEnumerable<DTOPastaPrincipal>> GetAllAsync();
        Task<DTOPastaPrincipal> GetByNamePrincipalAsync(string nomePasta);
        Task<IEnumerable<DTOPastaPrincipal>> GetByEmpresaAsync(string empresaContratante);
        Task<DTOPastaPrincipal?> GetByIdAsync(int id);
        Task<DTOPastaPrincipal> CreateAsync(DTOPastaPrincipalCreate dto);
        Task<bool> DeleteAsync(int id);
    }
}