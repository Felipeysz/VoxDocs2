namespace VoxDocs.DTO
{
    public class DTOUser
    {
        public required string Usuario { get; set; }
        public required string Senha { get; set; }
        public required string PermissionAccount { get; set; }
    }

    public class DTOUserLogin
    {
        public required string Usuario { get; set; }
        public required string Senha { get; set; }
    }

        public class DTOUserPermission
    {
        public required string Usuario { get; set; }
        public required string PermissionAccount { get; set; }
    }

        public class DTOActiveToken
    {
        public required string Usuario { get; set; }
        public required string Token { get; set; }
        public required string TempoRestante { get; set; }
    }
}