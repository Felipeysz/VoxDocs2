using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VoxDocs.Services;
using VoxDocs.ViewModels;
using VoxDocs.DTO;  // DTOs para receber dados via POST

namespace VoxDocs.Controllers
{
    public class PagamentosMvcController : Controller
    {
        private readonly IPlanosVoxDocsService _planosService;
        private readonly IPagamentoFalsoService _pagamentoFalsoService;
        private readonly IConfiguration _configuration;

        public PagamentosMvcController(IPlanosVoxDocsService planosService, IPagamentoFalsoService pagamentoFalsoService, IConfiguration configuration)
        {
            _planosService = planosService;
            _pagamentoFalsoService = pagamentoFalsoService;
            _configuration = configuration;
        }

        
        // GET: /ConfirmandoPagamentoPix?token={token}
        public async Task<IActionResult> ConfirmandoPagamentoPix(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return View("SemTokenConfirmandoPagamentoPix");
            }

            var existe = await _pagamentoFalsoService.TokenPixExisteAsync(token);
            if (!existe)
            {
                return View("SemTokenConfirmandoPagamentoPix");
            }

            ViewData["Token"] = token;
            return View("ConfirmandoPagamentoPix");
        }


        // GET: /Pagamentos?categoria=Basicos
        public async Task<IActionResult> Pagamentos(string categoria)
        {
            if (string.IsNullOrEmpty(categoria))
            {
                return BadRequest("Categoria não informada.");
            }

            var planos = await _planosService.GetPlansByCategoryAsync(categoria);

            if (planos == null || planos.Count == 0)
            {
                return View("NenhumPlano", categoria);
            }

            // Agora usa o _configuration injetado
            var baseUrl = _configuration["ApiSettings:BaseUrl"];
            ViewData["BaseUrl"] = baseUrl;

            var viewModel = new PagamentosViewModel
            {
                Categoria = categoria,
                Planos = planos
            };

            ViewData["BaseUrl"] = baseUrl;

            return View(viewModel);
        }

        // POST: /Pagamentos/Debito
        [HttpPost]
        public async Task<IActionResult> Debito(DTOCartaoPagamentoFalso dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dados inválidos para pagamento débito.");
            }

            try
            {
                var resultado = await _pagamentoFalsoService.ProcessarPagamentoCartaoFalsoAsync(dto);
                ViewBag.Mensagem = resultado;
                return View("ResultadoPagamento");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("ErroPagamento");
            }
        }

        // POST: /Pagamentos/Credito
        [HttpPost]
        public async Task<IActionResult> Credito(DTOCartaoPagamentoFalso dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Dados inválidos para pagamento crédito.");
            }

            try
            {
                var resultado = await _pagamentoFalsoService.ProcessarPagamentoCartaoFalsoAsync(dto);
                ViewBag.Mensagem = resultado;
                return View("ResultadoPagamento");
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("ErroPagamento");
            }
        }


    }
}

