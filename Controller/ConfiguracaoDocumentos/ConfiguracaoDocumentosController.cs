using Microsoft.AspNetCore.Mvc;
using VoxDocs.Services;
using VoxDocs.DTO;
using System.Threading.Tasks;

namespace VoxDocs.Controllers
{
    [ApiController]
    [Route("api/configuracao-documentos")]
    public class ConfiguracaoDocumentoController : ControllerBase
    {
        private readonly IConfiguracaoDocumentoService _service;

        public ConfiguracaoDocumentoController(IConfiguracaoDocumentoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetConfiguracoes()
        {
            var result = await _service.GetConfiguracoesAsync();
            
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new
                {
                    message = result.ErrorMessage,
                    success = result.Success
                });
            }

            return Ok(new
            {
                data = result.Data,
                success = result.Success
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateConfiguracoes([FromBody] DTOConfiguracaoDocumentos dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Dados de configuração não fornecidos",
                    success = false
                });
            }

            var result = await _service.SalvarConfiguracoesAsync(dto);
            
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new
                {
                    message = result.ErrorMessage,
                    success = result.Success
                });
            }

            return Ok(new
            {
                data = result.Data,
                success = result.Success,
                message = "Configurações atualizadas com sucesso"
            });
        }

        [HttpGet("validar-tipo-arquivo")]
        public async Task<IActionResult> ValidarTipoArquivo([FromQuery] string nomeArquivo)
        {
            if (string.IsNullOrWhiteSpace(nomeArquivo))
            {
                return BadRequest(new
                {
                    message = "Nome do arquivo é obrigatório",
                    success = false
                });
            }

            try
            {
                var valido = await _service.ValidarTipoArquivoAsync(nomeArquivo);
                return Ok(new
                {
                    valido,
                    success = true
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new
                {
                    message = $"Erro ao validar tipo de arquivo: {ex.Message}",
                    success = false
                });
            }
        }

        [HttpGet("validar-tamanho-arquivo")]
        public async Task<IActionResult> ValidarTamanhoArquivo([FromQuery] long tamanhoArquivo)
        {
            if (tamanhoArquivo <= 0)
            {
                return BadRequest(new
                {
                    message = "Tamanho do arquivo deve ser maior que zero",
                    success = false
                });
            }

            try
            {
                var valido = await _service.ValidarTamanhoArquivoAsync(tamanhoArquivo);
                return Ok(new
                {
                    valido,
                    success = true
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new
                {
                    message = $"Erro ao validar tamanho do arquivo: {ex.Message}",
                    success = false
                });
            }
        }
    }
}