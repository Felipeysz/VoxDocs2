using VoxDocs.DTO;
using System.Net;

namespace VoxDocs.Services
{
    public class PagamentoBusinessRules : IPagamentoBusinessRules
    {
        private readonly IPagamentoRepository _repository;
        private readonly IPlanosVoxDocsService _planoService;
        private readonly IUserService _userService;
        private readonly IEmpresasContratanteService _empresaService;

        public PagamentoBusinessRules(
            IPagamentoRepository repository,
            IPlanosVoxDocsService planoService,
            IUserService userService,
            IEmpresasContratanteService empresaService)
        {
            _repository = repository;
            _planoService = planoService;
            _userService = userService;
            _empresaService = empresaService;
        }

        public async Task<ValidationResult> ValidarPagamentoExisteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return ValidationResult.Error("O ID do pagamento não pode ser vazio.", HttpStatusCode.BadRequest);

            var pagamento = await _repository.GetPagamentoById(id);
            
            return pagamento == null 
                ? ValidationResult.Error($"Pagamento com ID {id} não encontrado.", HttpStatusCode.NotFound)
                : ValidationResult.Success(pagamento);
        }

        public async Task<ValidationResult> ValidarDadosPagamentoAsync(string id, string nomePlano, string periodicidade)
        {
            // Validações individuais
            if (string.IsNullOrWhiteSpace(id))
                return ValidationResult.Error("O ID do pagamento não pode ser vazio.", HttpStatusCode.BadRequest);

            if (string.IsNullOrWhiteSpace(nomePlano))
                return ValidationResult.Error("O nome do plano não pode ser vazio.", HttpStatusCode.BadRequest);

            if (string.IsNullOrWhiteSpace(periodicidade))
                return ValidationResult.Error("A periodicidade não pode ser vazia.", HttpStatusCode.BadRequest);

            // Validação de existência do pagamento
            var pagamento = await _repository.GetPagamentoById(id);
            if (pagamento == null)
                return ValidationResult.Error($"Pagamento com ID {id} não encontrado.", HttpStatusCode.NotFound);

            // Validações específicas
            var errors = new List<string>();

            if (pagamento.NomePlano != nomePlano)
                errors.Add("Nome do plano não corresponde ao registro.");

            if (pagamento.Periodicidade != periodicidade)
                errors.Add("Periodicidade do plano não corresponde ao registro.");

            return errors.Any()
                ? ValidationResult.Error(string.Join(" ", errors), HttpStatusCode.BadRequest)
                : ValidationResult.Success(pagamento);
        }

        public Task<ValidationResult> ValidarMetodoPagamentoAsync(string metodoPagamento)
        {
            if (string.IsNullOrWhiteSpace(metodoPagamento))
                return Task.FromResult(ValidationResult.Error("Método de pagamento não informado.", HttpStatusCode.BadRequest));

            var metodosValidos = new[] { "Cartão", "Boleto", "PIX" };
            if (!metodosValidos.Contains(metodoPagamento))
                return Task.FromResult(ValidationResult.Error($"Método de pagamento '{metodoPagamento}' não é válido. Métodos aceitos: {string.Join(", ", metodosValidos)}", HttpStatusCode.BadRequest));

            return Task.FromResult(ValidationResult.Success());
        }

        public async Task<ValidationResult> ValidarPlanoExisteAsync(string nomePlano, string periodicidade)
        {
            // Validações individuais
            if (string.IsNullOrWhiteSpace(nomePlano))
                return ValidationResult.Error("O nome do plano não pode ser vazio.", HttpStatusCode.BadRequest);

            if (string.IsNullOrWhiteSpace(periodicidade))
                return ValidationResult.Error("A periodicidade não pode ser vazia.", HttpStatusCode.BadRequest);

            // Validação de existência do plano
            var plano = await _planoService.GetPlanByNameAsync(nomePlano);
            if (plano == null)
                return ValidationResult.Error($"Plano '{nomePlano}' não encontrado.", HttpStatusCode.NotFound);

            // Validação de periodicidade
            if (!string.Equals(plano.Periodicidade, periodicidade, StringComparison.OrdinalIgnoreCase))
                return ValidationResult.Error($"Periodicidade do plano '{periodicidade}' não corresponde à periodicidade registrada '{plano.Periodicidade}'.", HttpStatusCode.BadRequest);

            return ValidationResult.Success(plano);
        }

        public async Task<TokenResponseDto> ValidarTokenPagoAsync(TokenRequestDto request)
        {
            if (request == null)
                return new TokenResponseDto { Sucesso = false, Mensagem = "Requisição inválida.", CodigoStatus = (int)HttpStatusCode.BadRequest };

            if (string.IsNullOrWhiteSpace(request.Token))
                return new TokenResponseDto { Sucesso = false, Mensagem = "Token não informado.", CodigoStatus = (int)HttpStatusCode.BadRequest };

            var pagamento = await _repository.GetPagamentoById(request.Token);
            if (pagamento == null)
                return new TokenResponseDto { Sucesso = false, Mensagem = $"Token '{request.Token}' não existe.", CodigoStatus = (int)HttpStatusCode.NotFound };

            return pagamento.IsPagamentoConcluido
                ? new TokenResponseDto { Sucesso = true, Mensagem = "Token Pago", CodigoStatus = (int)HttpStatusCode.OK }
                : new TokenResponseDto { Sucesso = false, Mensagem = "O Token Não Foi pago", CodigoStatus = (int)HttpStatusCode.PaymentRequired };
        }

        public async Task<ValidationResult<PlanoInfoDto>> ValidarObterDadosPlanoAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return ValidationResult<PlanoInfoDto>.Error("Token não informado.", HttpStatusCode.BadRequest);

            var pagamento = await _repository.GetPagamentoById(token);
            if (pagamento == null)
                return ValidationResult<PlanoInfoDto>.Error($"Token '{token}' inexistente.", HttpStatusCode.NotFound);

            if (pagamento.IsPagamentoConcluido)
                return ValidationResult<PlanoInfoDto>.Error("Plano já foi pago.", HttpStatusCode.Conflict);

            return ValidationResult<PlanoInfoDto>.Success(new PlanoInfoDto
            {
                NomePlano = pagamento.NomePlano,
                Periodicidade = pagamento.Periodicidade
            });
        }

        public async Task<ValidationResult> ValidarDadosExisteAsync(string empresaContratante, List<DTORegisterUser> usuarios)
        {
            // Validações de entrada
            if (string.IsNullOrWhiteSpace(empresaContratante))
                return ValidationResult.Error("Nome da empresa contratante não pode ser vazio.", HttpStatusCode.BadRequest);

            if (usuarios == null || !usuarios.Any())
                return ValidationResult.Error("Pelo menos um usuário deve ser informado.", HttpStatusCode.BadRequest);

            // Validação de empresa
            var empresaExistente = await _empresaService.GetEmpresaByNome(empresaContratante);
            if (empresaExistente != null)
                return ValidationResult.Error($"A empresa '{empresaContratante}' já está registrada.", HttpStatusCode.Conflict);

            // Validação de usuários
            var usuariosInvalidos = new List<string>();
            foreach (var usuario in usuarios)
            {
                if (string.IsNullOrWhiteSpace(usuario.Usuario) || string.IsNullOrWhiteSpace(usuario.Email))
                {
                    usuariosInvalidos.Add("Usuário ou email não informado.");
                    continue;
                }

                var userExistente = await _userService.GetUserByEmailOrUsernameAsync(usuario.Email, usuario.Usuario);
                if (userExistente != null)
                {
                    var mensagem = userExistente.Usuario == usuario.Usuario
                        ? $"O nome de usuário '{usuario.Usuario}' já está em uso."
                        : $"O email '{usuario.Email}' já está cadastrado.";
                    
                    usuariosInvalidos.Add(mensagem);
                }
            }

            return usuariosInvalidos.Any()
                ? ValidationResult.Error($"Problemas com os usuários: {string.Join(" ", usuariosInvalidos)}", HttpStatusCode.Conflict)
                : ValidationResult.Success();
        }
    }

    // Classes auxiliares para padronizar as respostas
    public class ValidationResult
    {
        public bool IsValid { get; }
        public string ErrorMessage { get; }
        public HttpStatusCode StatusCode { get; }
        public object Data { get; }

        protected ValidationResult(bool isValid, string errorMessage, HttpStatusCode statusCode, object data = null)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
            StatusCode = statusCode;
            Data = data;
        }

        public static ValidationResult Success(object data = null) => new ValidationResult(true, null, HttpStatusCode.OK, data);
        public static ValidationResult Error(string message, HttpStatusCode statusCode) => new ValidationResult(false, message, statusCode);
    }

    public class ValidationResult<T> : ValidationResult
    {
        public T Result => (T)Data;

        private ValidationResult(bool isValid, string errorMessage, HttpStatusCode statusCode, T data) 
            : base(isValid, errorMessage, statusCode, data)
        {
        }

        public static ValidationResult<T> Success(T data) => new ValidationResult<T>(true, null, HttpStatusCode.OK, data);
        public static new ValidationResult<T> Error(string message, HttpStatusCode statusCode) => new ValidationResult<T>(false, message, statusCode, default);
    }
}