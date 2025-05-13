using System.Collections.Generic;

namespace VoxDocs.Models.ViewModels
{
    public class DocumentosViewModel
    {
        public IEnumerable<AreasDocumentoModel> AreasDocumento { get; set; }
        public IEnumerable<TipoDocumentoModel> TiposDocumento { get; set; }
        public IEnumerable<DocumentoModel> Documentos { get; set; }
    }
}