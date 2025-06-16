// BusinessRules/PlanosVoxDocsBusinessRules.cs
using VoxDocs.DTO;
using VoxDocs.Interfaces;
using VoxDocs.Models;

namespace VoxDocs.BusinessRules
{

    public class PlanosVoxDocsBusinessRules : IPlanosVoxDocsBusinessRules
    {
        private readonly IPlanosVoxDocsRepository _repository;

        public PlanosVoxDocsBusinessRules(IPlanosVoxDocsRepository repository)
        {
            _repository = repository;
        }

        public async Task<(PlanosVoxDocsModel? plan, string? error)> GetPlanByNameAndPeriodicidadeWithValidationAsync(string nome, string periodicidade)
        {
            if (string.IsNullOrWhiteSpace(nome))
                return (null, "Nome do plano não pode ser vazio");
            
            if (string.IsNullOrWhiteSpace(periodicidade))
                return (null, "Periodicidade não pode ser vazia");

            var plan = await _repository.GetPlanByNameAndPeriodicidadeAsync(nome, periodicidade);
            return plan == null 
                ? (null, "Plano não encontrado") 
                : (plan, null);
        }

        public async Task<(int? limiteAdmin, int? limiteUsuario, string? error)> ObterLimitesPlanoAsync(int planoId)
        {
            if (planoId <= 0)
                return (null, null, "ID do plano inválido");

            try
            {
                var plan = await _repository.GetPlanByIdAsync(planoId);
                if (plan == null)
                    return (null, null, "Plano não encontrado");

                return (plan.LimiteAdmin, plan.LimiteUsuario, null);
            }
            catch (Exception ex)
            {
                return (null, null, $"Erro ao obter limites do plano: {ex.Message}");
            }
        }

        public async Task<(List<PlanosVoxDocsModel> plans, string? error)> GetAllPlansWithValidationAsync()
        {
            var plans = await _repository.GetAllPlansAsync();
            return plans.Count == 0
                ? (new List<PlanosVoxDocsModel>(), "Nenhum plano encontrado")
                : (plans, null);
        }

        public async Task<(List<PlanosVoxDocsModel> plans, string? error)> GetPlansByCategoryWithValidationAsync(string categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria))
                return (new List<PlanosVoxDocsModel>(), "Categoria não pode ser vazia");

            var plans = await _repository.GetPlansByCategoryAsync(categoria);
            return plans.Count == 0
                ? (new List<PlanosVoxDocsModel>(), "Nenhum plano encontrado para esta categoria")
                : (plans, null);
        }

        public async Task<(PlanosVoxDocsModel? plan, string? error)> GetPlanByIdWithValidationAsync(int id)
        {
            if (id <= 0)
                return (null, "ID inválido");

            try
            {
                var plan = await _repository.GetPlanByIdAsync(id);
                return (plan, null);
            }
            catch (KeyNotFoundException)
            {
                return (null, "Plano não encontrado");
            }
        }

        public async Task<(PlanosVoxDocsModel? plan, string? error)> CreatePlanWithValidationAsync(DTOPlanosVoxDocs dto)
        {
            if (dto.Nome.ToLower() != "gratuito" && dto.Nome.ToLower() != "premium")
                return (null, "Somente os planos 'Gratuito' e 'Premium' são permitidos");

            if (dto.Preco <= 0)
                return (null, "Preço deve ser maior que zero");

            if (dto.Duracao <= 0)
                return (null, "Duração deve ser maior que zero");

            decimal finalPrice = dto.Preco;
            if (dto.Nome.ToLower() == "premium")
            {
                if (dto.Duracao == 6)
                    finalPrice = Math.Round(dto.Preco * 6 * 0.9m, 2);
                else if (dto.Duracao == 12)
                    finalPrice = Math.Round(dto.Preco * 12 * 0.8m, 2);
                else if (dto.Duracao != 1)
                    return (null, "Duração inválida para plano Premium");
            }

            var plan = new PlanosVoxDocsModel
            {
                Id = Guid.NewGuid(),
                Nome = dto.Nome,
                Descriçao = dto.Descricao,
                Preco = finalPrice,
                Duracao = dto.Duracao,
                Periodicidade = dto.Periodicidade,
                ArmazenamentoDisponivel = dto.ArmazenamentoDisponivel,
                LimiteAdmin = dto.LimiteAdmin ?? 0,
                LimiteUsuario = dto.LimiteUsuario ?? 0
            };

            var createdPlan = await _repository.CreatePlanAsync(plan);
            return (createdPlan, null);
        }

        public async Task<(PlanosVoxDocsModel? plan, string? error)> UpdatePlanWithValidationAsync(int id, DTOPlanosVoxDocs dto)
        {
            var existingPlan = await GetPlanByIdWithValidationAsync(id);
            if (existingPlan.error != null)
                return (null, existingPlan.error);

            if (dto.Nome.ToLower() != "gratuito" && dto.Nome.ToLower() != "premium")
                return (null, "Somente os planos 'Gratuito' e 'Premium' são permitidos");

            existingPlan.plan!.Nome = dto.Nome;
            existingPlan.plan.Descriçao = dto.Descricao;
            existingPlan.plan.Preco = dto.Preco;
            existingPlan.plan.Duracao = dto.Duracao;
            existingPlan.plan.Periodicidade = dto.Periodicidade;
            existingPlan.plan.ArmazenamentoDisponivel = dto.ArmazenamentoDisponivel;
            existingPlan.plan.LimiteAdmin = dto.LimiteAdmin ?? 0;
            existingPlan.plan.LimiteUsuario = dto.LimiteUsuario ?? 0;

            var updatedPlan = await _repository.UpdatePlanAsync(existingPlan.plan);
            return (updatedPlan, null);
        }

        public async Task<string?> DeletePlanWithValidationAsync(int id)
        {
            var existingPlan = await GetPlanByIdWithValidationAsync(id);
            if (existingPlan.error != null)
                return existingPlan.error;

            await _repository.DeletePlanAsync(id);
            return null;
        }

        public async Task<(PlanosVoxDocsModel? plan, string? error)> GetPlanByNameWithValidationAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return (null, "Nome não pode ser vazio");

            var plan = await _repository.GetPlanByNameAsync(name);
            return plan == null
                ? (null, "Plano não encontrado")
                : (plan, null);
        }
    }
}