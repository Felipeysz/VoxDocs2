namespace VoxDocs.DTO
{
    public class DTOEmpresasContratante
    {
        public required string EmpresaContratante { get; set; }
        public required string Email { get; set; }
    }

    public class DTOEmpresasContratantePlano
    {
        public required string EmpresaContratante { get; set; }
        public required string TipoPlano { get; set; }
        public int TotalConsultas { get; set; } // Adicionado
    }
}
