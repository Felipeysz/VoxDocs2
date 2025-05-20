using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoxDocs.DTO;
using VoxDocs.Models;
using VoxDocs.Services;

namespace VoxDocs.Controllers
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

        [HttpGet]
        public async Task<ActionResult<List<PlanosVoxDocsModel>>> GetPlans()
        {
            var plans = await _planosService.GetAllPlansAsync();
            return Ok(plans);
        }

        [HttpPost]
        public async Task<ActionResult<PlanosVoxDocsModel>> CreatePlan([FromBody] DTOPlanosVoxDocs dto)
        {
            var plan = await _planosService.CreatePlanAsync(dto);
            return CreatedAtAction(nameof(GetPlans), new { id = plan.Id }, plan);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PlanosVoxDocsModel>> UpdatePlan(int id, [FromBody] DTOPlanosVoxDocs dto)
        {
            var updatedPlan = await _planosService.UpdatePlanAsync(id, dto);
            return Ok(updatedPlan);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlan(int id)
        {
            await _planosService.DeletePlanAsync(id);
            return NoContent();
        }
    }
}