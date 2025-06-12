using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using VoxDocs.DTO;

namespace VoxDocs.DTO
{
    public class CriarPlanoNomeDto
    {
        public required string NomePlano { get; set; }
        public required string Periodicidade { get; set; }
    }

    public class FinalizarPagamentoDto
    {
        // --- PAGAMENTO ---
        public required string Id { get; set; } = null!;
        public required string MetodoPagamento { get; set; } = null!;
        public string? EmpresaContratantePlano { get; set; }
        public required DateTime DataPagamento { get; set; }
        public required string NomePlano { get; set; }
        public required string Periodicidade { get; set; }

        public required DTOEmpresasContratante EmpresaContratante { get; set; } = new DTOEmpresasContratante
        {
            EmpresaContratante = string.Empty,
            Email = string.Empty
        };

        public required List<DTOPastaPrincipalCreate> PastasPrincipais { get; set; } = new();
        public required List<DTOSubPastaCreate> SubPastas { get; set; } = new();
        public required List<DTORegisterUser> Usuarios { get; set; } = new();
    }

    public class TokenRequestDto
    {
        public required string Token { get; set; }
    }

    public class TokenResponseDto
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public int CodigoStatus { get; set; } = (int)HttpStatusCode.OK; // Valor padrão
    }

    public class PagamentoResponseDto
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string DetalhesErro { get; set; } = string.Empty;
        // Adicione outras propriedades conforme necessário
    }
        public class PlanoInfoDto
    {
        public string NomePlano { get; set; } = string.Empty;
        public string Periodicidade { get; set; } = string.Empty;
    }
}
