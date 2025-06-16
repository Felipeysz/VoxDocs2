using VoxDocs.BusinessRules;
using VoxDocs.Models;
using VoxDocs.DTO;

namespace VoxDocs.Services
{
    public class EmpresasContratanteService : IEmpresasContratanteService
    {
        private readonly IEmpresasContratanteBusinessRules _businessRules;

        public EmpresasContratanteService(IEmpresasContratanteBusinessRules businessRules)
        {
            _businessRules = businessRules;
        }

        public async Task<List<EmpresasContratanteModel>> GetAllAsync()
        {
            var result = await _businessRules.ValidarGetAllAsync();
            if (!result.IsValid)
            {
                throw new Exception(result.ErrorMessage);
            }
            return result.Result;
        }

        public async Task<EmpresasContratanteModel> GetByIdAsync(Guid id)
        {
            var result = await _businessRules.ValidarGetByIdAsync(id);
            if (!result.IsValid)
            {
                throw new Exception(result.ErrorMessage);
            }
            return result.Result;
        }

        public async Task<EmpresasContratanteModel> GetEmpresaByNome(string nome)
        {
            var result = await _businessRules.ValidarGetByNomeAsync(nome);
            if (!result.IsValid)
            {
                throw new Exception(result.ErrorMessage);
            }
            return result.Result;
        }

        public async Task<EmpresasContratanteModel> CreateAsync(DTOEmpresasContratante dto)
        {
            var empresa = new EmpresasContratanteModel
            {
                Id = Guid.NewGuid(),
                EmpresaContratante = dto.EmpresaContratante,
                Email = dto.Email,
                PlanoContratado = dto.PlanoContratado,
                DataContratacao = dto.DataContratacao
            };

            var result = await _businessRules.ValidarCreateAsync(empresa);
            if (!result.IsValid)
            {
                throw new Exception(result.ErrorMessage);
            }
            return result.Result;
        }

        public async Task<EmpresasContratanteModel> UpdateAsync(Guid id, DTOEmpresasContratante dto)
        {
            var empresa = new EmpresasContratanteModel
            {
                Id = id,
                EmpresaContratante = dto.EmpresaContratante,
                Email = dto.Email,
                PlanoContratado = dto.PlanoContratado,
                DataContratacao = dto.DataContratacao
            };

            var result = await _businessRules.ValidarUpdateAsync(empresa);
            if (!result.IsValid)
            {
                throw new Exception(result.ErrorMessage);
            }
            return result.Result;
        }

        public async Task DeleteAsync(Guid id)
        {
            var result = await _businessRules.ValidarDeleteAsync(id);
            if (!result.IsValid)
            {
                throw new Exception(result.ErrorMessage);
            }
        }
    }
}