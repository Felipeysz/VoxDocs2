using VoxDocs.Models;
using System.Threading.Tasks;

namespace VoxDocs.Services
{
    public interface IPagamentoRepository
    {
        Task<PagamentoConcluido> GetPagamentoById(string id);
        Task CreatePagamento(PagamentoConcluido pagamento);
        Task UpdatePagamento(PagamentoConcluido pagamento);
        Task<EmpresasContratanteModel> GetEmpresaByNome(string nome);
        Task CreateEmpresa(EmpresasContratanteModel empresa);
        Task CreatePastaPrincipal(PastaPrincipalModel pasta);
        Task CreateSubPasta(SubPastaModel subpasta);
    }
}