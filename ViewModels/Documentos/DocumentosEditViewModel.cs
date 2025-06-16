using VoxDocs.DTO;
using System.ComponentModel.DataAnnotations;

namespace VoxDocs.Models.ViewModels
{
    public class DocumentoEditViewModel
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "A descrição é obrigatória")]
        public string Descricao { get; set; }
        
        [Required(ErrorMessage = "O nível de segurança é obrigatório")]
        public NivelSeguranca NivelSeguranca { get; set; }
        
        public string? TokenSeguranca { get; set; }
        public IFormFile? NovoArquivo { get; set; }
        public string NomeArquivoAtual { get; set; }
    }
}