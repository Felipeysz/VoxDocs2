using VoxDocs.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VoxDocs.Services
{
    public interface IPagamentoBusinessRules
    {
        Task<PagamentoConcluido> ValidarPagamentoExisteAsync(Guid id);
        Task ValidarSolicitacaoPagamentoAsync(PagamentoConcluido pagamento);
        Task ValidarCadastroPagamentoAsync(PagamentoConcluido pagamento);
        Task ValidarMetodoPagamentoAsync(string metodoPagamento);
    }
}