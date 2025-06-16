using VoxDocs.Models;

namespace VoxDocs.Interfaces
{
    public interface IPlanosVoxDocsRepository
    {
        Task<PlanosVoxDocsModel> GetPlanByNameAndPeriodicidadeAsync(string nome, string periodicidade);
        Task<List<PlanosVoxDocsModel>> GetAllPlansAsync();
        Task<List<PlanosVoxDocsModel>> GetPlansByCategoryAsync(string categoria);
        Task<PlanosVoxDocsModel> GetPlanByIdAsync(int id);
        Task<PlanosVoxDocsModel> CreatePlanAsync(PlanosVoxDocsModel plan);
        Task<PlanosVoxDocsModel> UpdatePlanAsync(PlanosVoxDocsModel plan);
        Task DeletePlanAsync(int id);
        Task<PlanosVoxDocsModel> GetPlanByNameAsync(string name);
    }
}