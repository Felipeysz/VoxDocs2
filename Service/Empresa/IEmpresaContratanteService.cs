using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public interface IEmpresasContratanteService
    {
        Task<List<EmpresasContratanteModel>> GetAllAsync();
        Task<EmpresasContratanteModel> GetByIdAsync(int id);
        Task<EmpresasContratanteModel> GetEmpresaByNome(string nome); // ✅ Novo método
        Task<EmpresasContratanteModel> CreateAsync(DTOEmpresasContratante dto);
        Task<EmpresasContratanteModel> UpdateAsync(int id, DTOEmpresasContratante dto);
        Task DeleteAsync(int id);
    }
}