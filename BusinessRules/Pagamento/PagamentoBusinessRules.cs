using VoxDocs.Models;
using VoxDocs.Data;
using Microsoft.EntityFrameworkCore;

namespace VoxDocs.Services
{
    public class PagamentoBusinessRules : IPagamentoBusinessRules
    {
        private readonly IPagamentoRepository _repository;
        private readonly VoxDocsContext _context;

        public PagamentoBusinessRules(IPagamentoRepository repository, VoxDocsContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<PagamentoConcluido> ValidarPagamentoExisteAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("ID do pagamento não pode ser vazio");

            var pagamento = await _repository.GetPagamentoByIdAsync(id);
            
            return pagamento ?? throw new KeyNotFoundException($"Pagamento com ID {id} não encontrado");
        }
        public async Task ValidarSolicitacaoPagamentoAsync(PagamentoConcluido pagamento)
        {
            if (pagamento == null)
                throw new ArgumentNullException(nameof(pagamento));

            // Validações básicas
            if (string.IsNullOrWhiteSpace(pagamento.NomePlano))
                throw new ArgumentException("Nome do Plano não informado");

            if (string.IsNullOrWhiteSpace(pagamento.PeriodicidadePlano))
                throw new ArgumentException("Periodicidade do plano não informada");

            await ValidarMetodoPagamentoAsync(pagamento.MetodoPagamento);
        }       
        public async Task ValidarCadastroPagamentoAsync(PagamentoConcluido pagamento)
        {
            if (pagamento == null)
                throw new ArgumentNullException(nameof(pagamento));

            // Validações básicas
            if (string.IsNullOrWhiteSpace(pagamento.EmpresaContratante))
                throw new ArgumentException("Empresa contratante não informada");

            if (string.IsNullOrWhiteSpace(pagamento.MetodoPagamento))
                throw new ArgumentException("Método de pagamento não informado");

            if (string.IsNullOrWhiteSpace(pagamento.PeriodicidadePlano))
                throw new ArgumentException("Periodicidade do plano não informada");

            await ValidarMetodoPagamentoAsync(pagamento.MetodoPagamento);
        }
        public Task ValidarMetodoPagamentoAsync(string metodoPagamento)
        {
            var metodosValidos = new[] { "PIX", "CARTAO", "PENDENTE" };

            if (!metodosValidos.Contains(metodoPagamento.ToUpper()))
                throw new ArgumentException($"Método inválido. Use: {string.Join(", ", metodosValidos)}");

            return Task.CompletedTask;
        }

    }
}