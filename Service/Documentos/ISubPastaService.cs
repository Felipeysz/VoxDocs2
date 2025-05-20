using System.Collections.Generic;
using System.Threading.Tasks;
using VoxDocs.Models.Dto;

namespace VoxDocs.Services
{
    public interface ISubPastaService
    {
        Task<IEnumerable<DTOSubPasta>> GetAllAsync();
        Task<DTOSubPasta?> GetByIdAsync(int id);
        Task<DTOSubPasta> CreateAsync(DTOSubPastaCreate dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<DTOSubPasta>> GetSubChildrenAsync(string nomePastaPrincipal);
    }
}