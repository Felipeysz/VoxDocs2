using VoxDocs.DTO;

namespace VoxDocs.Services
{
    public interface IPagamentoService
    {
        Task<string> CriarPlanoNome(string nomePlanoPlain, string periodicidade);
        Task<bool> FinalizarPagamentoAsync(FinalizarPagamentoDto dto);
        Task<TokenResponseDto> TokenPagoValidoAsync(TokenRequestDto dto);
        Task<PlanoInfoDto> ObterDadosPlanoAsync(string token);
    }
}
