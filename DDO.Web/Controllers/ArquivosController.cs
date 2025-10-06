using DDO.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DDO.Web.Controllers
{
    /// <summary>
    /// Controller para operações com arquivos PDF
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ArquivosController : ControllerBase
    {
        private readonly FileUploadService _fileUploadService;
        private readonly ILogger<ArquivosController> _logger;

        public ArquivosController(FileUploadService fileUploadService, ILogger<ArquivosController> logger)
        {
            _fileUploadService = fileUploadService;
            _logger = logger;
        }

        /// <summary>
        /// Faz o download de um arquivo PDF
        /// </summary>
        [HttpGet("{id}/download")]
        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var resultado = await _fileUploadService.ObterArquivoParaDownloadAsync(id);
                
                if (!resultado.Encontrado)
                {
                    return NotFound("Arquivo não encontrado.");
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(resultado.CaminhoFisico);
                
                return File(fileBytes, resultado.TipoMime, resultado.NomeArquivo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer download do arquivo {ArquivoId}", id);
                return StatusCode(500, "Erro interno do servidor.");
            }
        }

        /// <summary>
        /// Remove um arquivo PDF (apenas para administradores)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "";
                var removido = await _fileUploadService.RemoverArquivoAsync(id, userId);
                
                if (removido)
                {
                    return Ok(new { message = "Arquivo removido com sucesso." });
                }
                else
                {
                    return NotFound("Arquivo não encontrado.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover arquivo {ArquivoId}", id);
                return StatusCode(500, "Erro interno do servidor.");
            }
        }
    }
}
