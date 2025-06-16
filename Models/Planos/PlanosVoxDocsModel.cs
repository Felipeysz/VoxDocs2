namespace VoxDocs.Models
{
    public class PlanosVoxDocsModel
    {
        public required Guid Id { get; set; } = Guid.NewGuid();
        public required string Nome { get; set; }
        public required string Descriçao { get; set; }
        public required decimal Preco { get; set; }
        public required string Periodicidade { get; set; } // Mensal, Trimestral, Semestral
        public required int? Duracao { get; set; }
        public required int? ArmazenamentoDisponivel { get; set; }
        public required int? LimiteAdmin { get; set; }
        public required int? LimiteUsuario { get; set; }
        public bool Ativo { get; set; }
    }
}