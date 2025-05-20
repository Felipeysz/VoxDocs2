using System.Collections.Generic;
using VoxDocs.Models;

namespace VoxDocs.ViewModels
{


    public class PlanosIndexViewModel
    {
        public List<PlanoViewModel> Basicos { get; set; }
        public List<PlanoViewModel> Intermediarios { get; set; }
        public List<PlanoViewModel> Avancados { get; set; }

        // Novos campos para menor preço de cada categoria
        public decimal MenorPrecoBasico { get; set; }
        public decimal MenorPrecoIntermediario { get; set; }
        public decimal MenorPrecoAvancado { get; set; }
    }

    public class PlanoViewModel
    {
        public string Periodicidade { get; set; }
        public decimal Price { get; set; }
        public int? LimiteUsuario { get; set; }
        public int? LimiteAdmin { get; set; }
    }


}
