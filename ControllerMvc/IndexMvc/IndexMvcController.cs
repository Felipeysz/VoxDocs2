using Microsoft.AspNetCore.Mvc;
using VoxDocs.Services;
using VoxDocs.ViewModels;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using VoxDocs.Models;
using VoxDocs.Interfaces;

namespace VoxDocs.Controllers
{
    public class IndexMvcController : Controller
    {
        private readonly IPlanosVoxDocsService _planosService;

        public IndexMvcController(IPlanosVoxDocsService planosService)
        {
            _planosService = planosService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var planos = await _planosService.GetAllPlansAsync();
                
                var viewModel = new PlanosViewModel
                {
                    Planos = planos
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ocorreu um erro ao carregar os planos. Por favor, tente novamente mais tarde.";
                // Em produção, você pode querer logar o erro ex
                return View(new PlanosViewModel { Planos = new List<PlanosVoxDocsModel>() });
            }
        }
    }
}