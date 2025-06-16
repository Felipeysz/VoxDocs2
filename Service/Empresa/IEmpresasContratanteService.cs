using VoxDocs.DTO;
using VoxDocs.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoxDocs.Services
{
    public interface IEmpresasContratanteService
    {
        Task<List<EmpresasContratanteModel>> GetAllAsync();
        Task<EmpresasContratanteModel> GetByIdAsync(Guid id);
        Task<EmpresasContratanteModel> GetEmpresaByNome(string nome);
        Task<EmpresasContratanteModel> CreateAsync(DTOEmpresasContratante dto);
        Task<EmpresasContratanteModel> UpdateAsync(Guid id, DTOEmpresasContratante dto);
        Task DeleteAsync(Guid id);
    }
}