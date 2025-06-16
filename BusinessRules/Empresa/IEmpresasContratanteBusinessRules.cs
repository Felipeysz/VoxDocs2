using VoxDocs.Models;

namespace VoxDocs.BusinessRules
{
    public interface IEmpresasContratanteBusinessRules
    {
        Task<ValidationResult<List<EmpresasContratanteModel>>> ValidarGetAllAsync();
        Task<ValidationResult<EmpresasContratanteModel>> ValidarGetByIdAsync(Guid id);
        Task<ValidationResult<EmpresasContratanteModel>> ValidarGetByNomeAsync(string nome);
        Task<ValidationResult<EmpresasContratanteModel>> ValidarCreateAsync(EmpresasContratanteModel empresa);
        Task<ValidationResult<EmpresasContratanteModel>> ValidarUpdateAsync(EmpresasContratanteModel empresa);
        Task<ValidationResult> ValidarDeleteAsync(Guid id);
    }
}