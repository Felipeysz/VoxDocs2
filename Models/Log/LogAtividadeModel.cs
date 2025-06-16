// LogAtividadeModel.cs em VoxDocs.Models
using System;
using System.ComponentModel.DataAnnotations;

namespace VoxDocs.Models
{
    public class LogAtividadeModel
    {
        [Key]
        public Guid Id { get; set; }
        
        public Guid DocumentoId { get; set; }
        public Guid usuarioId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Usuario { get; set; }
        
        [Required]
        [StringLength(50)]
        public string TipoAcao { get; set; }
        
        [Required]
        public DateTime DataHora { get; set; }
        
        public string Detalhes { get; set; }
        
        [StringLength(50)]
        public string IpAddress { get; set; }
    }
}