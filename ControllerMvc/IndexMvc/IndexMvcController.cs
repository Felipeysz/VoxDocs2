using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using VoxDocs.Services;
using VoxDocs.ViewModels;
using VoxDocs.Models;

namespace VoxDocs.Controllers
{
    public class IndexMvcController : Controller
    {
        private readonly IPlanosVoxDocsService _planosService;

        public IndexMvcController(IPlanosVoxDocsService planosService)
            => _planosService = planosService;

        // Método auxiliar para mapear Model para ViewModel
        private PlanoViewModel MapToViewModel(PlanosVoxDocsModel plano)
        {
            return new PlanoViewModel
            {
                Nome = plano.Nome,
                Periodicidade = plano.Periodicidade,
                Preco = plano.Preco,
                ArmazenamentoDisponivel = plano.ArmazenamentoDisponivel,
                LimiteAdmin = plano.LimiteAdmin,
                LimiteUsuario = plano.LimiteUsuario
            };
        }

        // GET: /IndexMvc/Index
        public async Task<IActionResult> Index()
        {
            if (TempData.ContainsKey("ErrorMessage"))
                ViewBag.ErrorMessage = TempData["ErrorMessage"]?.ToString();

            // Busca todos os planos do banco
            var planosModel = await _planosService.GetAllPlansAsync();

            var planosViewModel = planosModel
                .Select(plano => new PlanoViewModel
                {
                    Nome = plano.Nome,
                    Periodicidade = plano.Periodicidade,
                    Preco = plano.Preco,
                    ArmazenamentoDisponivel = plano.ArmazenamentoDisponivel,
                    LimiteAdmin = plano.LimiteAdmin,
                    LimiteUsuario = plano.LimiteUsuario
                })
                .ToList();

            var vm = new PlanosIndexViewModel
            {
                Planos = planosViewModel
            };

            return View(vm);
        }

    }
}