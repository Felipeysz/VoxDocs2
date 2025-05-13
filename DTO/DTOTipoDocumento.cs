using System.ComponentModel.DataAnnotations;

namespace VoxDocs.Models.Dto
{
    public class DTOTipoDocumento
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }
    }
}