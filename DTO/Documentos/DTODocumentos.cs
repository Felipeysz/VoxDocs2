using Microsoft.AspNetCore.Http;
using System;

namespace VoxDocs.DTO
{
    public enum NivelSeguranca
    {
        Publico,
        Restrito,
        Confidencial
    }

    // DTO base para Documento (para operações de leitura)
    public class DocumentoDto
    {
        public Guid Id { get; set; }
        public string NomeArquivo { get; set; }
        public string UrlArquivo { get; set; }
        public string UsuarioCriador { get; set; }
        public DateTime DataCriacao { get; set; }
        public string UsuarioUltimaAlteracao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public string EmpresaContratante { get; set; }
        public string NomePastaPrincipal { get; set; }
        public string NomeSubPasta { get; set; }
        public long TamanhoArquivo { get; set; }
        public NivelSeguranca NivelSeguranca { get; set; }
        public string Descricao { get; set; }
    }

    // DTO para criação de documento
    public class DocumentoCriacaoDto
    {
        public IFormFile Arquivo { get; set; }
        public string NomePastaPrincipal { get; set; }
        public string NomeSubPasta { get; set; }
        public NivelSeguranca NivelSeguranca { get; set; } = NivelSeguranca.Publico;
        public string TokenSeguranca { get; set; }
        public string Descricao { get; set; }
        
        // Esses campos podem ser preenchidos automaticamente pelo serviço
        public string Usuario { get; set; }
        public string EmpresaContratante { get; set; }
    }

    // DTO para atualização de documento
    public class DocumentoAtualizacaoDto
    {
        public Guid Id { get; set; }

        public IFormFile? NovoArquivo { get; set; }
        public string UsuarioUltimaAlteracao { get; set; }
        public string? Descricao { get; set; }
        public NivelSeguranca? NivelSeguranca { get; set; }
        public string? TokenSeguranca { get; set; }
    }

    // DTO para resposta detalhada (incluindo dados sensíveis para admin)
    public class DocumentoDetalhesDto : DocumentoDto
    {
        public string TokenSeguranca { get; set; } // Apenas para usuários autorizados
    }

    // DTO para estatísticas
    public class DocumentoEstatisticasDto
    {
        public string EmpresaContratante { get; set; }
        public int QuantidadeDocumentos { get; set; }
        public double TamanhoTotalGb { get; set; }
        public int Publicos { get; set; }
        public int Restritos { get; set; }
        public int Confidenciais { get; set; }
    }
    public class ResultadoOperacaoDto
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; }
        public object? Dados { get; set; }
    }
}