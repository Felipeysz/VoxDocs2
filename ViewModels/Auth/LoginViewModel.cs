// ViewModels/Auth/LoginViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace VoxDocs.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [Display(Name = "Usuário")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Display(Name = "Lembrar-me")]
        public bool RememberMe { get; set; }
    }

    // ViewModels/Auth/ForgotPasswordViewModel.cs
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
    }

    // ViewModels/Auth/ResetPasswordViewModel.cs
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "Nova senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nova Senha")]
        [Compare("NewPassword", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmNewPassword { get; set; }
    }
}