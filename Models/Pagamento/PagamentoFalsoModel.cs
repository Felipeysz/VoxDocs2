namespace VoxDocs.Models
{
    public class PagamentoCartaoFalsoModel
    {
        public int Id { get; set; }
        public required string CartaoNumber { get; set; }
        public required string ValidadeCartao { get; set; }
        public required string CvvCartao { get; set; }
        public required string TipoCartao { get; set; }    // "Credito" ou "Debito"
        public required string TipoPlano { get; set; }
        public decimal ValorPlano { get; set; }           // será preenchido pelo serviço
        public DateTime DataPagamento { get; set; }
        public DateTime DataExpiracao { get; set; }
    }

    public class PagamentoPixModel
    {
        public int Id { get; set; }
        public required string QRCodePix { get; set; }     // gerado pelo serviço
        public required string TipoPlano { get; set; }
        public decimal ValorPlano { get; set; }           // será preenchido pelo serviço
        public bool Confirmado { get; set; }              // sinaliza pagamento concluído
        public DateTime? DataPagamento { get; set; }
        public DateTime? DataExpiracao { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
