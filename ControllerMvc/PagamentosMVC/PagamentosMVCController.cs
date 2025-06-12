// PagamentoMvcController.cs
using Microsoft.AspNetCore.Mvc;
using VoxDocs.Services;
using VoxDocs.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoxDocs.Controllers
{
    public class PagamentoMvcController : Controller
    {
        private readonly IPagamentoService _pagamentoService;

        public PagamentoMvcController(IPagamentoService pagamentoService)
        {
            _pagamentoService = pagamentoService;
        }

        // Cria o plano e redireciona para a URL única apenas com o token
        [HttpPost]
        public async Task<IActionResult> CriarPlano(string nomePlano, string periodicidade)
        {
            if (string.IsNullOrWhiteSpace(nomePlano))
            {
                TempData["Erro"] = "Nome do plano é obrigatório.";
                return RedirectToAction("Index", "IndexMvc");
            }
            if (string.IsNullOrWhiteSpace(periodicidade))
            {
                TempData["Erro"] = "Periodicidade do plano é obrigatória.";
                return RedirectToAction("Index", "IndexMvc");
            }

            var idGerado = await _pagamentoService.CriarPlanoNome(nomePlano, periodicidade);
            return RedirectToAction("PlanoPagamento", new { id = idGerado });
        }

        // Exibe a página de finalização de pagamento com dados do plano (recuperados via serviço)
        [HttpGet]
        public async Task<IActionResult> PlanoPagamento(string id)
        {
            var tokenResponse = await _pagamentoService.TokenPagoValidoAsync(new TokenRequestDto { Token = id });

            if (tokenResponse.Mensagem != "O Token Não Foi pago")
            {
                // Redireciona para a view de erro personalizada
                return RedirectToAction("ErrorTokenPlano", new { mensagem = tokenResponse.Mensagem });
            }

            var planoInfo = await _pagamentoService.ObterDadosPlanoAsync(id);

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


        // Recebe os dados do wizard e finaliza pagamento
        [HttpPost]
        public async Task<IActionResult> FinalizarPagamento(FinalizarPagamentoDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Erro"] = "Dados inválidos para finalização.";
                return RedirectToAction("PlanoPagamento", new { id = dto.Id });
            }

            try
            {
                var sucesso = await _pagamentoService.FinalizarPagamentoAsync(dto);
                if (!sucesso)
                {
                    TempData["Erro"] = "Falha ao finalizar pagamento.";
                    return RedirectToAction("PlanoPagamento", new { id = dto.Id });
                }

                TempData["Sucesso"] = "Pagamento concluído com sucesso!";
                return RedirectToAction("Index", "Home");
            }
            catch (InvalidOperationException ex)
            {
                // Aqui caem erros do ValidarDadosExisteAsync
                TempData["Erro"] = ex.Message;
                return RedirectToAction("PlanoPagamento", new { id = dto.Id });
            }
        }


        // Confirmação de pagamento (GET)
        [HttpGet]
        public async Task<IActionResult> ConfirmarPagamento(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["Erro"] = "Token não informado.";
                return RedirectToAction("Index", "Home");
            }

            var tokenResponse = await _pagamentoService
                .TokenPagoValidoAsync(new TokenRequestDto { Token = token });

            if (tokenResponse.Mensagem == "Token não existe")
            {
                TempData["Erro"] = tokenResponse.Mensagem;
                return RedirectToAction("Login", "Home");
            }

            // Se chegou aqui, o token existe — renderiza a view
            ViewData["Token"] = token;
            return View();
        }

        


        [HttpGet]
        public IActionResult ErrorTokenPlano(string mensagem)
        {
            ViewBag.MensagemErro = mensagem;
            return View();
        }
    }
}
