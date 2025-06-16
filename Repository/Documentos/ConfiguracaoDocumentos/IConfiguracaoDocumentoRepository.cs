// IConfiguracaoDocumentoRepository.cs
using System.Threading.Tasks;
using VoxDocs.Models;

namespace VoxDocs.Data.Repositories
{
    public interface IConfiguracaoDocumentoRepository
    {
        Task<ConfiguracaoDocumentosModel> GetFirstAsync();
        Task UpdateAsync(ConfiguracaoDocumentosModel config);
    }
}