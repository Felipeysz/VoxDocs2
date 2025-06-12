namespace VoxDocs.DTO
{
    public class DTOUserInfo
    {
        public required string Usuario { get; set; }
        public required string Email { get; set; }
        public required string PermissionAccount { get; set; } // "admin" ou "user"
        public required string EmpresaContratante { get; set; }
        public required string Plano { get; set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public bool? Ativo { get; set; }
    }
    public class DTORegisterUser
    {
        public required string Usuario { get; set; }
        public required string Email { get; set; }
        public required string Senha { get; set; }
        public required string PermissionAccount { get; set; }
        public required string EmpresaContratante { get; set; }
        public string? PlanoPago { get; set; }
    }
    public class DTOLoginUser
    {
        public required string Usuario { get; set; }
        public required string Senha { get; set; }
    }

    // DTO para atualização de usuário
    public class DTOUpdateUser
    {
        public required string Usuario { get; set; }
        public required string Email { get; set; }
        public string? EmpresaContratante { get; set; }
        public string? PlanoPago { get; set; }
        public string? LimiteUsuario { get; set; }
        public string? LimiteAdmin { get; set; }
        public required string PermissionAccount { get; set; }
    }

    // DTO para reset de senha
    public class DTOResetPassword
    {
        public required string Email { get; set; }
    }

    // DTO para redefinir senha com token (pós-reset)
    public class DTOResetPasswordWithToken
    {
        public required string Token { get; set; }
        public required string NovaSenha { get; set; }
    }

    // DTO para alteração de senha com senha atual
    public class DTOUserLoginPasswordChange
    {
        public required string Usuario { get; set; }
        public required string SenhaAntiga { get; set; }
        public required string NovaSenha { get; set; }
    }

    // DTO de resposta de erro
    public class ErrorResponse
    {
        public required string Mensagem { get; set; }
        public required string Detalhes { get; set; }
    }

    public class AdminStatsDTO
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public List<DTOUserInfo> RecentUsers { get; set; }
    }

    public class UserStorageDTO
    {
        public int StorageUsage { get; set; }
        public int StorageLimit { get; set; }
    }
}
