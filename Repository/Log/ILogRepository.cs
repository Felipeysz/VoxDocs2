// ILogRepository.cs em VoxDocs.Data.Repositories
using System.Collections.Generic;
using System.Threading.Tasks;
using VoxDocs.Models;

namespace VoxDocs.Data.Repositories
{
    public interface ILogRepository
    {
        Task<IEnumerable<LogAtividadeModel>> GetLogsAtividadesAsync();
        Task<IEnumerable<LogAtividadeModel>> GetLogsByTipoAcaoAsync(string tipoAcao);
        Task<IEnumerable<LogAtividadeModel>> GetLogsByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<LogAtividadeModel>> GetLogsByDocumentoIdAsync(Guid documentoId);
        Task<IEnumerable<LogAtividadeModel>> GetRecentLogsAsync(int quantidade);
        Task AddLogAsync(LogAtividadeModel log);
        Task AddRangeAsync(IEnumerable<LogAtividadeModel> logs);
    }
}