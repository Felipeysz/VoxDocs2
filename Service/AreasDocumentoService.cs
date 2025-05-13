using VoxDocs.Models;
using VoxDocs.Models.Dto;
using VoxDocs.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace VoxDocs.Services
{
    public class AreasDocumentoService : IAreasDocumentoService
    {
        private readonly VoxDocsContext _context;

        public AreasDocumentoService(VoxDocsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DTOAreasDocumentos>> GetAllAsync()
        {
            return await _context.AreasDocumento
                .Select(a => new DTOAreasDocumentos
                {
                    Id = a.Id,
                    Nome = a.Nome
                })
                .ToListAsync();
        }

        public async Task<DTOAreasDocumentos?> GetByIdAsync(int id)
        {
            var area = await _context.AreasDocumento.FindAsync(id);
            if (area == null) return null;
            return new DTOAreasDocumentos
            {
                Id = area.Id,
                Nome = area.Nome
            };
        }

        public async Task<DTOAreasDocumentos> CreateAsync(DTOAreasDocumentos dto)
        {
            var area = new AreasDocumentoModel
            {
                Nome = dto.Nome
            };
            _context.AreasDocumento.Add(area);
            await _context.SaveChangesAsync();
            dto.Id = area.Id;
            return dto;
        }

        public async Task<DTOAreasDocumentos> UpdateAsync(DTOAreasDocumentos dto)
        {
            var area = await _context.AreasDocumento.FindAsync(dto.Id);
            if (area == null) return null;
            area.Nome = dto.Nome;
            _context.AreasDocumento.Update(area);
            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var area = await _context.AreasDocumento.FindAsync(id);
            if (area == null) return false;
            _context.AreasDocumento.Remove(area);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}