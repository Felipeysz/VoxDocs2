using VoxDocs.DTO;
using System.Net;
using System.Threading.Tasks;

namespace VoxDocs.Services
{
    public interface IPagamentoBusinessRules
    {
        Task<ValidationResult> ValidarPagamentoExisteAsync(string id);
        Task<ValidationResult> ValidarDadosPagamentoAsync(string id, string nomePlano, string periodicidade);
        Task<ValidationResult> ValidarMetodoPagamentoAsync(string metodoPagamento);
        Task<ValidationResult> ValidarPlanoExisteAsync(string nomePlano, string periodicidade);
        Task<TokenResponseDto> ValidarTokenPagoAsync(TokenRequestDto request);
        Task<ValidationResult<PlanoInfoDto>> ValidarObterDadosPlanoAsync(string token);
        Task<ValidationResult> ValidarDadosExisteAsync(string empresaContratante, List<DTORegisterUser> usuarios);
    }
}