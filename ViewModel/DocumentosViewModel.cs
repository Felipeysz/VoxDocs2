using System.Collections.Generic;
using VoxDocs.DTO;
using VoxDocs.Models.Dto;

namespace VoxDocs.Models.ViewModels
{
    public class DocumentosViewModel
    {
        public IEnumerable<DTOPastaPrincipal> PastaPrincipais { get; set; } = new List<DTOPastaPrincipal>();
        public IEnumerable<DTOSubPasta> SubPastas { get; set; } = new List<DTOSubPasta>();
        public IEnumerable<DTODocumentoCreate> Documentos { get; set; } = new List<DTODocumentoCreate>();
        public string? TokenSeguranca { get; set; } // Added for token validation
        public string? NivelSeguranca { get; set; } // Added for security level
    }
}