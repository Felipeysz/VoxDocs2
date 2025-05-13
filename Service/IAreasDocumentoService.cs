using VoxDocs.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoxDocs.Services
{
    public interface IAreasDocumentoService
    {
        Task<IEnumerable<DTOAreasDocumentos>> GetAllAsync();
        Task<DTOAreasDocumentos?> GetByIdAsync(int id);
        Task<DTOAreasDocumentos> CreateAsync(DTOAreasDocumentos dto);
        Task<DTOAreasDocumentos> UpdateAsync(DTOAreasDocumentos dto);
        Task<bool> DeleteAsync(int id);
    }
}