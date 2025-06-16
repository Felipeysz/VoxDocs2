using System.Threading.Tasks;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public interface IAdminStatisticsBusinessRules
    {
        Task<EstatisticasAdminModel> ObterEstatisticasAdminAsync();
        Task<EstatisticasPlanoModel> ObterEstatisticasPlanoAsync(string planoNome);
        Task<EstatisticasEmpresaModel> ObterEstatisticasEmpresaAsync(string empresaNome);
    }
}