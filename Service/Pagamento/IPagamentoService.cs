using VoxDocs.DTO;
using VoxDocs.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoxDocs.Services
{
    public interface IPagamentoFalsoService
    {
        Task<string> ProcessarPagamentoCartaoFalsoAsync(DTOCartaoPagamentoFalso dto);

        Task<(int pagamentoPixId, string qrCodeUrl)> GerarPixAsync(DTOPixGerar dto);

        Task<string> ConfirmarPixAsync(DTOPixConfirmar dto);
        Task<bool> TokenPixExisteAsync(string token);
        Task<bool> PixStatusAsync(int pagamentoPixId);
    }
}