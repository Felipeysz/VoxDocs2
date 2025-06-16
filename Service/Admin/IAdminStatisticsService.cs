using VoxDocs.DTO;

namespace VoxDocs.Services
{
    public interface IAdminStatisticsService
    {
        Task<DTOEstatisticasAdmin> GetAdminStatisticsAsync();
        Task<DTOEstatisticasPlano> GetPlanStatisticsAsync(string planoNome);
        Task<DTOEstatisticasEmpresa> GetCompanyStatisticsAsync(string empresaNome);
    }
}
