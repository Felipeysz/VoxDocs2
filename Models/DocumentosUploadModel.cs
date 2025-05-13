using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoxDocs.Models
{
        public class DocumentoUploadModel
    {
        public int Id { get; set; }
        public required string NomeArquivo { get; set; }
        public required string UrlArquivo { get; set; }
        public required string UsuarioCriador { get; set; }
        public DateTime DataCriacao { get; set; }
        public required string UsuarioUltimaAlteracao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
    }
}