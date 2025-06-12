using VoxDocs.Data;
using VoxDocs.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace VoxDocs.Services
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly VoxDocsContext _context;

        public PagamentoRepository(VoxDocsContext context)
        {
            _context = context;
        }

        public async Task<PagamentoConcluido> GetPagamentoById(string id)
        {
            return await _context.PagamentosConcluidos.FindAsync(id);
        }

        public async Task CreatePagamento(PagamentoConcluido pagamento)
        {
            await _context.PagamentosConcluidos.AddAsync(pagamento);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePagamento(PagamentoConcluido pagamento)
        {
            _context.PagamentosConcluidos.Update(pagamento);
            await _context.SaveChangesAsync();
        }

        public async Task<EmpresasContratanteModel> GetEmpresaByNome(string nome)
        {
            return await _context.EmpresasContratantes
                .FirstOrDefaultAsync(e => e.EmpresaContratante == nome);
        }

        public async Task CreateEmpresa(EmpresasContratanteModel empresa)
        {
            await _context.EmpresasContratantes.AddAsync(empresa);
            await _context.SaveChangesAsync();
        }

        public async Task CreatePastaPrincipal(PastaPrincipalModel pasta)
        {
            await _context.PastaPrincipal.AddAsync(pasta);
            await _context.SaveChangesAsync();
        }

        public async Task CreateSubPasta(SubPastaModel subpasta)
        {
            await _context.SubPastas.AddAsync(subpasta);
            await _context.SaveChangesAsync();
        }
    }
}