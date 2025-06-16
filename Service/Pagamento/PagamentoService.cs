using VoxDocs.Models;
using VoxDocs.DTO;
using VoxDocs.Data;
using System.Net;

namespace VoxDocs.Services
{
    public class PagamentoService : IPagamentoService
    {
        private readonly IPagamentoRepository _repository;
        private readonly IPagamentoBusinessRules _businessRules;
        private readonly IUserService _userService;
        private readonly IEmpresasContratanteService _empresasContratanteService;
        private readonly IDocumentosPastasService _documentosPastasService;


        public PagamentoService(
            IPagamentoRepository repository,
            IPagamentoBusinessRules businessRules,
            IUserService userService,
            IEmpresasContratanteService empresasContratanteService,
            IDocumentosPastasService documentosPastasService)
        {
            _repository = repository;
            _businessRules = businessRules;
            _userService = userService;
            _empresasContratanteService = empresasContratanteService;
            _documentosPastasService = documentosPastasService;
        }

        public async Task<string> CriarSolicitacaoPagamentoAsync(CriarPlanoDto dto)
        {
            var pagamento = new PagamentoConcluido
            {
                Id = dto.PagamentoId,
                EmpresaContratante = dto.EmpresaContratante,
                NomePlano = dto.nomePlano,
                PeriodicidadePlano = dto.nomePlano.Contains("Gratuito", StringComparison.OrdinalIgnoreCase)
                    ? "Ilimitado"
                    : dto.periodicidade,
                ValorPlano = dto.valorPlano,
                MetodoPagamento = dto.MetodoPagamento,
                DataPagamento = dto.DataPagamento,
                DataExpiracao = dto.DataExpiracao ?? DateTime.MaxValue,
                StatusEmpresa = dto.StatusEmpresa
            };

            // Validações pré-criação
            await _businessRules.ValidarSolicitacaoPagamentoAsync(pagamento);
            await _businessRules.ValidarMetodoPagamentoAsync(dto.MetodoPagamento);

            // Cria o pagamento
            await _repository.CreatePagamentoAsync(pagamento);

            // Validação pós-criação
            var pagamentoCriado = await _businessRules.ValidarPagamentoExisteAsync(pagamento.Id);
            if (pagamentoCriado == null)
            {
                throw new InvalidOperationException("Falha ao verificar a criação do pagamento");
            }

            return pagamento.Id.ToString();
        }

       public async Task<PagamentoResponseDto> CriarCadastroPagamentoAsync(CriarCadastroPagamentoPlanoDto dto)
        {
            try
            {
                // Validação opcional do método de pagamento
                var metodosPermitidos = new[] { "PIX", "Cartão de Crédito", "Boleto", "Transferência" };
                if (!metodosPermitidos.Contains(dto.MetodoPagamento))
                {
                    return PagamentoResponseDto.Falha(
                        erro: "Método de pagamento inválido",
                        status: HttpStatusCode.BadRequest,
                        mensagem: $"Os métodos permitidos são: {string.Join(", ", metodosPermitidos)}"
                    );
                }

                // ETAPA 1 - Processar empresa
                EmpresasContratanteModel empresaContratante = await ProcessarEmpresaContratante(dto);

                // ETAPA 2 - Criar estrutura de pastas dinâmica
                var estruturaPastas = await CriarEstruturaPastas(
                    empresaContratante.EmpresaContratante, 
                    dto.Pastas);

                // ETAPA 3 - Cadastrar Admins
                foreach (var adminDto in dto.AdminUsuarios)
                {
                    var registroAdmin = new DTORegistrarUsuario
                    {
                        Usuario = adminDto.Nome,
                        Email = adminDto.Email,
                        Senha = adminDto.Senha,
                        EmpresaContratante = empresaContratante.EmpresaContratante,
                        PermissaoConta = PermissaoConta.Admin.ToString(),
                        PlanoPago = dto.nomePlano
                    };

                    var (admin, adminLimit, _) = await _userService.RegisterUserAsync(registroAdmin);
                    
                    if (admin == null || !string.IsNullOrEmpty(adminLimit))
                    {
                        return PagamentoResponseDto.Falha(
                            erro: "Limite de admins atingido",
                            status: HttpStatusCode.BadRequest,
                            mensagem: adminLimit ?? "Não foi possível cadastrar o admin"
                        );
                    }
                }

                // ETAPA 4 - Cadastrar Usuários Comuns
                var errosCadastro = new List<string>();
                
                foreach (var usuarioDto in dto.UsuariosComum)
                {
                    try
                    {
                        var registroUsuario = new DTORegistrarUsuario
                        {
                            Usuario = usuarioDto.Nome,
                            Email = usuarioDto.Email,
                            Senha = usuarioDto.Senha,
                            EmpresaContratante = empresaContratante.EmpresaContratante,
                            PermissaoConta = PermissaoConta.User.ToString(),
                            PlanoPago = dto.nomePlano
                        };

                        var (user, _, userLimit) = await _userService.RegisterUserAsync(registroUsuario);
                        
                        if (user == null || !string.IsNullOrEmpty(userLimit))
                        {
                            var erroMsg = $"Falha ao cadastrar usuário {usuarioDto.Nome}: {userLimit}";
                            errosCadastro.Add(erroMsg);
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] WARNING - {erroMsg}");
                        }
                    }
                    catch (Exception ex)
                    {
                        var erroMsg = $"Erro ao cadastrar usuário {usuarioDto.Nome}: {ex.Message}";
                        errosCadastro.Add(erroMsg);
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ERROR - {erroMsg}");
                    }
                }

                // ETAPA 5 - Montar resposta
                return MontarRespostaSucesso(dto, empresaContratante, estruturaPastas);
            }
            catch (Exception ex)
            {
                return PagamentoResponseDto.Falha(
                    erro: ex.Message,
                    status: HttpStatusCode.InternalServerError,
                    mensagem: "Erro ao processar o pagamento",
                    detalhes: ex.StackTrace
                );
            }
        }
        
            private PagamentoResponseDto MontarRespostaSucesso(
            CriarCadastroPagamentoPlanoDto dto,
            EmpresasContratanteModel empresa,
            dynamic estruturaPastas)
        {
            var resposta = new
            {
                PagamentoId = dto.PagamentoId,
                Empresa = new {
                    Id = empresa.Id,
                    Nome = empresa.EmpresaContratante,
                    Status = dto.StatusEmpresa
                },
                Plano = new {
                    Nome = dto.nomePlano,
                    Valor = dto.valorPlano,
                    Periodicidade = dto.periodicidade,
                    DataExpiracao = dto.DataExpiracao
                },
                EstruturaPastas = estruturaPastas
            };

            return PagamentoResponseDto.Ok(resposta, "Pagamento e estrutura de pastas criados com sucesso");
        }

        // Métodos auxiliares para cada etapa
        private async Task<EmpresasContratanteModel> ProcessarEmpresaContratante(CriarCadastroPagamentoPlanoDto dto)
        {
            var empresaExistente = await _empresasContratanteService.GetEmpresaByNome(dto.EmpresaContratante);

            if (empresaExistente != null) return empresaExistente;

            var novaEmpresaDto = new DTOEmpresasContratante
            {
                EmpresaContratante = dto.EmpresaContratante,
                Email = dto.EmailEmpresaContratante,
                PlanoContratado = dto.nomePlano,
                DataContratacao = dto.DataPagamento
            };

            return await _empresasContratanteService.CreateAsync(novaEmpresaDto);
        }

        private async Task<dynamic> CriarEstruturaPastas(string empresaContratante, List<PastaConfigurationDto> pastasConfiguracao)
        {
            if (pastasConfiguracao == null || !pastasConfiguracao.Any())
            {
                return new { Mensagem = "Nenhuma pasta configurada para criação" };
            }

            var resultado = new List<object>();

            foreach (var pastaConfig in pastasConfiguracao)
            {
                // Validação básica
                if (string.IsNullOrWhiteSpace(pastaConfig.NomePastaPrincipal))
                {
                    continue; // ou lançar exceção específica
                }

                // Cria pasta principal
                var pastaPrincipalDto = new DTOPastaPrincipalCreate
                {
                    NomePastaPrincipal = pastaConfig.NomePastaPrincipal,
                    EmpresaContratante = empresaContratante
                };

                var pastaPrincipal = await _documentosPastasService.CreatePastaPrincipalAsync(pastaPrincipalDto);

                // Processa subpastas se existirem
                var subpastasCriadas = new List<DTOSubPasta>();
                
                if (pastaConfig.Subpastas != null)
                {
                    foreach (var subpastaConfig in pastaConfig.Subpastas)
                    {
                        if (string.IsNullOrWhiteSpace(subpastaConfig.NomeSubPasta))
                        {
                            continue;
                        }

                        var subpastaDto = new DTOSubPastaCreate
                        {
                            NomeSubPasta = subpastaConfig.NomeSubPasta,
                            NomePastaPrincipal = pastaPrincipal.NomePastaPrincipal,
                            EmpresaContratante = empresaContratante
                        };
                        
                        subpastasCriadas.Add(await _documentosPastasService.CreateSubPastaAsync(subpastaDto));
                    }
                }

                resultado.Add(new 
                {
                    PastaPrincipal = pastaPrincipal,
                    Subpastas = subpastasCriadas
                });
            }

            return resultado;
        }

        Task<string> IPagamentoService.CriarCadastroPagamentoAsync(CriarCadastroPagamentoPlanoDto dto)
        {
            throw new NotImplementedException();
        }
    }
}