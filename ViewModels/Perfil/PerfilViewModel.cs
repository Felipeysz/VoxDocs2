// ViewModels/Perfil/PerfilViewModel.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace VoxDocs.ViewModels
{
    public class PerfilViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        [Display(Name = "Usuário")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "Empresa Contratante")]
        public string EmpresaContratante { get; set; }

        [Display(Name = "Plano")]
        public string Plano { get; set; }

        [Display(Name = "Tipo de Conta")]
        public string PermissaoConta { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime DataCriacao { get; set; }

        [Display(Name = "Último Login")]
        public DateTime? UltimoLogin { get; set; }

        [Display(Name = "Status")]
        public bool Ativo { get; set; }
    }

    public class AlterarSenhaViewModel
    {
        [Required(ErrorMessage = "A senha atual é obrigatória")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha Atual")]
        public string SenhaAtual { get; set; }

        [Required(ErrorMessage = "A nova senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha")]
        public string NovaSenha { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nova Senha")]
        [Compare("NovaSenha", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmarNovaSenha { get; set; }
    }
}