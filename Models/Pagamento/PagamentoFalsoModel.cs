using System.ComponentModel.DataAnnotations;

namespace VoxDocs.Models
{
    public class PagamentoConcluido
    {
        [Key]
        public required string Id { get; set; }                // Será o hash do GUID
        public string? EmpresaContratantePlano { get; set; }        // opcional, pode ficar null
        public string? MetodoPagamento { get; set; }           // Cartão, Pix; só preenchido no Concluído
        public DateTime? DataPagamento { get; set; }   // já setado no UTC now
        public DateTime? DataExpiracao { get; set; }           // só no Concluído
        public string? StatusEmpresa { get; set; } = "Em Andamento";
        public required string NomePlano { get; set; }
        public required string Periodicidade { get; set; }
        public bool IsPagamentoConcluido { get; set; } = false;
    }
}
