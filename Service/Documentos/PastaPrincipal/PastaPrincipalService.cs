using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Services
{

    public class PastaPrincipalService : IPastaPrincipalService
    {
        private readonly IPastaPrincipalRepository _repository;

        public PastaPrincipalService(IPastaPrincipalRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<DTOPastaPrincipal>> GetAllAsync()
        {
            var pastas = await _repository.GetAllAsync();

            return pastas.Select(p => new DTOPastaPrincipal
            {
                Id = p.Id,
                NomePastaPrincipal = p.NomePastaPrincipal,
                EmpresaContratante = p.EmpresaContratante,
                Quantidade = p.SubPastas?.Count ?? 0
            });
        }

        public async Task<DTOPastaPrincipal> GetByNamePrincipalAsync(string nomePasta)
        {
            var model = await _repository.GetByNamePrincipalAsync(nomePasta);
            return model is null 
                ? null 
                : new DTOPastaPrincipal {
                    Id = model.Id,
                    NomePastaPrincipal = model.NomePastaPrincipal,
                };
        }

        public async Task<IEnumerable<DTOPastaPrincipal>> GetByEmpresaAsync(string empresaContratante)
        {
            var pastas = await _repository.GetByEmpresaAsync(empresaContratante);

            return pastas.Select(p => new DTOPastaPrincipal
            {
                Id = p.Id,
                NomePastaPrincipal = p.NomePastaPrincipal,
                EmpresaContratante = p.EmpresaContratante,
                Quantidade = p.SubPastas?.Count ?? 0
            });
        }

        public async Task<DTOPastaPrincipal?> GetByIdAsync(int id)
        {
            var pasta = await _repository.GetByIdAsync(id);
            if (pasta == null) return null;

            return new DTOPastaPrincipal
            {
                Id = pasta.Id,
                NomePastaPrincipal = pasta.NomePastaPrincipal,
                EmpresaContratante = pasta.EmpresaContratante
            };
        }

        public async Task<DTOPastaPrincipal> CreateAsync(DTOPastaPrincipalCreate dto)
        {
            var pasta = new PastaPrincipalModel
            {
                NomePastaPrincipal = dto.NomePastaPrincipal,
                EmpresaContratante = dto.EmpresaContratante
            };

            var createdPasta = await _repository.CreateAsync(pasta);

            return new DTOPastaPrincipal
            {
                Id = createdPasta.Id,
                NomePastaPrincipal = createdPasta.NomePastaPrincipal,
                EmpresaContratante = createdPasta.EmpresaContratante
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}