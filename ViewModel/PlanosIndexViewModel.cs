using System.Collections.Generic;

namespace VoxDocs.ViewModels
{
    public class PlanosIndexViewModel
    {
        public List<PlanoViewModel> Planos { get; set; } = new();
    }

    public class PlanoViewModel
    {
        public required string Nome { get; set; }
        public required string Periodicidade { get; set; }
        public decimal Preco { get; set; }
        public int? ArmazenamentoDisponivel { get; set; }
        public int? LimiteAdmin { get; set; }
        public int? LimiteUsuario { get; set; }
    }
}
