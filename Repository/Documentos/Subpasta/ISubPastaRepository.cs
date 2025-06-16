using VoxDocs.Models;

namespace VoxDocs.Data.Repositories
{
    public interface ISubPastaRepository
    {
        Task<IEnumerable<SubPastaModel>> GetAllAsync();
        Task<IEnumerable<SubPastaModel>> GetByEmpresaAsync(string empresa);
        Task<SubPastaModel?> GetByNameSubPastaAsync(string nomeSubPasta);
        Task<SubPastaModel?> GetByIdAsync(Guid id);
        Task<SubPastaModel> CreateAsync(SubPastaModel subPasta);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<SubPastaModel>> GetSubChildrenAsync(string nomePastaPrincipal);
        Task<SubPastaModel?> GetByNameAndEmpresaAsync(string nomeSubPasta, string empresaContratante);
    }
}