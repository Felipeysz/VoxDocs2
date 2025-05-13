using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoxDocs.Models
{
    public class DocumentoModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Descricao { get; set; }

        [ForeignKey("TipoDocumento")]
        public int TipoDocumentoId { get; set; }
        public TipoDocumentoModel TipoDocumento { get; set; }

        [ForeignKey("AreaDocumento")]
        public int AreaDocumentoId { get; set; }
        public AreasDocumentoModel AreaDocumento { get; set; }

        // Relacionamento com upload
        [ForeignKey("DocumentoUpload")]
        public int DocumentoUploadId { get; set; }
        public DocumentoUploadModel DocumentoUpload { get; set; }

        // Datas e usuários para controle dinâmico
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public string UsuarioCriador { get; set; }
        public string UsuarioUltimaAlteracao { get; set; }
    }
}