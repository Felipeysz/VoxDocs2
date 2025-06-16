// LogBusinessRules.cs em VoxDocs.Business.Rules
using VoxDocs.Data.Repositories;
using VoxDocs.Models;
using VoxDocs.Repository;

namespace VoxDocs.Business.Rules
{
    public class LogBusinessRules : ILogBusinessRules
    {
        private readonly ILogRepository _logRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IDocumentoRepository _documentoRepository;

        public LogBusinessRules(
            ILogRepository logRepository,
            IUsuarioRepository usuarioRepository,
            IDocumentoRepository documentoRepository)
        {
            _logRepository = logRepository ?? throw new ArgumentNullException(nameof(logRepository));
            _usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
            _documentoRepository = documentoRepository ?? throw new ArgumentNullException(nameof(documentoRepository));
        }

        public async Task<IEnumerable<LogAtividadeModel>> ObterTodosLogsAsync()
        {
            return await _logRepository.GetLogsAtividadesAsync();
        }

        public async Task<IEnumerable<LogAtividadeModel>> ObterLogsPorTipoAcaoAsync(string tipoAcao)
        {
            if (string.IsNullOrWhiteSpace(tipoAcao))
            {
                throw new ArgumentException("Tipo de ação não pode ser vazio", nameof(tipoAcao));
            }

            var logs = await _logRepository.GetLogsAtividadesAsync();
            return logs.Where(l => l.TipoAcao.Equals(tipoAcao, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<LogAtividadeModel>> ObterLogsPorUsuarioAsync(Guid usuarioId)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorIdAsync(usuarioId);
            if (usuario == null)
            {
                throw new KeyNotFoundException($"Usuário com ID {usuarioId} não encontrado");
            }

            var logs = await _logRepository.GetLogsAtividadesAsync();
            return logs.Where(l => l.Id == usuarioId);
        }

        public async Task<IEnumerable<LogAtividadeModel>> ObterLogsPorDocumentoAsync(Guid documentoId)
        {
            var documento = await _documentoRepository.GetByIdAsync(documentoId);
            if (documento == null)
            {
                throw new KeyNotFoundException($"Documento com ID {documentoId} não encontrado");
            }

            var logs = await _logRepository.GetLogsAtividadesAsync();
            return logs.Where(l => l.Id == documentoId);
        }

        public async Task AdicionarLogAsync(LogAtividadeModel log, Guid usuarioId)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            // Valida se o usuário existe
            var usuario = await _usuarioRepository.ObterUsuarioPorIdAsync(usuarioId);
            if (usuario == null)
            {
                throw new UnauthorizedAccessException("Usuário não autorizado");
            }

            // Valida se o documento existe (se aplicável)
            if (log.DocumentoId != Guid.Empty) // Check against Guid.Empty instead of HasValue
            {
                var documento = await _documentoRepository.GetByIdAsync(log.DocumentoId); // No .Value needed
                if (documento == null)
                {
                    throw new InvalidOperationException($"Documento com ID {log.DocumentoId} não encontrado");
                }
            }

            // Define informações padrão
            log.usuarioId = usuarioId;
            log.DataHora = DateTime.UtcNow;
            log.IpAddress = log.IpAddress ?? "Desconhecido";

            await _logRepository.AddLogAsync(log);
        }

        public async Task<IEnumerable<LogAtividadeModel>> ObterLogsRecentesAsync(int quantidade = 10)
        {
            if (quantidade <= 0)
            {
                throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));
            }

            var logs = await _logRepository.GetLogsAtividadesAsync();
            return logs.Take(quantidade);
        }
    }
}