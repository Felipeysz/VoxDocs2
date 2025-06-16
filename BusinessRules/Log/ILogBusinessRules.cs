// ILogBusinessRules.cs em VoxDocs.Business.Rules
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoxDocs.Models;

namespace VoxDocs.Business.Rules
{
    public interface ILogBusinessRules
    {
        Task<IEnumerable<LogAtividadeModel>> ObterTodosLogsAsync();
        Task<IEnumerable<LogAtividadeModel>> ObterLogsPorTipoAcaoAsync(string tipoAcao);
        Task<IEnumerable<LogAtividadeModel>> ObterLogsPorUsuarioAsync(Guid usuarioId);
        Task<IEnumerable<LogAtividadeModel>> ObterLogsPorDocumentoAsync(Guid documentoId);
        Task AdicionarLogAsync(LogAtividadeModel log, Guid usuarioId);
        Task<IEnumerable<LogAtividadeModel>> ObterLogsRecentesAsync(int quantidade = 10);
    }
}