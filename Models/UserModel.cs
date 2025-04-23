using System.ComponentModel.DataAnnotations;

namespace VoxDocs.Models
{
    public class UserModel
    {
        [Key]
        [Required] // Ensures the field is not nullable
        public int Id { get; set; }

        [Required]
        public required string Usuario { get; set; }

        [Required]
        public required string Senha { get; set; }

        [Required]
        public required string PermissionAccount { get; set; }
    }
}