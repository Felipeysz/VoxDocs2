using System;
using System.Collections.Generic;
using VoxDocs.DTO;

namespace VoxDocs.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Informações do Plano
        public string Plano { get; set; }
        public string ArmazenamentoTotal { get; set; }
        public string ArmazenamentoUsado { get; set; }
        public double PercentualUsoArmazenamento { get; set; }

        // Informações de Usuários
        public int UsuariosAtuais { get; set; }
        public int UsuariosPermitidos { get; set; }
        public int AdministradoresAtuais { get; set; }
        public int AdministradoresPermitidos { get; set; }

        // Tokens e Documentos
        public string TokensDisponiveis { get; set; }
        public int TokensUsados { get; set; }
        public int DocumentosEnviados { get; set; }
        public int ConsultasRealizadas { get; set; }

        // Pagamento
        public PagamentoResponseDto PagamentoInfo { get; set; }
        public DateTime? ProximaRenovacao { get; set; }

        // Timestamp
        public string UltimaAtualizacao { get; set; }

        // Listas para grids/tabelas
        public IEnumerable<DTOUsuarioInfo> UsuariosRecentes { get; set; }
        public IEnumerable<DTOEmpresasContratante> Empresas { get; set; }
        public int TotalEmpresas { get; set; }
        public int TotalPlanosAtivos { get; set; }
    }

    public class AdminUsuariosViewModel
    {
        public IEnumerable<DTOUsuarioInfo> Usuarios { get; set; }
        public DTOUsuarioInfo UsuarioSelecionado { get; set; }
        public IEnumerable<string> PlanosDisponiveis { get; set; }
        public IEnumerable<string> EmpresasDisponiveis { get; set; }
        public string? SenhaRegistro { get; set; }
        public string? ConfirmacaoSenha { get; set; }
    }

    public class AdminEmpresasViewModel
    {
        public IEnumerable<DTOEmpresasContratante> Empresas { get; set; }
        public DTOEmpresasContratante EmpresaSelecionada { get; set; }
    }

    public class AdminPlanosViewModel
    {
        public IEnumerable<DTOPlanosVoxDocs> Planos { get; set; }
        public DTOPlanosVoxDocs PlanoSelecionado { get; set; }
    }

    public class ConfiguracaoDocumentosViewModel
    {
        // Tipos de documentos permitidos
        public bool PermitirPDF { get; set; }
        public bool PermitirWord { get; set; }
        public bool PermitirExcel { get; set; }
        public bool PermitirImagens { get; set; }

        // Limites
        public int TamanhoMaximoMB { get; set; } = 10;
        public int DiasArmazenamentoTemporario { get; set; } = 30;
    }
}