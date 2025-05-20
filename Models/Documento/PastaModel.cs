using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace VoxDocs.Models
{
    public class PastaPrincipalModel
    {
        public int Id { get; set; }
        public string NomePastaPrincipal { get; set; } = null!;
        public string EmpresaContratante { get; set; } = null!;
        public ICollection<SubPastaModel> SubPastas { get; set; } = new List<SubPastaModel>();
    }

    public class SubPastaModel
    {
        public int Id { get; set; }
        public string NomeSubPasta { get; set; } = null!;
        public string NomePastaPrincipal { get; set; } = null!;
        public string EmpresaContratante { get; set; } = null!;
        public ICollection<DocumentoModel> Documentos { get; set; } = new List<DocumentoModel>();
    }
}
