// Interfaces/IPlanosVoxDocsService.cs
using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Interfaces
{
    public interface IPlanosVoxDocsService
    {
        Task<PlanosVoxDocsModel> GetPlanByNameAndPeriodicidadeAsync(string nome, string periodicidade);
        Task<List<PlanosVoxDocsModel>> GetAllPlansAsync();
        Task<List<PlanosVoxDocsModel>> GetPlansByCategoryAsync(string categoria);
        Task<PlanosVoxDocsModel> GetPlanByIdAsync(int id);
        Task<PlanosVoxDocsModel> CreatePlanAsync(DTOPlanosVoxDocs dto);
        Task<PlanosVoxDocsModel> UpdatePlanAsync(int id, DTOPlanosVoxDocs dto);
        Task DeletePlanAsync(int id);
        Task<PlanosVoxDocsModel> GetPlanByNameAsync(string name);
    }
}