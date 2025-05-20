using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoxDocs.DTO;
using VoxDocs.Models;
using VoxDocs.Services;
using VoxDocs.BusinessRules;

namespace VoxDocs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpresasContratanteController : ControllerBase
    {
        private readonly IEmpresasContratanteService _service;
        private readonly EmpresasContratanteRules _rules;

        public EmpresasContratanteController(IEmpresasContratanteService service, EmpresasContratanteRules rules)
        {
            _service = service;
            _rules = rules;
        }

        [HttpGet]
        public async Task<ActionResult<List<EmpresasContratanteModel>>> GetAll()
        {
            var empresas = await _service.GetAllAsync();
            return Ok(empresas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmpresasContratanteModel>> GetById(int id)
        {
            var empresa = await _service.GetByIdAsync(id);
            return Ok(empresa);
        }

        [HttpPost]
        public async Task<ActionResult<EmpresasContratanteModel>> Create([FromBody] DTOEmpresasContratante dto)
        {
            _rules.Validate(dto);
            var empresa = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = empresa.Id }, empresa);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EmpresasContratanteModel>> Update(int id, [FromBody] DTOEmpresasContratante dto)
        {
            _rules.Validate(dto);
            var empresa = await _service.UpdateAsync(id, dto);
            return Ok(empresa);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("Plano/{nomeEmpresa}")]
        public async Task<ActionResult<DTOEmpresasContratantePlano>> GetPlanoByEmpresa(string nomeEmpresa)
        {
            if (string.IsNullOrWhiteSpace(nomeEmpresa))
                return BadRequest(new { mensagem = "Empresa não informada." });

            try
            {
                var dto = await _service.GetPlanoByEmpresaAsync(nomeEmpresa);
                return Ok(dto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }
    }
}