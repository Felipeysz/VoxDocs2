// Services/PlanosVoxDocsService.cs
using VoxDocs.BusinessRules;
using VoxDocs.DTO;
using VoxDocs.Interfaces;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public class PlanosVoxDocsService : IPlanosVoxDocsService
    {
        private readonly IPlanosVoxDocsBusinessRules _businessRules;

        public PlanosVoxDocsService(IPlanosVoxDocsBusinessRules businessRules)
        {
            _businessRules = businessRules;
        }

        public async Task<PlanosVoxDocsModel> GetPlanByNameAndPeriodicidadeAsync(string nome, string periodicidade)
        {
            var result = await _businessRules.GetPlanByNameAndPeriodicidadeWithValidationAsync(nome, periodicidade);
            if (result.error != null) throw new Exception(result.error);
            return result.plan!;
        }

        public async Task<List<PlanosVoxDocsModel>> GetAllPlansAsync()
        {
            var result = await _businessRules.GetAllPlansWithValidationAsync();
            if (result.error != null) throw new Exception(result.error);
            return result.plans;
        }

        public async Task<List<PlanosVoxDocsModel>> GetPlansByCategoryAsync(string categoria)
        {
            var result = await _businessRules.GetPlansByCategoryWithValidationAsync(categoria);
            if (result.error != null) throw new Exception(result.error);
            return result.plans;
        }

        public async Task<PlanosVoxDocsModel> GetPlanByIdAsync(int id)
        {
            var result = await _businessRules.GetPlanByIdWithValidationAsync(id);
            if (result.error != null) throw new Exception(result.error);
            return result.plan!;
        }

        public async Task<PlanosVoxDocsModel> CreatePlanAsync(DTOPlanosVoxDocs dto)
        {
            var result = await _businessRules.CreatePlanWithValidationAsync(dto);
            if (result.error != null) throw new Exception(result.error);
            return result.plan!;
        }

        public async Task<PlanosVoxDocsModel> UpdatePlanAsync(int id, DTOPlanosVoxDocs dto)
        {
            var result = await _businessRules.UpdatePlanWithValidationAsync(id, dto);
            if (result.error != null) throw new Exception(result.error);
            return result.plan!;
        }

        public async Task DeletePlanAsync(int id)
        {
            var error = await _businessRules.DeletePlanWithValidationAsync(id);
            if (error != null) throw new Exception(error);
        }

        public async Task<PlanosVoxDocsModel> GetPlanByNameAsync(string name)
        {
            var result = await _businessRules.GetPlanByNameWithValidationAsync(name);
            if (result.error != null) throw new Exception(result.error);
            return result.plan!;
        }
    }
}