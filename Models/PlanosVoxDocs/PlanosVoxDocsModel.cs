namespace VoxDocs.Models
{
    public class PlanosVoxDocsModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }               // em meses
        public required string Periodicidade { get; set; } // Mensal, Trimestral, Semestral
        public required int ArmazenamentoDisponivel { get; set; }
        public string TokensDisponiveis { get; set; } = "Infinito";
        public int? LimiteAdmin { get; internal set; }
        public int? LimiteUsuario { get; internal set; }
        public int MaxStorageGb { get; set; } // Adicionado
        public int UserLimit { get; set; } // Adicionado
        public int? TokenLimit { get; set; } // Adicionado
    }
}