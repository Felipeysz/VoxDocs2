using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using VoxDocs.Data;
using VoxDocs.DTO;
using VoxDocs.Services;

namespace VoxDocs.Controllers.Api
{
    [ApiController]
    [Route("api/pagamentos")]
    public class PagamentosController : ControllerBase
    {
        private readonly IPagamentoService _pagamentoService;
        private readonly IPagamentoRepository _pagamentoRepository;

        public PagamentosController(
            IPagamentoService pagamentoService,
            IPagamentoRepository pagamentoRepository)
        {
            _pagamentoService = pagamentoService;
            _pagamentoRepository = pagamentoRepository;
        }

        [HttpPost("solicitar")]
        public async Task<ActionResult<PagamentoResponseDto>> CriarSolicitacao([FromBody] CriarPlanoDto dto)
        {
            var pagamentoId = await _pagamentoService.CriarSolicitacaoPagamentoAsync(dto);
            
            return Ok(PagamentoResponseDto.Ok(
                dados: new { 
                    PagamentoId = pagamentoId,
                    Status = "Solicitação criada",
                    DataProcessamento = DateTime.Now
                },
                mensagem: "Solicitação de pagamento criada com sucesso"
            ));
        }

        [HttpPost("criar-cadastro")]
        public async Task<ActionResult<PagamentoResponseDto>> CriarCadastroPagamento(
            [FromBody] CriarCadastroPagamentoPlanoDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(PagamentoResponseDto.Falha(
                        mensagem: "Dados de pagamento inválidos",
                        StatusCode: HttpStatusCode.BadRequest
                    ));
                }

                if (string.IsNullOrEmpty(dto.MetodoPagamento))
                {
                    return BadRequest(PagamentoResponseDto.Falha(
                        mensagem: "Método de pagamento não informado",
                        StatusCode: HttpStatusCode.BadRequest
                    ));
                }

                // Garanta que o serviço retorne PagamentoResponseDto, não string
                var resultadoJson = await _pagamentoService.CriarCadastroPagamentoAsync(dto);
                var resultado = JsonConvert.DeserializeObject<PagamentoResponseDto>(resultadoJson);

                if (!resultado.Sucesso)
                {
                    return StatusCode(
                        (int)resultado.StatusCode,
                        resultado
                    );
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    (int)HttpStatusCode.InternalServerError,
                    PagamentoResponseDto.Falha(
                        erro: ex.Message,
                        status: HttpStatusCode.InternalServerError,
                        mensagem: "Erro ao processar cadastro com pagamento",
                        detalhes: ex.StackTrace
                    )
                );
            }
        }

        [HttpGet("ValidarTokenCadastro/{id}")]
        public async Task<ActionResult<PagamentoResponseDto>> ValidarTokenCadastro(Guid id)
        {
            var pagamento = await _pagamentoRepository.GetPagamentoByIdAsync(id);
            
            if (pagamento == null)
            {
                return NotFound(PagamentoResponseDto.Falha(
                    mensagem: "Pagamento não encontrado",
                    StatusCode: HttpStatusCode.NotFound
                ));
            }

            return Ok(PagamentoResponseDto.Ok(
                dados: new { 
                    Valido = true,
                    Id = pagamento.Id,
                    Status = pagamento.StatusEmpresa,
                    DataExpiracao = pagamento.DataExpiracao
                },
                mensagem: "Pagamento válido"
            ));
        }
    }
}