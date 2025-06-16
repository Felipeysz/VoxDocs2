// DTOs/DTOPlanosVoxDocs.cs
namespace VoxDocs.DTO
{
    public class DTOPlanosVoxDocs
    {
        public required string Nome { get; set; }
        public required string Descricao { get; set; }
        public decimal Preco { get; set; }
        public int? Duracao { get; set; } 
        public required string Periodicidade { get; set; }
        public int? ArmazenamentoDisponivel { get; set; }
        public int? LimiteAdmin { get; set; }
        public int? LimiteUsuario { get; set; }
    }
}
