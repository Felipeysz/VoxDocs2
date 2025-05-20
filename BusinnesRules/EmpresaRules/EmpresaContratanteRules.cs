using System;
using VoxDocs.DTO;

namespace VoxDocs.BusinessRules
{
    public class EmpresasContratanteRules
    {
        public void Validate(DTOEmpresasContratante dto)
        {
            if (dto == null)
                throw new ArgumentException("Dados da empresa não informados.");
            if (string.IsNullOrWhiteSpace(dto.EmpresaContratante))
                throw new ArgumentException("Nome da empresa é obrigatório.");
            // Adicione outras validações conforme necessário
        }
    }
}