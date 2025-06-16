using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using VoxDocs.DTO;
using VoxDocs.Interfaces;
using VoxDocs.Models.ViewModels;
using VoxDocs.Services;

namespace VoxDocs.ControllerMvc
{
    [Authorize(Roles = "admin")]
    public class AdminMvcController : Controller
    {
        private readonly ILogger<AdminMvcController> _logger;
        private readonly IUserService _userService;
        private readonly IPlanosVoxDocsService _planosService;
        private readonly IDocumentosPastasService _documentoService;
        private readonly IPagamentoBusinessRules _pagamentoBusinessRules;
        private readonly IEmpresasContratanteService _empresaService;
        private readonly IAdminStatisticsService _statsService;
        private readonly ILogService _logService;
        private readonly IConfiguracaoDocumentoService _configuracaoDocumentoService;

        public AdminMvcController(
            ILogger<AdminMvcController> logger,
            IUserService userService,
            IPlanosVoxDocsService planosService,
            IDocumentosPastasService documentoService,
            IPagamentoBusinessRules pagamentoBusinessRules,
            IEmpresasContratanteService empresaService,
            IAdminStatisticsService statsService,
            ILogService logService,
            IConfiguracaoDocumentoService configuracaoDocumentoService)
        {
            _logger = logger;
            _userService = userService;
            _planosService = planosService;
            _documentoService = documentoService;
            _pagamentoBusinessRules = pagamentoBusinessRules;
            _empresaService = empresaService;
            _statsService = statsService;
            _logService = logService;
            _configuracaoDocumentoService = configuracaoDocumentoService;
        }

                [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var userModel = await _userService.GetUserByUsernameAsync(User.Identity.Name);
                var stats = await _statsService.GetAdminStatisticsAsync();
                var plano = await _planosService.GetPlanByNameAsync(userModel.Plano);
                var storageInfo = await _userService.GetUserStorageInfoAsync(userModel.Id);
                var documentos = await _documentoService.GetAllDocumentosAsync();
                var totalDocumentos = documentos.Count();

                // Obter informações de pagamento usando o Business Rules
                var pagamentoInfo = await _pagamentoBusinessRules.ValidarPagamentoExisteAsync(userModel.Id);

                var vm = new AdminDashboardViewModel
                {
                    // Informações do Plano
                    Plano = plano?.Nome ?? "Não definido",
                    ArmazenamentoTotal = (plano?.ArmazenamentoDisponivel ?? 0).ToString("N0") + " GB",
                    ArmazenamentoUsado = storageInfo.UsoArmazenamento.ToString("N0") + " GB",
                    PercentualUsoArmazenamento = plano != null ?
                        Math.Round((storageInfo.UsoArmazenamento / (double)plano.ArmazenamentoDisponivel) * 100, 2) : 0,

                    // Informações de Usuários
                    UsuariosAtuais = stats.TotalUsuarios,
                    UsuariosPermitidos = plano?.LimiteUsuario ?? 0,
                    AdministradoresAtuais = stats.TotalAdministradores,
                    AdministradoresPermitidos = plano?.LimiteAdmin ?? 0,

                    // Documentos
                    DocumentosEnviados = totalDocumentos,

                    // Pagamento
                    PagamentoInfo = new PagamentoResponseDto
                    {
                        Sucesso = pagamentoInfo != null,
                        Mensagem = pagamentoInfo?.StatusEmpresa ?? "N/A",
                        Dados = new
                        {
                            Metodo = pagamentoInfo?.MetodoPagamento ?? "N/A",
                            DataConfirmacao = pagamentoInfo?.DataPagamento ?? DateTime.MinValue,
                            Status = pagamentoInfo?.StatusEmpresa ?? "N/A"
                        }
                    },

                    // Estatísticas
                    TotalEmpresas = stats.TotalEmpresas,
                    TotalPlanosAtivos = stats.TotalPlanosAtivos,

                    // Listas
                    UsuariosRecentes = stats.UsuariosRecentes,
                    Empresas = (await _empresaService.GetAllAsync())
                        .Select(e => new DTOEmpresasContratante
                        {
                            EmpresaContratante = e.EmpresaContratante,
                            Email = e.Email,
                            PlanoContratado = e.PlanoContratado,
                            DataContratacao = e.DataContratacao
                        }).ToList(),

                    // Timestamp
                    UltimaAtualizacao = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar dashboard administrativo");
                TempData["ErrorMessage"] = "Ocorreu um erro ao carregar o dashboard. Por favor, tente novamente.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Usuarios()
        {
            try
            {
                var vm = new AdminUsuariosViewModel
                {
                    Usuarios = await _userService.GetAllUsersAsync(),
                    PlanosDisponiveis = (await _planosService.GetAllPlansAsync()).Select(p => p.Nome),
                    EmpresasDisponiveis = (await _empresaService.GetAllAsync()).Select(e => e.EmpresaContratante)
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de usuários");
                TempData["ErrorMessage"] = "Ocorreu um erro ao carregar a lista de usuários.";
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditarUsuario(Guid id)
        {
            try
            {
                var usuario = await _userService.GetUserByIdAsync(id);
                if (usuario == null)
                {
                    TempData["ErrorMessage"] = "Usuário não encontrado.";
                    return RedirectToAction("Usuarios");
                }

                var vm = new AdminUsuariosViewModel
                {
                    UsuarioSelecionado = usuario,
                    PlanosDisponiveis = (await _planosService.GetAllPlansAsync()).Select(p => p.Nome),
                    EmpresasDisponiveis = (await _empresaService.GetAllAsync()).Select(e => e.EmpresaContratante)
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao carregar edição do usuário {id}");
                TempData["ErrorMessage"] = "Ocorreu um erro ao carregar os dados do usuário.";
                return RedirectToAction("Usuarios");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(AdminUsuariosViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.PlanosDisponiveis = (await _planosService.GetAllPlansAsync()).Select(p => p.Nome);
                model.EmpresasDisponiveis = (await _empresaService.GetAllAsync()).Select(e => e.EmpresaContratante);
                return View(model);
            }

            try
            {
                var updateDto = new DTOAtualizarUsuario
                {
                    IdUser = model.UsuarioSelecionado.Id,
                    Usuario = model.UsuarioSelecionado.Usuario,
                    Email = model.UsuarioSelecionado.Email,
                    PermissaoConta = model.UsuarioSelecionado.PermissaoConta,
                    EmpresaContratante = model.UsuarioSelecionado.EmpresaContratante,
                    PlanoPago = model.UsuarioSelecionado.Plano,
                    Ativo = model.UsuarioSelecionado.Ativo
                };

                await _userService.UpdateUserAsync(updateDto);
                TempData["SuccessMessage"] = "Usuário atualizado com sucesso!";
                return RedirectToAction("Usuarios");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao atualizar usuário {model.UsuarioSelecionado?.Id}");
                TempData["ErrorMessage"] = "Ocorreu um erro ao atualizar o usuário.";

                model.PlanosDisponiveis = (await _planosService.GetAllPlansAsync()).Select(p => p.Nome);
                model.EmpresasDisponiveis = (await _empresaService.GetAllAsync()).Select(e => e.EmpresaContratante);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Empresas()
        {
            try
            {
                var empresasModel = await _empresaService.GetAllAsync();

                var vm = new AdminEmpresasViewModel
                {
                    Empresas = empresasModel.Select(e => new DTOEmpresasContratante
                    {
                        EmpresaContratante = e.EmpresaContratante,
                        Email = e.Email,
                        PlanoContratado = e.PlanoContratado,
                        DataContratacao = e.DataContratacao
                    }).ToList()
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de empresas");
                TempData["ErrorMessage"] = "Ocorreu um erro ao carregar a lista de empresas.";
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Planos()
        {
            try
            {
                var planosModel = await _planosService.GetAllPlansAsync();

                var vm = new AdminPlanosViewModel
                {
                    Planos = planosModel.Select(p => new DTOPlanosVoxDocs
                    {
                        Nome = p.Nome,
                        Descricao = p.Descriçao,
                        Preco = p.Preco,
                        Duracao = p.Duracao,
                        Periodicidade = p.Periodicidade,
                        ArmazenamentoDisponivel = p.ArmazenamentoDisponivel,
                        LimiteAdmin = p.LimiteAdmin,
                        LimiteUsuario = p.LimiteUsuario
                    }).ToList()
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar lista de planos");
                TempData["ErrorMessage"] = "Ocorreu um erro ao carregar a lista de planos.";
                return RedirectToAction("Dashboard");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(Guid userId, bool ativo)
        {
            try
            {
                await _userService.ToggleUserStatusAsync(userId, ativo);
                TempData["SuccessMessage"] = $"Status do usuário alterado para {(ativo ? "ativo" : "inativo")}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao alterar status do usuário {userId}");
                TempData["ErrorMessage"] = "Ocorreu um erro ao alterar o status do usuário.";
            }

            return RedirectToAction("Usuarios");
        }
        
        [HttpGet]
        public async Task<IActionResult> AtividadesDetalhes()
        {
            try
            {
                var logs = await _logService.ObterTodosLogsAsync();
                return View(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar logs de atividades");
                TempData["ErrorMessage"] = "Ocorreu um erro ao carregar os logs de atividades.";
                return RedirectToAction("Dashboard");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfiguracaoDocumentos()
        {
            try
            {
                var configResult = await _configuracaoDocumentoService.GetConfiguracoesAsync();
                if (!configResult.Success)
                {
                    throw new Exception(configResult.ErrorMessage);
                }

                var vm = new ConfiguracaoDocumentosViewModel
                {
                    PermitirPDF = configResult.Data.PermitirPDF,
                    PermitirWord = configResult.Data.PermitirWord,
                    PermitirExcel = configResult.Data.PermitirExcel,
                    PermitirImagens = configResult.Data.PermitirImagens,
                    TamanhoMaximoMB = configResult.Data.TamanhoMaximoMB,
                    DiasArmazenamentoTemporario = configResult.Data.DiasArmazenamentoTemporario
                };
                
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar configurações de documentos");
                TempData["ErrorMessage"] = "Ocorreu um erro ao carregar as configurações de documentos.";
                return RedirectToAction("Dashboard");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarConfiguracoesDocumentos(ConfiguracaoDocumentosViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ConfiguracaoDocumentos", model);
            }

            try
            {
                var dto = new DTOConfiguracaoDocumentos
                {
                    PermitirPDF = model.PermitirPDF,
                    PermitirWord = model.PermitirWord,
                    PermitirExcel = model.PermitirExcel,
                    PermitirImagens = model.PermitirImagens,
                    TamanhoMaximoMB = model.TamanhoMaximoMB,
                    DiasArmazenamentoTemporario = model.DiasArmazenamentoTemporario
                };

                var result = await _configuracaoDocumentoService.SalvarConfiguracoesAsync(dto);
                if (!result.Success)
                {
                    throw new Exception(result.ErrorMessage);
                }

                TempData["SuccessMessage"] = "Configurações salvas com sucesso!";
                return RedirectToAction("ConfiguracaoDocumentos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar configurações de documentos");
                TempData["ErrorMessage"] = "Ocorreu um erro ao salvar as configurações.";
                return View("ConfiguracaoDocumentos", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarUsuario(AdminUsuariosViewModel model)
        {
            // Carrega dados necessários para o caso de erro
            async Task CarregarDadosView()
            {
                model.PlanosDisponiveis = (await _planosService.GetAllPlansAsync()).Select(p => p.Nome);
                model.EmpresasDisponiveis = (await _empresaService.GetAllAsync()).Select(e => e.EmpresaContratante);
            }

            if (!ModelState.IsValid)
            {
                await CarregarDadosView();
                return View("CadastroFuncionario", model);
            }

            // Validação adicional da senha
            if (string.IsNullOrWhiteSpace(model.SenhaRegistro))
            {
                ModelState.AddModelError("SenhaRegistro", "A senha é obrigatória");
                await CarregarDadosView();
                return View("CadastroFuncionario", model);
            }

            if (model.SenhaRegistro != model.ConfirmacaoSenha)
            {
                ModelState.AddModelError("ConfirmacaoSenha", "As senhas não coincidem");
                await CarregarDadosView();
                return View("CadastroFuncionario", model);
            }

            try
            {
                var novoUsuario = new DTORegistrarUsuario
                {
                    Usuario = model.UsuarioSelecionado.Usuario,
                    Email = model.UsuarioSelecionado.Email,
                    Senha = model.SenhaRegistro,
                    PermissaoConta = model.UsuarioSelecionado.PermissaoConta,
                    EmpresaContratante = model.UsuarioSelecionado.EmpresaContratante,
                    PlanoPago = model.UsuarioSelecionado.Plano
                };

                var (usuarioCriado, _, _) = await _userService.RegisterUserAsync(novoUsuario);
                
                // Limpa os dados sensíveis após o uso
                model.SenhaRegistro = null;
                model.ConfirmacaoSenha = null;

                TempData["SuccessMessage"] = "Usuário criado com sucesso!";
                return RedirectToAction("Usuarios");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário");
                TempData["ErrorMessage"] = "Ocorreu um erro ao criar o usuário.";
                
                // Limpa a senha também em caso de erro
                model.SenhaRegistro = null;
                model.ConfirmacaoSenha = null;
                
                await CarregarDadosView();
                return View("CadastroFuncionario", model);
            }
        }
    }
}