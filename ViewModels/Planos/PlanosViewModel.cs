// ViewModels/PlanosViewModel.cs
using VoxDocs.Models;
using System.Collections.Generic;

namespace VoxDocs.ViewModels
{
    public class PlanosViewModel
    {
        public List<PlanosVoxDocsModel> Planos { get; set; } = new List<PlanosVoxDocsModel>();
    }
}