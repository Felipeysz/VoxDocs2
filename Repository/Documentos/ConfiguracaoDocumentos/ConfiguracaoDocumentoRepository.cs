// ConfiguracaoDocumentoRepository.cs
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using VoxDocs.Data;
using VoxDocs.Models;

namespace VoxDocs.Data.Repositories
{
    public class ConfiguracaoDocumentoRepository : IConfiguracaoDocumentoRepository
    {
        private readonly VoxDocsContext _context;

        public ConfiguracaoDocumentoRepository(VoxDocsContext context)
        {
            _context = context;
        }

        public async Task<ConfiguracaoDocumentosModel> GetFirstAsync()
        {
            // Retorna a primeira configuração ou cria uma nova se não existir
            var config = await _context.ConfiguracaoDocumentos.FirstOrDefaultAsync();
            
            if (config == null)
            {
                config = new ConfiguracaoDocumentosModel
                {
                    PermitirPDF = true,
                    PermitirWord = true,
                    PermitirExcel = true,
                    PermitirImagens = true,
                    TamanhoMaximoMB = 10,
                    DiasArmazenamentoTemporario = 30
                };
                await _context.ConfiguracaoDocumentos.AddAsync(config);
                await _context.SaveChangesAsync();
            }
            
            return config;
        }

        public async Task UpdateAsync(ConfiguracaoDocumentosModel config)
        {
            _context.ConfiguracaoDocumentos.Update(config);
            await _context.SaveChangesAsync();
        }
    }
}