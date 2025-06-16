using VoxDocs.Data;
using VoxDocs.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VoxDocs.Services
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly VoxDocsContext _context;

        public PagamentoRepository(VoxDocsContext context) => _context = context;

        public async Task<PagamentoConcluido> GetPagamentoByIdAsync(Guid id)
            => await _context.PagamentosConcluidos.FindAsync(id);

        public async Task<PagamentoConcluido> GetPagamentoByEmpresaAsync(string empresaContratante)
            => await _context.PagamentosConcluidos
                .Where(p => p.EmpresaContratante == empresaContratante)
                .OrderByDescending(p => p.DataPagamento)
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<PagamentoConcluido>> GetPagamentosByStatusAsync(string status)
            => await _context.PagamentosConcluidos
                .Where(p => p.StatusEmpresa == status)
                .ToListAsync();

        public async Task CreatePagamentoAsync(PagamentoConcluido pagamento)
        {
            // Garante valores padrão
            pagamento.StatusEmpresa ??= "Plano Pendente";
            pagamento.MetodoPagamento ??= "PENDENTE";
            
            if (pagamento.DataExpiracao == default)
                pagamento.CalcularDataExpiracao();

            await _context.PagamentosConcluidos.AddAsync(pagamento);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePagamentoAsync(PagamentoConcluido pagamento)
        {
            pagamento.CalcularDataExpiracao();
            _context.PagamentosConcluidos.Update(pagamento);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsPagamentoAtivoAsync(string empresaContratante)
        {
            var pagamento = await GetPagamentoByEmpresaAsync(empresaContratante);
            return pagamento != null && 
                   pagamento.StatusEmpresa == "Plano Ativo" && 
                   pagamento.DataExpiracao > DateTime.Now;
        }

        public async Task<DateTime?> GetProximaRenovacaoAsync(string empresaContratante)
        {
            var pagamento = await GetPagamentoByEmpresaAsync(empresaContratante);
            return pagamento?.DataExpiracao;
        }

        // Métodos para empresas
        public async Task<EmpresasContratanteModel> GetEmpresaByNomeAsync(string nome)
            => await _context.EmpresasContratantes
                .FirstOrDefaultAsync(e => e.EmpresaContratante == nome);

        public async Task CreateEmpresaAsync(EmpresasContratanteModel empresa)
        {
            await _context.EmpresasContratantes.AddAsync(empresa);
            await _context.SaveChangesAsync();
        }
    }
}