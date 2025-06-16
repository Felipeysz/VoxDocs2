using VoxDocs.DTO;
using System.Collections.Generic;

namespace VoxDocs.Models.ViewModels
{
    public class DocumentosViewModel
    {
        public List<DTOPastaPrincipal> PastaPrincipais { get; set; } = new List<DTOPastaPrincipal>();
        public IEnumerable<DTOSubPasta> SubPastas { get; set; } = Enumerable.Empty<DTOSubPasta>();
        public IEnumerable<DocumentoDto> Documentos { get; set; } = Enumerable.Empty<DocumentoDto>();
        public string? SelectedPastaPrincipalNome { get; set; }
        public string? SelectedSubPastaNome { get; set; }
        public bool IsOfflineMode { get; set; } = false;
    }
}