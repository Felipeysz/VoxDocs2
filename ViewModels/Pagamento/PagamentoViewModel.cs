using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VoxDocs.Models
{
    public class CriarCadastroPagamentoViewModel
    {
        // Dados do Plano
        public int CurrentStep { get; set; } = 1;
        public string Token { get; set; }
        public string NomePlano { get; set; }
        public int LimiteUsuarios { get; set; }
        public int LimiteAdministradores { get; set; } = 1; // Valor padrão
        public string Periodicidade { get; set; } // Mensal, Anual, etc.
        public decimal ValorPlano { get; set; }
        public string EmpresaContratante { get; set; }
        public string EmailContato { get; set; }
        public string StatusEmpresa { get; set; } = "Plano Ativo";
        public DateTime? DataExpiracao { get; set; }
        
        // Dados de Pagamento
        [Required(ErrorMessage = "O método de pagamento é obrigatório")]
        [Display(Name = "Método de Pagamento")]
        public string MetodoPagamentoSelecionado { get; set; }
        public List<MetodoPagamentoViewModel> MetodosPagamento { get; set; } = new List<MetodoPagamentoViewModel>();
        
        // Dados do Pagamento (dependendo do método escolhido)
        public string DadosPagamento { get; set; }
        
        // Dados da Empresa
        public DadosEmpresaViewModel DadosEmpresa { get; set; } = new DadosEmpresaViewModel();
        
        // Dados do Usuário Admin
        public UsuarioAdminViewModel DadosUsuarioAdmin { get; set; } = new UsuarioAdminViewModel();

        // Estruturas de pastas e usuários
        public List<PastaPrincipalViewModel> PastasPrincipais { get; set; } = new List<PastaPrincipalViewModel>();
        public List<SubPastaViewModel> SubPastas { get; set; } = new List<SubPastaViewModel>();
        public List<UsuarioViewModel> Usuarios { get; set; } = new List<UsuarioViewModel>();
    }

    // Classes auxiliares
    public class MetodoPagamentoViewModel
    {
        public string Valor { get; set; }
        public string Texto { get; set; }
    }

    public class DadosEmpresaViewModel
    {
        [Required(ErrorMessage = "O nome da empresa é obrigatório")]
        public string Nome { get; set; }
        
        [Required(ErrorMessage = "O CNPJ é obrigatório")]
        public string EmailEmpresaContratante { get; set; }
    }

    public class UsuarioAdminViewModel
    {
        [Required(ErrorMessage = "O nome do usuário é obrigatório")]
        public string Nome { get; set; }
        
        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "A senha é obrigatória")]
        [DataType(DataType.Password)]
        public string Senha { get; set; }
        
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmarSenha { get; set; }
    }

    public class PastaPrincipalViewModel
    {
        public string NomePastaPrincipal { get; set; }
        public List<SubPastaViewModel> SubPastas { get; set; } = new List<SubPastaViewModel>();
    }

    public class SubPastaViewModel
    {
        public string NomeSubPasta { get; set; }
        public string NomePastaPrincipal { get; set; } // Referência à pasta principal
    }

    public class UsuarioViewModel
    {
        public string Usuario { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string PermissaoConta { get; set; } // Admin ou User
    }

    public class ErrorViewModel
    {
        public string Message { get; set; }
        public bool ShowRetry { get; set; }
    }
}