using VoxDocs.DTO;
using System.Collections.Generic;

namespace VoxDocs.Models.ViewModels
{
    public class DocumentosOfflineViewModel
    {
        public IEnumerable<DocumentoDto> Documentos { get; set; } = Enumerable.Empty<DocumentoDto>();
        public bool IsOfflineMode { get; set; } = true;
        public string StatusMessage { get; set; }
        public bool HasError { get; set; }
    }
}