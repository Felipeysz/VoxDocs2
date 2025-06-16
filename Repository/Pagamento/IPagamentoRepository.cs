using VoxDocs.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoxDocs.Data
{
    public interface IPagamentoRepository
    {
        // Operações básicas de CRUD
        Task<PagamentoConcluido> GetPagamentoByIdAsync(Guid id);
        Task<PagamentoConcluido> GetPagamentoByEmpresaAsync(string empresaContratante);
        Task<IEnumerable<PagamentoConcluido>> GetPagamentosByStatusAsync(string status);
        Task CreatePagamentoAsync(PagamentoConcluido pagamento);
        Task UpdatePagamentoAsync(PagamentoConcluido pagamento);

        // Operações de verificação
        Task<bool> IsPagamentoAtivoAsync(string empresaContratante);
        Task<DateTime?> GetProximaRenovacaoAsync(string empresaContratante);

        // Operações relacionadas a empresas
        Task<EmpresasContratanteModel> GetEmpresaByNomeAsync(string nome);
        Task CreateEmpresaAsync(EmpresasContratanteModel empresa);
    }
}