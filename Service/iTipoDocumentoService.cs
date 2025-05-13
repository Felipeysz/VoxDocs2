using VoxDocs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoxDocs.Services
{
    public interface ITipoDocumentoService
    {
        Task<IEnumerable<TipoDocumentoModel>> GetAllAsync();
        Task<TipoDocumentoModel?> GetByIdAsync(int id);
        Task<TipoDocumentoModel> CreateAsync(TipoDocumentoModel tipo);
        Task<TipoDocumentoModel> UpdateAsync(TipoDocumentoModel tipo);
        Task<bool> DeleteAsync(int id);
    }
}