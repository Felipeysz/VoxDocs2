using System.ComponentModel.DataAnnotations;

namespace VoxDocs.Models
{
    public class DocumentoModel
    {
         public required Guid Id { get; set; } = Guid.NewGuid();
        public required string NomeArquivo { get; set; }
        public required string UrlArquivo { get; set; }
        public required string UsuarioCriador { get; set; }
        public required DateTime DataCriacao { get; set; }
        public string? UsuarioUltimaAlteracao { get; set; }
        public DateTime? DataUltimaAlteracao { get; set; }
        public required string Empresa { get; set; }
        public required string NomePastaPrincipal { get; set; }
        public required string NomeSubPasta { get; set; }
        public required long TamanhoArquivo { get; set; }
        public required string NivelSeguranca { get; set; }
        public int ContadorAcessos { get; set; } = 0;
        public string? TokenSeguranca { get; set; }
        public required string Descrição { get; set; }
    }
}