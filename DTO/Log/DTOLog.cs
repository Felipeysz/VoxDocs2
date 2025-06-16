namespace VoxDocs.DTO
{
    public class LogAtividadeDto
    {
        public Guid Id { get; set; }
        public string Usuario { get; set; }
        public string Acao { get; set; }
        public DateTime DataHora { get; set; }
        public string Detalhes { get; set; }
        public string Ip { get; set; }
    }
}