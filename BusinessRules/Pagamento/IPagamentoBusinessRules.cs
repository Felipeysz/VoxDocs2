using System.Collections.Generic;
using System.Threading.Tasks;
using VoxDocs.DTO;

namespace VoxDocs.Services
{
    public interface IPagamentoBusinessRules
    {
        Task ValidarPagamentoExisteAsync(string id);
        Task ValidarDadosPagamentoAsync(string id, string nomePlano, string periodicidade);
        Task ValidarMetodoPagamentoAsync(string metodoPagamento);
        Task ValidarPlanoExisteAsync(string nomePlano, string periodicidade);
        Task<TokenResponseDto> ValidarTokenPagoAsync(TokenRequestDto request);
        Task<PlanoInfoDto> ValidarObterDadosPlanoAsync(string token);
        Task ValidarDadosExisteAsync(string empresaContratante, List<DTORegisterUser> usuarios);
    }
}
