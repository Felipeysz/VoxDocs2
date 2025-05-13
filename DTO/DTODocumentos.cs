using System;

namespace VoxDocs.DTO
{
    public class DTODocumento
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int AreaDocumentoId { get; set; }
        public int TipoDocumentoId { get; set; }
        public int DocumentoUploadId { get; set; }
        public string UsuarioCriador { get; set; }
        public DateTime DataCriacao { get; set; }
        public string UsuarioUltimaAlteracao { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }

    
}