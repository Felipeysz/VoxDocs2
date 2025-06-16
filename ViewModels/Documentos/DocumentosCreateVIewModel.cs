using VoxDocs.DTO;
using System.ComponentModel.DataAnnotations;

namespace VoxDocs.Models.ViewModels
{
    public class DocumentoCreateViewModel
    {
        [Required(ErrorMessage = "O arquivo é obrigatório")]
        public IFormFile Arquivo { get; set; }
        
        [Required(ErrorMessage = "A pasta principal é obrigatória")]
        public Guid SelectedPastaPrincipalId { get; set; }
        
        [Required(ErrorMessage = "A subpasta é obrigatória")]
        public Guid SelectedSubPastaId { get; set; }
        
        [Required(ErrorMessage = "O nível de segurança é obrigatório")]
        public string NivelSeguranca { get; set; }
        
        public string? TokenSeguranca { get; set; }
        
        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(500, ErrorMessage = "A descrição não pode exceder 500 caracteres")]
        public string Descricao { get; set; }
    }
}