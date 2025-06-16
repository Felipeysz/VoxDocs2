using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VoxDocs.DTO;
using VoxDocs.Models;
using VoxDocs.Services;
using System.Net;

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
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var empresas = await _service.GetAllAsync();
                return Ok(new
                {
                    Success = true,
                    Data = empresas,
                    Message = "Empresas recuperadas com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var empresa = await _service.GetByIdAsync(id);
                return Ok(new
                {
                    Success = true,
                    Data = empresa,
                    Message = "Empresa recuperada com sucesso"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("nome/{nome}")]
        public async Task<IActionResult> GetByNome(string nome)
        {
            try
            {
                var empresa = await _service.GetEmpresaByNome(nome);
                return Ok(new
                {
                    Success = true,
                    Data = empresa,
                    Message = "Empresa recuperada com sucesso"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DTOEmpresasContratante dto)
        {
            try
            {
                var empresa = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = empresa.Id }, new
                {
                    Success = true,
                    Data = empresa,
                    Message = "Empresa criada com sucesso"
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DTOEmpresasContratante dto)
        {
            try
            {
                var empresa = await _service.UpdateAsync(id, dto);
                return Ok(new
                {
                    Success = true,
                    Data = empresa,
                    Message = "Empresa atualizada com sucesso"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok(new
                {
                    Success = true,
                    Message = "Empresa deletada com sucesso"
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}