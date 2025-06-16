namespace VoxDocs.DTO
{
    public class DTOEmpresasContratante
    {
        public required string EmpresaContratante { get; set; }
        public required string Email { get; set; }
        public required string PlanoContratado { get; set; }
        public DateTime DataContratacao { get; set; } = DateTime.UtcNow;
    }
}
