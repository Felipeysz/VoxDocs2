using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoxDocs.DTO;
using VoxDocs.Interfaces;
using VoxDocs.Models;
using VoxDocs.Services;

namespace VoxDocs.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanosVoxDocsController : ControllerBase
    {
        private readonly IPlanosVoxDocsService _planosService;

        public PlanosVoxDocsController(IPlanosVoxDocsService planosService)
        {
            _planosService = planosService;
        }

        /// Obtém todos os planos disponíveis
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanosVoxDocsModel>>> GetAllPlans()
        {
            try
            {
                var plans = await _planosService.GetAllPlansAsync();
                return Ok(plans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao obter planos: {ex.Message}" });
            }
        }

        /// Obtém planos por categoria
        /// <param name="categoria">Categoria do plano (ex: 'basic', 'pro', 'enterprise')</param>
        [HttpGet("categoria/{categoria}")]
        public async Task<ActionResult<IEnumerable<PlanosVoxDocsModel>>> GetPlansByCategory(string categoria)
        {
            try
            {
                var plans = await _planosService.GetPlansByCategoryAsync(categoria);
                return Ok(plans);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao obter planos por categoria: {ex.Message}" });
            }
        }

        /// Obtém um plano pelo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<PlanosVoxDocsModel>> GetPlanById(int id)
        {
            try
            {
                var plan = await _planosService.GetPlanByIdAsync(id);
                return Ok(plan);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = $"Plano não encontrado: {ex.Message}" });
            }
        }

        /// Obtém um plano pelo nome
        [HttpGet("nome/{name}")]
        public async Task<ActionResult<PlanosVoxDocsModel>> GetPlanByName(string name)
        {
            try
            {
                var plan = await _planosService.GetPlanByNameAsync(name);
                return Ok(plan);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = $"Plano não encontrado: {ex.Message}" });
            }
        }

        /// Obtém um plano pelo nome e periodicidade
        [HttpGet("nome/{nome}/periodicidade/{periodicidade}")]
        public async Task<ActionResult<PlanosVoxDocsModel>> GetPlanByNameAndPeriodicidade(string nome, string periodicidade)
        {
            try
            {
                var plan = await _planosService.GetPlanByNameAndPeriodicidadeAsync(nome, periodicidade);
                return Ok(plan);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = $"Plano não encontrado: {ex.Message}" });
            }
        }

        /// Cria um novo plano
        [HttpPost]
        public async Task<ActionResult<PlanosVoxDocsModel>> CreatePlan([FromBody] DTOPlanosVoxDocs dto)
        {
            try
            {
                var plan = await _planosService.CreatePlanAsync(dto);
                return CreatedAtAction(nameof(GetPlanById), new { id = plan.Id }, plan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao criar plano: {ex.Message}" });
            }
        }

        /// Atualiza um plano existente
        [HttpPut("{id}")]
        public async Task<ActionResult<PlanosVoxDocsModel>> UpdatePlan(int id, [FromBody] DTOPlanosVoxDocs dto)
        {
            try
            {
                var updatedPlan = await _planosService.UpdatePlanAsync(id, dto);
                return Ok(updatedPlan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao atualizar plano: {ex.Message}" });
            }
        }

        /// Remove um plano
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlan(int id)
        {
            try
            {
                await _planosService.DeletePlanAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao deletar plano: {ex.Message}" });
            }
        }
    }
}