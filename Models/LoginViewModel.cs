using System.ComponentModel.DataAnnotations;

namespace VoxDocs.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Usuário")]
        public required string Usuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public required string Senha { get; set; }
    }
}
