using System.ComponentModel.DataAnnotations;

namespace VoxDocs.Models
{
    public class DocumentoModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string NomeArquivo { get; set; }
        public string? UrlArquivo { get; set; }
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
    
    public class DocumentoEstatisticas
    {
        public string EmpresaContratante { get; set; }
        public int QuantidadeDocumentos { get; set; }
        public double TamanhoTotalGb { get; set; }
        
        // Pode adicionar métodos de negócio relacionados se necessário
        public string GetResumo()
        {
            return $"{EmpresaContratante}: {QuantidadeDocumentos} docs, {TamanhoTotalGb:N2} GB";
        }
    }
}