using System;
using System.ComponentModel.DataAnnotations;

namespace VoxDocs.Models
{
    public class PagamentoConcluido
    {
        [Key]
        public Guid Id { get; set; }
        
        // Campos recebidos do CriarPlanoDto
        public string NomePlano { get; set; }
        public string PeriodicidadePlano { get; set; }
        public decimal ValorPlano { get; set; }
        
        // Campos com valores padrão (mesmos do DTO)
        public string MetodoPagamento { get; set; } = "PENDENTE";
        public DateTime DataPagamento { get; set; } = DateTime.Now;
        public string StatusEmpresa { get; set; } = "Plano Pendente";
        
        // Campos recebidos do ConfirmarPagamentoDto
        public string? EmpresaContratante { get; set; }
        public string? EmailContato { get; set; }
        
        // Campo calculado
        public DateTime DataExpiracao { get; set; }

        public void CalcularDataExpiracao()
        {
            DataExpiracao = PeriodicidadePlano switch
            {
                "Mensal" => DataPagamento.AddMonths(1),
                "Semestral" => DataPagamento.AddMonths(6),  // Adicionado suporte semestral
                "Anual" => DataPagamento.AddYears(1),
                "Ilimitado" => DateTime.MaxValue,
                _ => DataPagamento.AddMonths(1) // Default mensal
            };
        }
    }
}