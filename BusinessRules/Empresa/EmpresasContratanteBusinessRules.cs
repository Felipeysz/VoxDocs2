using VoxDocs.Data.Repositories;
using VoxDocs.Models;
using System.Net;

namespace VoxDocs.BusinessRules
{
    public class EmpresasContratanteBusinessRules : IEmpresasContratanteBusinessRules
    {
        private readonly IEmpresasContratanteRepository _repository;

        public EmpresasContratanteBusinessRules(IEmpresasContratanteRepository repository)
        {
            _repository = repository;
        }

        public async Task<ValidationResult<List<EmpresasContratanteModel>>> ValidarGetAllAsync()
        {
            try
            {
                var empresas = await _repository.GetAllAsync();
                
                if (empresas == null || !empresas.Any())
                {
                    return ValidationResult<List<EmpresasContratanteModel>>.Error(
                        "Nenhuma empresa contratante encontrada.", 
                        HttpStatusCode.NotFound);
                }

                return ValidationResult<List<EmpresasContratanteModel>>.Success(empresas);
            }
            catch (Exception ex)
            {
                return ValidationResult<List<EmpresasContratanteModel>>.Error(
                    $"Erro ao obter empresas: {ex.Message}", 
                    HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ValidationResult<EmpresasContratanteModel>> ValidarGetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return ValidationResult<EmpresasContratanteModel>.Error(
                    "ID da empresa não pode ser vazio.", 
                    HttpStatusCode.BadRequest);
            }

            try
            {
                var empresa = await _repository.GetByIdAsync(id);
                
                if (empresa == null)
                {
                    return ValidationResult<EmpresasContratanteModel>.Error(
                        $"Empresa com ID {id} não encontrada.", 
                        HttpStatusCode.NotFound);
                }

                return ValidationResult<EmpresasContratanteModel>.Success(empresa);
            }
            catch (Exception ex)
            {
                return ValidationResult<EmpresasContratanteModel>.Error(
                    $"Erro ao obter empresa: {ex.Message}", 
                    HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ValidationResult<EmpresasContratanteModel>> ValidarGetByNomeAsync(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                return ValidationResult<EmpresasContratanteModel>.Error(
                    "Nome da empresa não pode ser vazio.", 
                    HttpStatusCode.BadRequest);
            }

            try
            {
                var empresa = await _repository.GetByNomeAsync(nome);
                
                if (empresa == null)
                {
                    return ValidationResult<EmpresasContratanteModel>.Error(
                        $"Empresa '{nome}' não encontrada.", 
                        HttpStatusCode.NotFound);
                }

                return ValidationResult<EmpresasContratanteModel>.Success(empresa);
            }
            catch (Exception ex)
            {
                return ValidationResult<EmpresasContratanteModel>.Error(
                    $"Erro ao obter empresa: {ex.Message}", 
                    HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ValidationResult<EmpresasContratanteModel>> ValidarCreateAsync(EmpresasContratanteModel empresa)
        {
            if (empresa == null)
            {
                return ValidationResult<EmpresasContratanteModel>.Error(
                    "Dados da empresa não podem ser nulos.", 
                    HttpStatusCode.BadRequest);
            }

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(empresa.EmpresaContratante))
                errors.Add("Nome da empresa é obrigatório.");

            if (string.IsNullOrWhiteSpace(empresa.Email))
                errors.Add("Email da empresa é obrigatório.");

            if (errors.Any())
            {
                return ValidationResult<EmpresasContratanteModel>.Error(
                    string.Join(" ", errors), 
                    HttpStatusCode.BadRequest);
            }

            try
            {
                var empresaExistente = await _repository.GetByNomeAsync(empresa.EmpresaContratante);
                if (empresaExistente != null)
                {
                    return ValidationResult<EmpresasContratanteModel>.Error(
                        $"Empresa '{empresa.EmpresaContratante}' já está cadastrada.", 
                        HttpStatusCode.Conflict);
                }

                var empresaCriada = await _repository.CreateAsync(empresa);
                return ValidationResult<EmpresasContratanteModel>.Success(empresaCriada);
            }
            catch (Exception ex)
            {
                return ValidationResult<EmpresasContratanteModel>.Error(
                    $"Erro ao criar empresa: {ex.Message}", 
                    HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ValidationResult<EmpresasContratanteModel>> ValidarUpdateAsync(EmpresasContratanteModel empresa)
        {
            if (empresa == null)
            {
                return ValidationResult<EmpresasContratanteModel>.Error(
                    "Dados da empresa não podem ser nulos.", 
                    HttpStatusCode.BadRequest);
            }

            if (empresa.Id == Guid.Empty)
            {
                return ValidationResult<EmpresasContratanteModel>.Error(
                    "ID da empresa não pode ser vazio.", 
                    HttpStatusCode.BadRequest);
            }

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(empresa.EmpresaContratante))
                errors.Add("Nome da empresa é obrigatório.");

            if (string.IsNullOrWhiteSpace(empresa.Email))
                errors.Add("Email da empresa é obrigatório.");

            if (errors.Any())
            {
                return ValidationResult<EmpresasContratanteModel>.Error(
                    string.Join(" ", errors), 
                    HttpStatusCode.BadRequest);
            }

            try
            {
                var empresaExistente = await _repository.GetByIdAsync(empresa.Id);
                if (empresaExistente == null)
                {
                    return ValidationResult<EmpresasContratanteModel>.Error(
                        $"Empresa com ID {empresa.Id} não encontrada.", 
                        HttpStatusCode.NotFound);
                }

                // Verifica se o novo nome já existe em outra empresa
                if (!string.Equals(empresaExistente.EmpresaContratante, empresa.EmpresaContratante, StringComparison.OrdinalIgnoreCase))
                {
                    var empresaComMesmoNome = await _repository.GetByNomeAsync(empresa.EmpresaContratante);
                    if (empresaComMesmoNome != null)
                    {
                        return ValidationResult<EmpresasContratanteModel>.Error(
                            $"Já existe uma empresa com o nome '{empresa.EmpresaContratante}'.", 
                            HttpStatusCode.Conflict);
                    }
                }

                var empresaAtualizada = await _repository.UpdateAsync(empresa);
                return ValidationResult<EmpresasContratanteModel>.Success(empresaAtualizada);
            }
            catch (Exception ex)
            {
                return ValidationResult<EmpresasContratanteModel>.Error(
                    $"Erro ao atualizar empresa: {ex.Message}", 
                    HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ValidationResult> ValidarDeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return ValidationResult.Error(
                    "ID da empresa não pode ser vazio.", 
                    HttpStatusCode.BadRequest);
            }

            try
            {
                var empresa = await _repository.GetByIdAsync(id);
                if (empresa == null)
                {
                    return ValidationResult.Error(
                        $"Empresa com ID {id} não encontrada.", 
                        HttpStatusCode.NotFound);
                }

                await _repository.DeleteAsync(id);
                return ValidationResult.Success();
            }
            catch (Exception ex)
            {
                return ValidationResult.Error(
                    $"Erro ao deletar empresa: {ex.Message}", 
                    HttpStatusCode.InternalServerError);
            }
        }
    }
}