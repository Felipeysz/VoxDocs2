// ILogService.cs em VoxDocs.Services
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoxDocs.DTO;

namespace VoxDocs.Services
{
    public interface ILogService
    {
        Task<IEnumerable<LogAtividadeDto>> ObterTodosLogsAsync();
        Task<IEnumerable<LogAtividadeDto>> ObterLogsPorTipoAcaoAsync(string tipoAcao);
        Task<IEnumerable<LogAtividadeDto>> ObterLogsPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<LogAtividadeDto>> ObterLogsPorDocumentoAsync(Guid documentoId);
        Task AdicionarLogAsync(LogAtividadeDto log, Guid usuarioId);
        Task<IEnumerable<LogAtividadeDto>> ObterLogsRecentesAsync(int quantidade = 10);
    }
}