// LogRepository.cs em VoxDocs.Data.Repositories
using Microsoft.EntityFrameworkCore;
using VoxDocs.Models;

namespace VoxDocs.Data.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly VoxDocsContext _context;

        public LogRepository(VoxDocsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<LogAtividadeModel>> GetLogsAtividadesAsync()
        {
            return await _context.LogsAtividades
                .AsNoTracking()
                .OrderByDescending(l => l.DataHora)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogAtividadeModel>> GetLogsByTipoAcaoAsync(string tipoAcao)
        {
            return await _context.LogsAtividades
                .AsNoTracking()
                .Where(l => l.TipoAcao == tipoAcao)
                .OrderByDescending(l => l.DataHora)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogAtividadeModel>> GetLogsByUsuarioIdAsync(Guid usuarioId)
        {
            return await _context.LogsAtividades
                .AsNoTracking()
                .Where(l => l.Id == usuarioId)
                .OrderByDescending(l => l.DataHora)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogAtividadeModel>> GetLogsByDocumentoIdAsync(Guid documentoId)
        {
            return await _context.LogsAtividades
                .AsNoTracking()
                .Where(l => l.Id == documentoId)
                .OrderByDescending(l => l.DataHora)
                .ToListAsync();
        }

        public async Task<IEnumerable<LogAtividadeModel>> GetRecentLogsAsync(int quantidade)
        {
            return await _context.LogsAtividades
                .AsNoTracking()
                .OrderByDescending(l => l.DataHora)
                .Take(quantidade)
                .ToListAsync();
        }

        public async Task AddLogAsync(LogAtividadeModel log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            await _context.LogsAtividades.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<LogAtividadeModel> logs)
        {
            if (logs == null)
            {
                throw new ArgumentNullException(nameof(logs));
            }

            await _context.LogsAtividades.AddRangeAsync(logs);
            await _context.SaveChangesAsync();
        }
    }
}