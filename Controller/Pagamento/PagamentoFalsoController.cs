using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VoxDocs.DTO;
using VoxDocs.Services;

namespace VoxDocs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentoFalsoController : ControllerBase
    {
        private readonly IPagamentoFalsoService _svc;

        public PagamentoFalsoController(IPagamentoFalsoService svc)
            => _svc = svc;

        [HttpPost("cartao")]
        public async Task<IActionResult> Cartao([FromBody] DTOCartaoPagamentoFalso dto)
        {
            try
            {
                var msg = await _svc.ProcessarPagamentoCartaoFalsoAsync(dto);
                return Ok(new { Mensagem = msg });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }

        [HttpPost("pix/gerar")]
        public async Task<IActionResult> GerarPix([FromBody] DTOPixGerar dto)
        {
            try
            {
                var (pagamentoPixId, qrCodeUrl) = await _svc.GerarPixAsync(dto);
                return Ok(new
                {
                    pagamentoPixId,
                    qrCode     = qrCodeUrl,
                    token      = qrCodeUrl.Split("Token=")[^1]
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }
        [HttpGet("pix/status/{id}")]
        public async Task<IActionResult> StatusPix(int id)
        {
            try
            {
                bool confirmado = await _svc.PixStatusAsync(id);
                return Ok(new { pagamentoPixId = id, confirmado });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("pix/token/{token}")]
        public async Task<IActionResult> ConfirmarPixToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { message = "Token não informado." });

            var existe = await _svc.TokenPixExisteAsync(token);
            if (!existe)
                return NotFound(new { message = "Token não encontrado ou inválido." });

            return Ok(new { message = "Token válido." });
        }

        [HttpPost("pix/confirmar")]
        public async Task<IActionResult> ConfirmarPix([FromBody] DTOPixConfirmar dto)
        {
            try
            {
                var msg = await _svc.ConfirmarPixAsync(dto);
                return Ok(new { Mensagem = msg });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Erro = ex.Message });
            }
        }
    }
}
