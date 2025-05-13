using Microsoft.AspNetCore.Mvc;
using VoxDocs.Models;
using VoxDocs.Services;
using VoxDocs.DTO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VoxDocs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadDocumentosController : ControllerBase
    {
        private readonly IDocumentoUploadService _service;
        private readonly AzureBlobService _blobService;
        private readonly IDocumentoService _documentoService;

        public UploadDocumentosController(
            IDocumentoUploadService service,
            AzureBlobService blobService,
            IDocumentoService documentoService)
        {
            _service = service;
            _blobService = blobService;
            _documentoService = documentoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DTOUploadDocumento>>> GetAll()
        {
            var uploads = await _service.GetAllAsync();
            var dtos = uploads.Select(u => new DTOUploadDocumento
            {
                Id = u.Id,
                NomeArquivo = u.NomeArquivo,
                UrlArquivo = u.UrlArquivo,
                UsuarioCriador = u.UsuarioCriador,
                DataCriacao = u.DataCriacao,
                UsuarioUltimaAlteracao = u.UsuarioUltimaAlteracao,
                DataUltimaAlteracao = u.DataUltimaAlteracao
            });
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DTOUploadDocumento>> GetById(int id)
        {
            var u = await _service.GetByIdAsync(id);
            if (u == null) return NotFound();
            var dto = new DTOUploadDocumento
            {
                Id = u.Id,
                NomeArquivo = u.NomeArquivo,
                UrlArquivo = u.UrlArquivo,
                UsuarioCriador = u.UsuarioCriador,
                DataCriacao = u.DataCriacao,
                UsuarioUltimaAlteracao = u.UsuarioUltimaAlteracao,
                DataUltimaAlteracao = u.DataUltimaAlteracao
            };
            return Ok(dto);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocumento([FromForm] UploadDocumentoDto dto)
        {
            if (dto.File == null || dto.AreaId == 0 || dto.TipoId == 0)
                return BadRequest("Preencha todos os campos.");

            var ext = System.IO.Path.GetExtension(dto.File.FileName).ToLower();
            var date = DateTime.Now.ToString("MMyyyy");
            var fileName = $"DOC{dto.TipoId}{date}_{dto.AreaId}{ext}";

            string url;
            using (var stream = dto.File.OpenReadStream())
            {
                url = await _blobService.UploadAsync(fileName, stream);
            }

            var docUpload = new DocumentoUploadModel
            {
                NomeArquivo = fileName,
                UrlArquivo = url,
                UsuarioCriador = dto.Usuario,
                DataCriacao = DateTime.Now,
                UsuarioUltimaAlteracao = dto.Usuario,
                DataUltimaAlteracao = DateTime.Now
            };

            // Salva o upload
            var uploadSalvo = await _service.CreateAsync(docUpload);

            // Cria o Documento vinculado ao upload
            var documento = new DocumentoModel
            {
                Nome = fileName,
                Descricao = dto.Descricao,
                AreaDocumentoId = dto.AreaId,
                TipoDocumentoId = dto.TipoId,
                DocumentoUploadId = uploadSalvo.Id,
                UsuarioCriador = dto.Usuario,
                DataCriacao = DateTime.Now,
                UsuarioUltimaAlteracao = dto.Usuario,
                DataAtualizacao = DateTime.Now
            };
            await _documentoService.CreateAsync(documento);

            return Ok(new DTOUploadDocumento
            {
                Id = uploadSalvo.Id,
                NomeArquivo = uploadSalvo.NomeArquivo,
                UrlArquivo = uploadSalvo.UrlArquivo,
                UsuarioCriador = uploadSalvo.UsuarioCriador,
                DataCriacao = uploadSalvo.DataCriacao,
                UsuarioUltimaAlteracao = uploadSalvo.UsuarioUltimaAlteracao,
                DataUltimaAlteracao = uploadSalvo.DataUltimaAlteracao
            });
        }
    }
}