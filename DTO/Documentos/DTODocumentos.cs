using Microsoft.AspNetCore.Http;
using System;

namespace VoxDocs.DTO
{
    public class DTODocumentoCreate
    {
        public int Id { get; set; }
        public string NomeArquivo { get; set; }
        public string UrlArquivo { get; set; }
        public string UsuarioCriador { get; set; }
        public DateTime DataCriacao { get; set; }
        public string UsuarioUltimaAlteracao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public string Empresa { get; set; }
        public string NomePastaPrincipal { get; set; }
        public string NomeSubPasta { get; set; }
        public long TamanhoArquivo { get; set; }
        public string NivelSeguranca { get; set; }
        public string? TokenSeguranca { get; set; }
        public string Descrição { get; set; }
    }

    public class DocumentoDto
    {
        public IFormFile Arquivo { get; set; }
        public string Usuario { get; set; }
        public string Empresa { get; set; }
        public string NomePastaPrincipal { get; set; }
        public string NomeSubPasta { get; set; }
        public string NivelSeguranca { get; set; }
        public string? TokenSeguranca { get; set; }
        public string Descrição { get; set; }
    }

    public class DTOQuantidadeDocumentoEmpresa
    {
        public string NomeEmpresa { get; set; }
        public int Quantidade { get; set; }
        public double TamanhoTotalGb { get; set; }
    }

    public class DTOAcessosDocumento
    {
        public string NomeArquivo { get; set; }
        public string NomeSubPasta { get; set; }
        public string NomePastaPrincipal { get; set; }
        public int QuantidadeAcessos { get; set; }
    }
}