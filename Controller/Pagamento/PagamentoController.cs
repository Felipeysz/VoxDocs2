using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoxDocs.Data;
using VoxDocs.DTO;
using VoxDocs.Models;
using VoxDocs.Services;

namespace VoxDocs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentosController : ControllerBase
    {
        private readonly IPagamentoService _service;
        private readonly VoxDocsContext _context;

        public PagamentosController(IPagamentoService service, VoxDocsContext context)
        {
            _service = service;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PagamentoConcluido>>> GetAll()
        {
            var list = await _context.PagamentosConcluidos.ToListAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PagamentoConcluido>> GetById(string id)
        {
            var pagamento = await _context.PagamentosConcluidos.FindAsync(id);
            if (pagamento == null) return NotFound();
            return Ok(pagamento);
        }

        [HttpPost("CriarPlanoNome")]
        public async Task<IActionResult> CriarPlanoNome([FromBody] CriarPlanoNomeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NomePlano))
                return BadRequest("Informe NomePlano.");
            if (string.IsNullOrWhiteSpace(dto.Periodicidade))
                return BadRequest("Informe Periodicidade.");

            // chama serviço que gera Id, faz hash do nome e salva
            var id = await _service.CriarPlanoNome(dto.NomePlano, dto.Periodicidade);

            // devolve 201 Created com o novo id
            return CreatedAtAction(
                nameof(GetById),
                new { id },
                new { id }
            );
        }

        [HttpPost("finalizar")]
        public async Task<IActionResult> FinalizarPagamento([FromBody] FinalizarPagamentoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sucesso = await _service.FinalizarPagamentoAsync(dto);
            if (!sucesso)
                return NotFound("Pagamento não encontrado ou inválido.");

            return Ok(new { message = "Pagamento finalizado com sucesso." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePagamento(string id, [FromBody] PagamentoConcluido dto)
        {
            var existente = await _context.PagamentosConcluidos.FindAsync(id);
            if (existente == null) return NotFound();

            // Atualiza apenas os campos permitidos
            if (!string.IsNullOrWhiteSpace(dto.MetodoPagamento))
                existente.MetodoPagamento = dto.MetodoPagamento;

            if (dto.DataExpiracao.HasValue)
                existente.DataExpiracao = dto.DataExpiracao;

            if (!string.IsNullOrWhiteSpace(dto.StatusEmpresa))
                existente.StatusEmpresa = dto.StatusEmpresa;

            existente.IsPagamentoConcluido = dto.IsPagamentoConcluido;

            _context.PagamentosConcluidos.Update(existente);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /api/pagamentos/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePagamento(string id)
        {
            var pagamento = await _context.PagamentosConcluidos.FindAsync(id);
            if (pagamento == null) return NotFound();

            _context.PagamentosConcluidos.Remove(pagamento);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("validar-token")]
        public async Task<IActionResult> ValidarToken([FromBody] TokenRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest("Token não informado.");

            var response = await _service.TokenPagoValidoAsync(dto);

            if (!response.Sucesso)
                return NotFound(new { message = response.Mensagem });

            return Ok(new { message = response.Mensagem });
        }

        [HttpGet("obter-dados-plano")]
        public async Task<IActionResult> ObterDadosPlano([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("Token não informado.");

            try
            {
                var planoInfo = await _service.ObterDadosPlanoAsync(token);

                if (planoInfo == null)
                    return NotFound(new { message = "Dados do plano não encontrados." });

                return Ok(planoInfo);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

    }
}
