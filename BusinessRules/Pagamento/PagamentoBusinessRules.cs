using VoxDocs.Models;
using VoxDocs.DTO;
using BCrypt.Net;
using VoxDocs.Services;
using Bcrypt = BCrypt.Net.BCrypt;

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

        public async Task ValidarPagamentoExisteAsync(string id)
        {
            var pagamento = await _repository.GetPagamentoById(id);
            if (pagamento == null)
                throw new InvalidOperationException("Pagamento não encontrado.");
        }

        public async Task ValidarDadosPagamentoAsync(string id, string nomePlano, string periodicidade)
        {
            var pagamento = await _repository.GetPagamentoById(id);
            if (pagamento == null)
                throw new InvalidOperationException("Pagamento não encontrado.");

            if (pagamento.NomePlano != nomePlano)
                throw new InvalidOperationException("Nome do plano não corresponde ao registro.");

            if (pagamento.Periodicidade != periodicidade)
                throw new InvalidOperationException("Periodicidade do plano não corresponde ao registro.");
        }

        public Task ValidarMetodoPagamentoAsync(string metodoPagamento)
        {
            if (string.IsNullOrWhiteSpace(metodoPagamento))
                throw new InvalidOperationException("Método de pagamento não informado.");
            return Task.CompletedTask;
        }

        public async Task ValidarPlanoExisteAsync(string nomePlano, string periodicidade)
        {
            var plano = await _planoService.GetPlanByNameAsync(nomePlano);
            if (plano == null)
                throw new InvalidOperationException("Plano não encontrado.");

            if (!string.Equals(plano.Periodicidade, periodicidade, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Periodicidade do plano não corresponde.");
        }

        public async Task<TokenResponseDto> ValidarTokenPagoAsync(TokenRequestDto request)
        {
            var pagamento = await _repository.GetPagamentoById(request.Token);
            if (pagamento == null)
            {
                return new TokenResponseDto
                {
                    Sucesso = false,
                    Mensagem = "Token não existe"
                };
            }

            if (pagamento.IsPagamentoConcluido)
            {
                return new TokenResponseDto
                {
                    Sucesso = true,
                    Mensagem = "Token Pago"
                };
            }
            else
            {
                return new TokenResponseDto
                {
                    Sucesso = false,
                    Mensagem = "O Token Não Foi pago"
                };
            }
        }

        public async Task<PlanoInfoDto> ValidarObterDadosPlanoAsync(string token)
        {
            var pagamento = await _repository.GetPagamentoById(token);

            if (pagamento == null)
                throw new InvalidOperationException("Token Inexistente");

            if (pagamento.IsPagamentoConcluido)
                throw new InvalidOperationException("Plano Pago");

            return new PlanoInfoDto
            {
                NomePlano = pagamento.NomePlano,
                Periodicidade = pagamento.Periodicidade
            };
        }

        public async Task ValidarDadosExisteAsync(string empresaContratante, List<DTORegisterUser> usuarios)
        {
            // Verifica se a empresa já existe
            var empresaExistente = await _empresaService.GetEmpresaByNome(empresaContratante);
            if (empresaExistente != null)
                throw new InvalidOperationException($"A empresa '{empresaContratante}' já está registrada.");

            // Verifica se algum usuário já existe (por email ou username)
            foreach (var usuario in usuarios)
            {
                var userExistente = await _userService.GetUserByEmailOrUsername(usuario.Email, usuario.Usuario);
                if (userExistente != null)
                    throw new InvalidOperationException($"O usuário '{usuario.Usuario}' ou email '{usuario.Email}' já está cadastrado.");
            }
        }
    }
}