using System.ComponentModel.DataAnnotations;

namespace VoxDocs.Models
{
    public class EmpresasContratanteModel
    {
        [Key]
         public Guid Id { get; set; } = Guid.NewGuid();
        public required string EmpresaContratante { get; set; }
        public required string Email { get; set; }
    }
}