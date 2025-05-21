using System.Collections.Generic;
using System.Threading.Tasks;
using VoxDocs.DTO;

namespace VoxDocs.Services
{
    public interface IDocumentoService
    {
        Task<DTODocumentoCreate> GetByIdAsync(int id, string? token = null);
        Task<IEnumerable<DTODocumentoCreate>> GetAllAsync();
        Task<IEnumerable<DTODocumentoCreate>> GetBySubPastaAsync(string subPasta);
        Task<IEnumerable<DTODocumentoCreate>> GetByPastaPrincipalAsync(string pastaPrincipal);
        Task<DTODocumentoCreate> CreateAsync(DocumentoDto dto);
        Task<DTODocumentoCreate> UpdateAsync(DTODocumentoCreate dto);
        Task DeleteAsync(int id);
        Task<DTOQuantidadeDocumentoEmpresa> GetEstatisticasEmpresaAsync(string empresa);
        Task<DTOAcessosDocumento> GetAcessosDocumentoAsync(int id, int dias);
        Task IncrementarAcessoAsync(int id);
        Task<bool> ArquivoExisteAsync(string nomeArquivo);
    }
}
