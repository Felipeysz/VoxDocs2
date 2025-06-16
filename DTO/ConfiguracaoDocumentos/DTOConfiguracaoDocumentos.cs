// DTO para configuração de documentos

namespace VoxDocs.DTO
{
    public class DTOConfiguracaoDocumentos
    {
        public bool PermitirPDF { get; set; }
        public bool PermitirWord { get; set; }
        public bool PermitirExcel { get; set; }
        public bool PermitirImagens { get; set; }
        public int TamanhoMaximoMB { get; set; }
        public int DiasArmazenamentoTemporario { get; set; }
    }
}