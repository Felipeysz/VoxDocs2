using System;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace VoxDocs.DTO
{
    // DTO para criar solicitação de pagamento
    public class CriarPlanoDto
    {
        public Guid PagamentoId { get; set; } = Guid.NewGuid();
        public string? EmpresaContratante { get; set; } = null;
        public string? EmailEmpresaContratante { get; set; } = null;
        public string MetodoPagamento { get; set; } = "PENDENTE";
        public required DateTime DataPagamento { get; set; } = DateTime.Now;
        public required DateTime? DataExpiracao { get; set; } = null;
        public string StatusEmpresa { get; set; } = "Plano Pendente";
        public required string nomePlano { get; set; }
        public required string periodicidade { get; set; }
        public required decimal valorPlano { get; set; }

    }

    // DTO para confirmar pagamento
    public class CriarCadastroPagamentoPlanoDto
    {
        public Guid PagamentoId { get; }
        public required string EmpresaContratante { get; set; }
        public required string EmailEmpresaContratante { get; set; }
        public required string MetodoPagamento { get; set; }
        public DateTime DataPagamento { get; }
        public required DateTime DataExpiracao { get; set; } = DateTime.MaxValue;
        public required string StatusEmpresa { get; set; } = "Plano Ativo";
        public required string nomePlano { get; set; }
        public required string periodicidade { get; set; }
        public required decimal valorPlano { get; set; }
        public List<PastaConfigurationDto> Pastas { get; set; }
        public List<UsuarioCadastroDto> AdminUsuarios { get; set; } = new List<UsuarioCadastroDto>();

        public List<UsuarioCadastroDto> UsuariosComum { get; set; } = new List<UsuarioCadastroDto>();
    }

    public class UsuarioCadastroDto
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public required string PermissaoConta { get; set; } = "user";
        public required string EmpresaContratante { get; set; }
        public string? PlanoPago { get; set; }
    }
    public enum PermissaoConta
    {
        Admin,
        User
    }

    public class PastaConfigurationDto
    {
        public string NomePastaPrincipal { get; set; }
        public List<SubpastaConfigurationDto> Subpastas { get; set; }
    }

    public class SubpastaConfigurationDto
    {
        public string NomeSubPasta { get; set; }
    }  
    // DTO de resposta
    public class PagamentoResponseDto
    {
        public bool Sucesso { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Mensagem { get; set; }
        public object Dados { get; set; }

        public static PagamentoResponseDto Ok(object dados = null, string mensagem = null) => new()
        {
            Sucesso = true,
            StatusCode = HttpStatusCode.OK,
            Mensagem = mensagem ?? "Operação realizada com sucesso",
            Dados = dados
        };

        public static PagamentoResponseDto Falha(string erro, HttpStatusCode status = HttpStatusCode.BadRequest, string mensagem = null, string detalhes = null, HttpStatusCode StatusCode = default, string? erros = null) => new()
        {
            Sucesso = false,
            StatusCode = status,
            Mensagem = erro
        };

        internal static object? Falha(string mensagem, HttpStatusCode StatusCode)
        {
            throw new NotImplementedException();
        }
    }
}