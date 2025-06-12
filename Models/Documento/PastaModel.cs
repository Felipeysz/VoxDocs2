using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace VoxDocs.Models
{
    public class PastaPrincipalModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string NomePastaPrincipal { get; set; }
        public required string EmpresaContratante { get; set; }
        public ICollection<SubPastaModel> SubPastas { get; set; } = new List<SubPastaModel>();
    }

    public class SubPastaModel
    {
         public Guid Id { get; set; } = Guid.NewGuid();
        public required string NomeSubPasta { get; set; }
        public required string NomePastaPrincipal { get; set; }
        public required string EmpresaContratante { get; set; }
        public ICollection<DocumentoModel> Documentos { get; set; } = new List<DocumentoModel>();
    }
}
