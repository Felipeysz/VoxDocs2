using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoxDocs.DTO;
using VoxDocs.Services;

namespace VoxDocs.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogAtividadeDto>>> ObterTodosLogs()
        {
            try
            {
                var logs = await _logService.ObterTodosLogsAsync();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao obter logs: {ex.Message}" });
            }
        }

        [HttpGet("por-acao/{tipoAcao}")]
        public async Task<ActionResult<IEnumerable<LogAtividadeDto>>> ObterLogsPorAcao(string tipoAcao)
        {
            try
            {
                var logs = await _logService.ObterLogsPorTipoAcaoAsync(tipoAcao);
                return Ok(logs);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao obter logs por ação: {ex.Message}" });
            }
        }

        [HttpGet("por-usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<LogAtividadeDto>>> ObterLogsPorUsuario(Guid usuarioId)
        {
            try
            {
                var logs = await _logService.ObterLogsPorUsuarioAsync(usuarioId);
                return Ok(logs);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao obter logs por usuário: {ex.Message}" });
            }
        }

        [HttpGet("por-documento/{documentoId}")]
        public async Task<ActionResult<IEnumerable<LogAtividadeDto>>> ObterLogsPorDocumento(Guid documentoId)
        {
            try
            {
                var logs = await _logService.ObterLogsPorDocumentoAsync(documentoId);
                return Ok(logs);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao obter logs por documento: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> AdicionarLog([FromBody] LogAtividadeDto logDto, [FromQuery] Guid usuarioId)
        {
            try
            {
                await _logService.AdicionarLogAsync(logDto, usuarioId);
                return Ok(new { message = "Log adicionado com sucesso" });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao adicionar log: {ex.Message}" });
            }
        }

        [HttpGet("recentes")]
        public async Task<ActionResult<IEnumerable<LogAtividadeDto>>> ObterLogsRecentes([FromQuery] int quantidade = 10)
        {
            try
            {
                var logs = await _logService.ObterLogsRecentesAsync(quantidade);
                return Ok(logs);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao obter logs recentes: {ex.Message}" });
            }
        }
    }
}