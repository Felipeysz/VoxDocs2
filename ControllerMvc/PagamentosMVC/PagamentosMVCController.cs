using Microsoft.AspNetCore.Mvc;
using VoxDocs.Services;
using VoxDocs.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;

namespace VoxDocs.Controllers
{
    public class PagamentoMvcController : Controller
    {
        private readonly IPagamentoService _pagamentoService;
        private readonly IPagamentoBusinessRules _businessRules;

        public PagamentoMvcController(
            IPagamentoService pagamentoService,
            IPagamentoBusinessRules businessRules)
        {
            _pagamentoService = pagamentoService;
            _businessRules = businessRules;
        }

        [HttpPost]
        public async Task<IActionResult> CriarPlano(string nomePlano, string periodicidade)
        {
            // Validações básicas
            if (string.IsNullOrWhiteSpace(nomePlano))
            {
                return Json(new { success = false, message = "Nome do plano é obrigatório." });
            }

            if (string.IsNullOrWhiteSpace(periodicidade))
            {
                return Json(new { success = false, message = "Periodicidade do plano é obrigatória." });
            }

            // Validação do plano
            var validacaoPlano = await _businessRules.ValidarPlanoExisteAsync(nomePlano, periodicidade);
            if (!validacaoPlano.IsValid)
            {
                return Json(new { success = false, message = validacaoPlano.ErrorMessage });
            }

            try
            {
                var idGerado = await _pagamentoService.CriarPlanoNome(nomePlano, periodicidade);
                
                // Redireciona diretamente para a ação PlanoPagamento
                return RedirectToAction("PlanoPagamento", new { id = idGerado });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    message = $"Erro ao criar plano: {ex.Message}" 
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> PlanoPagamento(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View("NotFoundPage");
            }

            // Validação do token
            var tokenResponse = await _pagamentoService.TokenPagoValidoAsync(new TokenRequestDto { Token = id });
            
            if (!tokenResponse.Sucesso)
            {
                return tokenResponse.CodigoStatus switch
                {
                    (int)HttpStatusCode.NotFound => View("NotFoundPage", new ErrorViewModel { 
                        Message = "Token não encontrado ou já utilizado",
                        ShowRetry = false
                    }),
                    (int)HttpStatusCode.PaymentRequired => View("ErrorTokenPlano", new ErrorViewModel { 
                        Message = "Pagamento pendente para este token",
                        ShowRetry = true,
                        RetryUrl = Url.Action("PlanoPagamento", new { id })
                    }),
                    _ => View("ErrorTokenPlano", new ErrorViewModel { 
                        Message = "Token inválido ou expirado",
                        ShowRetry = false
                    })
                };
            }

            // Obter dados do plano
            var planoResult = await _pagamentoService.ObterDadosPlanoAsync(id);
            if (!planoResult.IsValid)
            {
                return View("ErrorTokenPlano", new ErrorViewModel { 
                    Message = planoResult.ErrorMessage ?? "Erro ao obter informações do plano",
                    ShowRetry = false
                });
            }

            var planoInfo = planoResult.Result;
            var brasiliaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            var dataAgora = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brasiliaTimeZone);

            var model = new FinalizarPagamentoDto
            {
                Id = id,
                NomePlano = planoInfo.NomePlano,
                Periodicidade = planoInfo.Periodicidade,
                DataPagamento = dataAgora,
                MetodoPagamento = string.Empty,
                EmpresaContratante = new DTOEmpresasContratante
                {
                    EmpresaContratante = string.Empty,
                    Email = string.Empty
                },
                PastasPrincipais = new List<DTOPastaPrincipalCreate>(),
                SubPastas = new List<DTOSubPastaCreate>(),
                Usuarios = new List<DTORegisterUser>()
            };

            ViewBag.IdGerado = id;
            ViewBag.NomePlano = planoInfo.NomePlano;
            ViewBag.Periodicidade = planoInfo.Periodicidade;
            ViewBag.LimiteUsuarios = 10;
            ViewBag.LimiteAdmin = 2;
            ViewBag.Disponiveis = ViewBag.LimiteUsuarios;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> FinalizarPagamento([FromBody] FinalizarPagamentoDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Json(new PagamentoResponseDto
                {
                    Sucesso = false,
                    Mensagem = "Dados inválidos para finalização.",
                    DetalhesErro = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage))
                });
            }

            // Validação dos dados antes de finalizar
            var validacaoDados = await _businessRules.ValidarDadosExisteAsync(
                dto.EmpresaContratante.EmpresaContratante, 
                dto.Usuarios);

            if (!validacaoDados.IsValid)
            {
                return Json(new PagamentoResponseDto
                {
                    Sucesso = false,
                    Mensagem = validacaoDados.ErrorMessage
                });
            }

            try
            {
                var sucesso = await _pagamentoService.FinalizarPagamentoAsync(dto);
                
                if (!sucesso)
                {
                    return Json(new PagamentoResponseDto
                    {
                        Sucesso = false,
                        Mensagem = "Falha ao finalizar pagamento."
                    });
                }

                return Json(new PagamentoResponseDto
                {
                    Sucesso = true,
                    Mensagem = "Pagamento concluído com sucesso!",
                    // Adicione outros dados de retorno se necessário
                });
            }
            catch (Exception ex)
            {
                return Json(new PagamentoResponseDto
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao finalizar pagamento: {ex.Message}",
                    DetalhesErro = ex.StackTrace ?? string.Empty
                });
            }
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmarPagamento(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return View("Error", new ErrorViewModel { 
                    Message = "Token não informado.",
                    ShowRetry = false
                });
            }

            var tokenResponse = await _pagamentoService.TokenPagoValidoAsync(new TokenRequestDto { Token = token });
            
            if (!tokenResponse.Sucesso)
            {
                return tokenResponse.CodigoStatus switch
                {
                    (int)HttpStatusCode.NotFound => RedirectToAction("Login", "Home"),
                    _ => View("Error", new ErrorViewModel { 
                        Message = tokenResponse.Mensagem,
                        ShowRetry = true,
                        RetryUrl = Url.Action("ConfirmarPagamento", new { token })
                    })
                };
            }

            ViewData["Token"] = token;
            return View();
        }

        [HttpGet]
        public IActionResult Error(string message)
        {
            return View("Error", new ErrorViewModel { 
                Message = message ?? "Ocorreu um erro inesperado",
                ShowRetry = false
            });
        }
    }

    public class ErrorViewModel
    {
        public string Message { get; set; }
        public bool ShowRetry { get; set; }
        public string RetryUrl { get; set; }
    }
}