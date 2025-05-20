using System.ComponentModel.DataAnnotations;

namespace VoxDocs.Models
{
    public class EmpresasContratanteModel
    {
        [Key]
        public int Id { get; set; }
        public required string EmpresaContratante { get; set; }
        public string? Email { get; set; }
    }
}