using Microsoft.EntityFrameworkCore;
using VoxDocs.Data;
using VoxDocs.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VoxDocs.Data.Repositories
{
    public class EmpresasContratanteRepository : IEmpresasContratanteRepository
    {
        private readonly VoxDocsContext _context;

        public EmpresasContratanteRepository(VoxDocsContext context)
        {
            _context = context;
        }

        public async Task<List<EmpresasContratanteModel>> GetAllAsync()
        {
            return await _context.EmpresasContratantes.ToListAsync();
        }

        public async Task<EmpresasContratanteModel> GetByIdAsync(Guid id)
        {
            return await _context.EmpresasContratantes.FindAsync(id);
        }

        public async Task<EmpresasContratanteModel> GetByNomeAsync(string nome)
        {
            return await _context.EmpresasContratantes
                .FirstOrDefaultAsync(e => e.EmpresaContratante == nome);
        }

        public async Task<EmpresasContratanteModel> CreateAsync(EmpresasContratanteModel empresa)
        {
            _context.EmpresasContratantes.Add(empresa);
            await _context.SaveChangesAsync();
            return empresa;
        }

        public async Task<EmpresasContratanteModel> UpdateAsync(EmpresasContratanteModel empresa)
        {
            _context.EmpresasContratantes.Update(empresa);
            await _context.SaveChangesAsync();
            return empresa;
        }

        public async Task DeleteAsync(Guid id)
        {
            var empresa = await GetByIdAsync(id);
            if (empresa != null)
            {
                _context.EmpresasContratantes.Remove(empresa);
                await _context.SaveChangesAsync();
            }
        }
    }
}