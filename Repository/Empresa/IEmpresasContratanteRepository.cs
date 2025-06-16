using VoxDocs.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoxDocs.Data.Repositories
{
    public interface IEmpresasContratanteRepository
    {
        Task<List<EmpresasContratanteModel>> GetAllAsync();
        Task<EmpresasContratanteModel> GetByIdAsync(Guid id);
        Task<EmpresasContratanteModel> GetByNomeAsync(string nome);
        Task<EmpresasContratanteModel> CreateAsync(EmpresasContratanteModel empresa);
        Task<EmpresasContratanteModel> UpdateAsync(EmpresasContratanteModel empresa);
        Task DeleteAsync(Guid id);
    }
}