// ConfiguracaoDocumentos.cs
using System.ComponentModel.DataAnnotations;

namespace VoxDocs.Models
{
    public class ConfiguracaoDocumentosModel
    {
        [Key]
        public int Id { get; set; }
        public bool PermitirPDF { get; set; } = true;
        public bool PermitirWord { get; set; } = true;
        public bool PermitirExcel { get; set; } = true;
        public bool PermitirImagens { get; set; } = true;
        public int TamanhoMaximoMB { get; set; } = 10;
        public int DiasArmazenamentoTemporario { get; set; } = 7;
    }
}

