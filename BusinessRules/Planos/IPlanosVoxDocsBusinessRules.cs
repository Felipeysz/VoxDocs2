// BusinessRules/PlanosVoxDocsBusinessRules.cs
using VoxDocs.DTO;
using VoxDocs.Interfaces;
using VoxDocs.Models;

namespace VoxDocs.BusinessRules
{
    public interface IPlanosVoxDocsBusinessRules
    {
        Task<(PlanosVoxDocsModel? plan, string? error)> GetPlanByNameAndPeriodicidadeWithValidationAsync(string nome, string periodicidade);
        Task<(List<PlanosVoxDocsModel> plans, string? error)> GetAllPlansWithValidationAsync();
        Task<(List<PlanosVoxDocsModel> plans, string? error)> GetPlansByCategoryWithValidationAsync(string categoria);
        Task<(PlanosVoxDocsModel? plan, string? error)> GetPlanByIdWithValidationAsync(int id);
        Task<(PlanosVoxDocsModel? plan, string? error)> CreatePlanWithValidationAsync(DTOPlanosVoxDocs dto);
        Task<(PlanosVoxDocsModel? plan, string? error)> UpdatePlanWithValidationAsync(int id, DTOPlanosVoxDocs dto);
        Task<string?> DeletePlanWithValidationAsync(int id);
        Task<(PlanosVoxDocsModel? plan, string? error)> GetPlanByNameWithValidationAsync(string name);

        Task<(int? limiteAdmin, int? limiteUsuario, string? error)> ObterLimitesPlanoAsync(int planoId);
    }
}