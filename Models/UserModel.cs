using System.ComponentModel.DataAnnotations;

namespace VoxDocs.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        public required string Usuario { get; set; }
        public required string Email { get; set; }
        public required string Senha { get; set; }
        public required string PermissionAccount { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiration { get; set; }
    }
}