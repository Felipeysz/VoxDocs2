using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace VoxDocs.Models
{
    public class AreasDocumentoModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }
        public ICollection<DocumentoModel> Documentos { get; set; }
    }
}