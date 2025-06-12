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
    public class EmpresasContratanteController : ControllerBase
    {
        private readonly IEmpresasContratanteService _service;

        public EmpresasContratanteController(IEmpresasContratanteService service)
        {
            _service = service;
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
            var empresa = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = empresa.Id }, empresa);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EmpresasContratanteModel>> Update(int id, [FromBody] DTOEmpresasContratante dto)
        {
            var empresa = await _service.UpdateAsync(id, dto);
            return Ok(empresa);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

    }
}