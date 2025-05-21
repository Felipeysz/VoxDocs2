using System.ComponentModel.DataAnnotations;

namespace VoxDocs.Models
{
    public class DocumentoModel
    {
        public int Id { get; set; }
        public required string NomeArquivo { get; set; }
        public required string UrlArquivo { get; set; }
        public required string UsuarioCriador { get; set; }
        public DateTime DataCriacao { get; set; }
        public required string UsuarioUltimaAlteracao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public required string Empresa { get; set; }
        public required string NomePastaPrincipal { get; set; }
        public required string NomeSubPasta { get; set; }
        public long TamanhoArquivo { get; set; }
        public required string NivelSeguranca { get; set; }
        public int ContadorAcessos { get; set; } = 0;
        public string? TokenSeguranca { get; set; } // Nullable for public documents
        public string Descrição { get; set; }
    }
}