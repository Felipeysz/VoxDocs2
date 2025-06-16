using VoxDocs.DTO;
using System.Threading.Tasks;

namespace VoxDocs.Services
{
    public interface IPagamentoService
    {
        Task<string> CriarSolicitacaoPagamentoAsync(CriarPlanoDto dto);
        Task<string> CriarCadastroPagamentoAsync(CriarCadastroPagamentoPlanoDto dto);
    }
}