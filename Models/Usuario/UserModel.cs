using System.ComponentModel.DataAnnotations;

namespace VoxDocs.Models
{
    public class UserModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Usuario { get; set; }
        public required string Email { get; set; }
        public required string Senha { get; set; }
        public required string PermissionAccount { get; set; }
        public required string EmpresaContratante { get; set; }
        public string? PlanoPago { get; set; }
        public string? LimiteUsuario { get; set; }
        public string? LimiteAdmin { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiration { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? UltimoLogin { get; set; }
        public bool Ativo { get; set; } = true;
    }

    public class ArmazenamentoUsuarioModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public int UsoArmazenamento { get; set; }
        public int LimiteArmazenamento { get; set; }
    }
}