namespace VoxDocs.DTO
{
    public class DTORedefinirSenha
    {
        public required string Email { get; set; }
    }

    public class DTORedefinirSenhaComToken
    {
        public required string Token { get; set; }
        public required string NovaSenha { get; set; }
    }

    public class DTOAlterarSenhaLogin
    {
        public required string Usuario { get; set; }
        public required string SenhaAntiga { get; set; }
        public required string NovaSenha { get; set; }
    }
}