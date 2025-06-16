namespace VoxDocs.DTO
{
    public class DTOUsuarioInfo
    {
        public Guid Id { get; set; }
        public required string Usuario { get; set; }
        public required string Email { get; set; }
        public required string PermissaoConta { get; set; } // "admin" ou "user"
        public required string EmpresaContratante { get; set; }
        public string? Plano { get; set; }
        public string? LimiteAdmin { get; set; }
        public string? LimiteUsuario { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public bool Ativo { get; set; }
    }

    public class DTORegistrarUsuario
    {
        public required string Usuario { get; set; }
        public required string Email { get; set; }
        public required string Senha { get; set; }
        public required string PermissaoConta { get; set; }
        public required string EmpresaContratante { get; set; }
        public string? PlanoPago { get; set; }
    }

    public class DTOLoginUsuario
    {
        public required string Usuario { get; set; }
        public required string Senha { get; set; }
    }

    public class DTOAtualizarUsuario
    {
        public Guid IdUser { get; set; }
        public required string Usuario { get; set; }
        public required string Email { get; set; }
        public string? EmpresaContratante { get; set; }
        public string? PlanoPago { get; set; }
        public string? LimiteUsuario { get; set; }
        public string? LimiteAdmin { get; set; }
        public required string PermissaoConta { get; set; }
        public bool Ativo { get; set; }
    }

}