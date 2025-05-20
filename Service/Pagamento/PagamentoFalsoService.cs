using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VoxDocs.Data;
using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public class PagamentoFalsoService : IPagamentoFalsoService
    {
        private readonly VoxDocsContext _context;

        public PagamentoFalsoService(VoxDocsContext context)
            => _context = context;

        public async Task<string> ProcessarPagamentoCartaoFalsoAsync(DTOCartaoPagamentoFalso dto)
        {
            var plano = await _context.PlanosVoxDocs
                .SingleOrDefaultAsync(p => p.Name == dto.TipoPlano)
                ?? throw new Exception("Plano não encontrado.");

            var now = DateTime.UtcNow;
            var pagamento = new PagamentoCartaoFalsoModel
            {
                CartaoNumber   = dto.CartaoNumber,
                ValidadeCartao = dto.ValidadeCartao,
                CvvCartao      = dto.CvvCartao,
                TipoCartao     = dto.TipoCartao,
                TipoPlano      = dto.TipoPlano,
                ValorPlano     = plano.Price,
                DataPagamento  = now,
                DataExpiracao  = now.AddMonths(plano.Duration)
            };

            _context.PagamentosCartao.Add(pagamento);
            await _context.SaveChangesAsync();
            return "Pagamento com cartão processado com sucesso!";
        }
        public async Task<(int pagamentoPixId, string qrCodeUrl)> GerarPixAsync(DTOPixGerar dto)
        {
            // 1) Limpeza dos QRCodes não confirmados há mais de 1h
            var cutoff = DateTime.UtcNow.AddHours(-1);
            var antigos = _context.PagamentosPix
                .Where(p => !p.Confirmado && p.CreatedAt < cutoff);
            _context.PagamentosPix.RemoveRange(antigos);
            await _context.SaveChangesAsync();

            // 2) Cria novo registro Pix
            var plano = await _context.PlanosVoxDocs
                .SingleOrDefaultAsync(p => p.Name == dto.TipoPlano)
                ?? throw new Exception("Plano não encontrado.");

            var token = Guid.NewGuid().ToString();
            var now   = DateTime.UtcNow;

            var pix = new PagamentoPixModel
            {
                QRCodePix   = token,
                TipoPlano   = dto.TipoPlano,
                ValorPlano  = plano.Price,
                Confirmado  = false,
                CreatedAt   = now
            };

            _context.PagamentosPix.Add(pix);
            await _context.SaveChangesAsync();

            // URL relativa para o front gerar o QR
            var qrCodeUrl = $"/ConfirmandoPagamentoPix?Token={token}";
            return (pix.Id, qrCodeUrl);
        }
        public async Task<bool> PixStatusAsync(int pagamentoPixId)
        {
            var pix = await _context.PagamentosPix
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id == pagamentoPixId);

            if (pix == null)
                throw new Exception("Pagamento Pix não encontrado.");

            return pix.Confirmado;
        }
        public async Task<string> ConfirmarPixAsync(DTOPixConfirmar dto)
        {
            // Busca pelo token
            var pix = await _context.PagamentosPix
                .FirstOrDefaultAsync(p => p.QRCodePix == dto.Token)
                ?? throw new Exception("Pagamento Pix não encontrado.");

            if (pix.Confirmado)
                throw new Exception("Pagamento Pix já foi confirmado.");

            var plano = await _context.PlanosVoxDocs
                .SingleOrDefaultAsync(p => p.Name == pix.TipoPlano)
                ?? throw new Exception("Plano não encontrado ao confirmar Pix.");

            var now = DateTime.UtcNow;
            pix.Confirmado = true;
            pix.DataPagamento = now;
            pix.DataExpiracao = now.AddMonths(plano.Duration);
            // sobrescreve QRCodePix para indicar confirmação
            pix.QRCodePix = "Pagamento Confirmado!";

            await _context.SaveChangesAsync();
            return "Pagamento via Pix confirmado com sucesso!";
        }
        public async Task<bool> TokenPixExisteAsync(string token)
        {
            return await _context.PagamentosPix
                .AsNoTracking()
                .AnyAsync(p => p.QRCodePix == token);
        }
    }
}
