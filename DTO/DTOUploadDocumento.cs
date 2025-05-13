using System;

namespace VoxDocs.DTO
{
    public class DTOUploadDocumento
    {
        public int Id { get; set; }
        public string NomeArquivo { get; set; }
        public string UrlArquivo { get; set; }
        public string UsuarioCriador { get; set; }
        public DateTime DataCriacao { get; set; }
        public string UsuarioUltimaAlteracao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
    }
    public class UploadDocumentoDto
    {
        public int AreaId { get; set; }
        public int TipoId { get; set; }
        public string Usuario { get; set; }
        public string Descricao { get; set; }
        public IFormFile File { get; set; }
    }
}
