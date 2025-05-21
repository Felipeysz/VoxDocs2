using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VoxDocs.DTO;
using VoxDocs.Models.ViewModels;
using VoxDocs.Services;

namespace VoxDocs.ControllerMvc
{
    [Authorize]
    public class DocumentosPaginaMvcController : Controller
    {
        private readonly IPastaPrincipalService _pastaService;
        private readonly ISubPastaService _subService;
        private readonly IDocumentoService _documentoService;

        public DocumentosPaginaMvcController(
            IPastaPrincipalService pastaService,
            ISubPastaService subService,
            IDocumentoService documentoService)
        {
            _pastaService = pastaService;
            _subService = subService;
            _documentoService = documentoService;
        }

        // GET: /DocumentosPaginaMvc/DocumentosPagina
        [HttpGet]
        public async Task<IActionResult> DocumentosPagina(int? pastaPrincipalId, int? subPastaId)
        {
            var vm = new DocumentosViewModel
            {
                PastaPrincipais = await _pastaService.GetAllAsync()
            };

            if (pastaPrincipalId.HasValue)
            {
                // Busca o nome da PastaPrincipal pelo Id
                var pastaPrincipal = await _pastaService.GetByIdAsync(pastaPrincipalId.Value);
                if (pastaPrincipal != null)
                {
                    vm.SubPastas = await _subService.GetSubChildrenAsync(pastaPrincipal.NomePastaPrincipal);
                }
            }

            if (subPastaId.HasValue)
            {
                // Busca o nome da SubPasta pelo Id
                var subPasta = await _subService.GetByIdAsync(subPastaId.Value);
                if (subPasta != null)
                {
                    // Busca documentos reais da subpasta
                    var documentos = await _documentoService.GetBySubPastaAsync(subPasta.NomeSubPasta);
                    vm.Documentos = documentos.Select(d => new DTODocumentoCreate
                    {
                        Id = d.Id,
                        NomeArquivo = d.NomeArquivo,
                        UrlArquivo = d.UrlArquivo,
                        UsuarioCriador = d.UsuarioCriador,
                        DataCriacao = d.DataCriacao,
                        UsuarioUltimaAlteracao = d.UsuarioUltimaAlteracao,
                        DataUltimaAlteracao = d.DataUltimaAlteracao,
                        Empresa = d.Empresa,
                        NomePastaPrincipal = d.NomePastaPrincipal,
                        NomeSubPasta = d.NomeSubPasta,
                        TamanhoArquivo = d.TamanhoArquivo,
                        NivelSeguranca = d.NivelSeguranca,
                        TokenSeguranca = d.TokenSeguranca
                    }).ToList();
                }
            }

            return View(vm);
        }
    }
}