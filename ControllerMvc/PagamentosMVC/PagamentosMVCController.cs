using Microsoft.AspNetCore.Mvc;
using VoxDocs.Services;
using VoxDocs.DTO;
using VoxDocs.Models;
using System;
using System.Threading.Tasks;
using System.Linq;
using VoxDocs.BusinessRules;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;

namespace VoxDocs.Controllers
{
    public class PagamentoMvcController : Controller
    {
        private readonly IPagamentoService _pagamentoService;
        private readonly IPagamentoBusinessRules _pagamentoBusinessRules;
        private readonly IPlanosVoxDocsBusinessRules _planoBusinessRules;
        private readonly IEmpresasContratanteService _empresaService;
        private readonly IPastaPrincipalBusinessRules _pastaPrincipalBusiness;
        private readonly ISubPastaBusinessRules _subPastaBusiness;
        private readonly IUserService _userService;
        private readonly HttpClient _httpClient;

        public PagamentoMvcController(
            IPagamentoService pagamentoService,
            IPagamentoBusinessRules pagamentoBusinessRules,
            IPlanosVoxDocsBusinessRules planoBusinessRules,
            IEmpresasContratanteService empresaService,
            IPastaPrincipalBusinessRules pastaPrincipalBusiness,
            ISubPastaBusinessRules subPastaBusiness,
            IUserService userService,
            IHttpClientFactory httpClientFactory)
        {
            _pagamentoService = pagamentoService;
            _pagamentoBusinessRules = pagamentoBusinessRules;
            _planoBusinessRules = planoBusinessRules;
            _empresaService = empresaService;
            _pastaPrincipalBusiness = pastaPrincipalBusiness;
            _subPastaBusiness = subPastaBusiness;
            _userService = userService;
            _httpClient = httpClientFactory.CreateClient("VoxDocsApi");
        }

        [HttpPost]
        public async Task<IActionResult> CriarSolicitacaoPagamento(CriarPlanoDto dto)
        {
            var token = await _pagamentoService.CriarSolicitacaoPagamentoAsync(dto);
            return string.IsNullOrEmpty(token)
                ? RedirectToAction("ErrorTokenPlano", new { message = "Falha ao gerar token de pagamento" })
                : RedirectToAction("CriarCadastroPagamento", new { token });
        }

        [HttpGet("CriarCadastroPagamento/{token}")]
        public async Task<IActionResult> CriarCadastroPagamento(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction("ErrorTokenPlano", new { 
                    message = "Token inválido ou não fornecido",
                    errorCode = "INVALID_TOKEN"
                });
            }

            try
            {
                var response = await _httpClient.GetAsync($"api/pagamentos/ValidarTokenCadastro/{token}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    
                    string tipoPlano = responseObject?.tipoPlano?.ToString() ?? "Gratuito";
                    string nomePlano = responseObject?.nomePlano?.ToString() ?? "Plano Gratuito";
                    string periodicidade = responseObject?.periodicidade?.ToString() ?? "Mensal";
                    decimal valorPlano = responseObject?.valorPlano != null ? Convert.ToDecimal(responseObject.valorPlano) : 0m;
                    
                    var viewModel = new CriarCadastroPagamentoViewModel 
                    { 
                        Token = token,
                        NomePlano = nomePlano,
                        Periodicidade = periodicidade,
                        ValorPlano = valorPlano,
                        LimiteUsuarios = tipoPlano.Equals("Gratuito", StringComparison.OrdinalIgnoreCase) ? 5 : -1,
                        LimiteAdministradores = tipoPlano.Equals("Gratuito", StringComparison.OrdinalIgnoreCase) ? 2 : -1,
                        MetodosPagamento = ObterMetodosPagamentoDisponiveis(),
                        DadosEmpresa = new DadosEmpresaViewModel(),
                        DadosUsuarioAdmin = new UsuarioAdminViewModel()
                    };
                    
                    return View(viewModel);
                }
                
                return RedirectToAction("ErrorTokenPlano", new { 
                    message = "Token inválido ou expirado",
                    errorCode = "API_VALIDATION_ERROR"
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("ErrorTokenPlano", new { 
                    message = "Erro ao validar o token",
                    errorCode = "VALIDATION_ERROR"
                });
            }
        }

        [HttpPost("CriarCadastroPagamento")]
        public async Task<IActionResult> CriarCadastroPagamento(CriarCadastroPagamentoViewModel model)
        {
            try
            {
                // Mapeia para o DTO conforme a estrutura existente
                var dto = new CriarCadastroPagamentoPlanoDto
                {
                    EmpresaContratante = model.DadosEmpresa.Nome,
                    EmailEmpresaContratante = model.DadosEmpresa.EmailEmpresaContratante,
                    MetodoPagamento = model.MetodoPagamentoSelecionado,
                    DataExpiracao = DateTime.Now.AddYears(1),
                    StatusEmpresa = "Plano Ativo",
                    nomePlano = model.NomePlano,
                    periodicidade = model.Periodicidade,
                    valorPlano = model.ValorPlano,
                    Pastas = new List<PastaConfigurationDto>(), // Inicializa lista vazia
                    AdminUsuarios = new List<UsuarioCadastroDto>
                    {
                        new UsuarioCadastroDto
                        {
                            Nome = model.DadosUsuarioAdmin.Nome,
                            Email = model.DadosUsuarioAdmin.Email,
                            Senha = model.DadosUsuarioAdmin.Senha,
                            PermissaoConta = "Admin",
                            EmpresaContratante = model.DadosEmpresa.Nome,
                            PlanoPago = model.NomePlano
                        }
                    },
                    UsuariosComum = new List<UsuarioCadastroDto>() // Inicializa lista vazia
                };

                var response = await _httpClient.PostAsJsonAsync("api/pagamentos/criar-cadastro", dto);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var resultado = JsonConvert.DeserializeObject<PagamentoResponseDto>(responseContent);
                    if (resultado?.Sucesso == true)
                    {
                        // Acessa o PagamentoId através do objeto Dados
                        var pagamentoId = (resultado.Dados as dynamic)?.PagamentoId?.ToString();
                        return RedirectToAction("CadastroSucesso", new { id = pagamentoId });
                    }
                    
                    ModelState.AddModelError(string.Empty, resultado?.Mensagem ?? "Erro desconhecido");
                    model.MetodosPagamento = ObterMetodosPagamentoDisponiveis();
                    return View(model);
                }

                var errorResult = JsonConvert.DeserializeObject<PagamentoResponseDto>(responseContent);
                ModelState.AddModelError(string.Empty, errorResult?.Mensagem ?? "Erro ao processar pagamento");
                model.MetodosPagamento = ObterMetodosPagamentoDisponiveis();
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Erro interno: {ex.Message}");
                model.MetodosPagamento = ObterMetodosPagamentoDisponiveis();
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ErrorTokenPlano(string message)
        {
            return View(new ErrorViewModel { 
                Message = message ?? "Ocorreu um erro inesperado",
                ShowRetry = false
            });
        }

        private List<MetodoPagamentoViewModel> ObterMetodosPagamentoDisponiveis()
        {
            return new List<MetodoPagamentoViewModel>
            {
                new MetodoPagamentoViewModel { Valor = "PIX", Texto = "PIX" },
                new MetodoPagamentoViewModel { Valor = "CartaoCredito", Texto = "Cartão de Crédito" },
            };
        }
    }
}