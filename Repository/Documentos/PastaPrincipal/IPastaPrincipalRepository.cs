using Microsoft.EntityFrameworkCore;
using VoxDocs.Data;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public interface IPastaPrincipalRepository
    {
        Task<IEnumerable<PastaPrincipalModel>> GetAllAsync();
        Task<PastaPrincipalModel> GetByNamePrincipalAsync(string nomePasta);
        Task<IEnumerable<PastaPrincipalModel>> GetByEmpresaAsync(string empresaContratante);
        Task<PastaPrincipalModel?> GetByIdAsync(int id);
        Task<PastaPrincipalModel> CreateAsync(PastaPrincipalModel pasta);
        Task<bool> DeleteAsync(int id);
    }
}