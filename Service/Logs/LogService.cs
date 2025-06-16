// LogService.cs em VoxDocs.Services
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoxDocs.Business.Rules;
using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public class LogService : ILogService
    {
        private readonly ILogBusinessRules _logBusinessRules;

        public LogService(ILogBusinessRules logBusinessRules)
        {
            _logBusinessRules = logBusinessRules ?? throw new ArgumentNullException(nameof(logBusinessRules));
        }

        public async Task<IEnumerable<LogAtividadeDto>> ObterTodosLogsAsync()
        {
            var logs = await _logBusinessRules.ObterTodosLogsAsync();
            return logs.Select(ConvertToDto);
        }

        public async Task<IEnumerable<LogAtividadeDto>> ObterLogsPorTipoAcaoAsync(string tipoAcao)
        {
            var logs = await _logBusinessRules.ObterLogsPorTipoAcaoAsync(tipoAcao);
            return logs.Select(ConvertToDto);
        }

        public async Task<IEnumerable<LogAtividadeDto>> ObterLogsPorUsuarioAsync(Guid usuarioId)
        {
            var logs = await _logBusinessRules.ObterLogsPorUsuarioAsync(usuarioId);
            return logs.Select(ConvertToDto);
        }

        public async Task<IEnumerable<LogAtividadeDto>> ObterLogsPorDocumentoAsync(Guid documentoId)
        {
            var logs = await _logBusinessRules.ObterLogsPorDocumentoAsync(documentoId);
            return logs.Select(ConvertToDto);
        }

        public async Task AdicionarLogAsync(LogAtividadeDto logDto, Guid usuarioId)
        {
            var logModel = ConvertToModel(logDto);
            await _logBusinessRules.AdicionarLogAsync(logModel, usuarioId);
        }

        public async Task<IEnumerable<LogAtividadeDto>> ObterLogsRecentesAsync(int quantidade = 10)
        {
            var logs = await _logBusinessRules.ObterLogsRecentesAsync(quantidade);
            return logs.Select(ConvertToDto);
        }

        private LogAtividadeDto ConvertToDto(LogAtividadeModel model)
        {
            return new LogAtividadeDto
            {
                Id = model.Id,
                Usuario = model.Usuario,
                Acao = model.TipoAcao,
                DataHora = model.DataHora,
                Detalhes = model.Detalhes,
                Ip = model.IpAddress
            };
        }

        private LogAtividadeModel ConvertToModel(LogAtividadeDto dto)
        {
            return new LogAtividadeModel
            {
                Id = dto.Id,
                Usuario = dto.Usuario,
                TipoAcao = dto.Acao,
                DataHora = dto.DataHora,
                Detalhes = dto.Detalhes,
                IpAddress = dto.Ip
            };
        }
    }
}