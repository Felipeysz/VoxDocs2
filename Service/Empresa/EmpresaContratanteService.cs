using Microsoft.EntityFrameworkCore;
using VoxDocs.Data;
using VoxDocs.DTO;
using VoxDocs.Models;

namespace VoxDocs.Services
{
    public class EmpresasContratanteService : IEmpresasContratanteService
    {
        private readonly VoxDocsContext _context;

        public EmpresasContratanteService(VoxDocsContext context)
        {
            _context = context;
        }

        public async Task<List<EmpresasContratanteModel>> GetAllAsync()
        {
            return await _context.EmpresasContratantes.ToListAsync();
        }

        public async Task<EmpresasContratanteModel> GetByIdAsync(int id)
        {
            return await _context.EmpresasContratantes.FindAsync(id)
                ?? throw new KeyNotFoundException("Empresa não encontrada.");
        }

        public async Task<EmpresasContratanteModel> GetEmpresaByNome(string nome)
        {
            return await _context.EmpresasContratantes
                .FirstOrDefaultAsync(e => e.EmpresaContratante == nome);
        }

        public async Task<EmpresasContratanteModel> CreateAsync(DTOEmpresasContratante dto)
        {
            // Verificar se empresa já existe
            var existing = await _context.EmpresasContratantes
                .FirstOrDefaultAsync(e => e.EmpresaContratante == dto.EmpresaContratante);
            
            if (existing != null)
            {
                throw new InvalidOperationException("Empresa já cadastrada");
            }

            var empresa = new EmpresasContratanteModel
            {
                Id = Guid.NewGuid(),
                EmpresaContratante = dto.EmpresaContratante,
                Email = dto.Email
            };
            
            _context.EmpresasContratantes.Add(empresa);
            await _context.SaveChangesAsync();
            return empresa;
        }

        public async Task<EmpresasContratanteModel> UpdateAsync(int id, DTOEmpresasContratante dto)
        {
            var empresa = await _context.EmpresasContratantes.FindAsync(id)
                ?? throw new KeyNotFoundException("Empresa não encontrada.");

            empresa.EmpresaContratante = dto.EmpresaContratante;
            empresa.Email = dto.Email;

            await _context.SaveChangesAsync();
            return empresa;
        }

        public async Task DeleteAsync(int id)
        {
            var empresa = await _context.EmpresasContratantes.FindAsync(id)
                ?? throw new KeyNotFoundException("Empresa não encontrada.");

            _context.EmpresasContratantes.Remove(empresa);
            await _context.SaveChangesAsync();
        }
    }
}