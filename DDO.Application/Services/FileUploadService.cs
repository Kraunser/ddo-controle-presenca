using DDO.Application.Interfaces;
using DDO.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace DDO.Application.Services
{
    /// <summary>
    /// Serviço para gerenciamento de upload de arquivos
    /// </summary>
    public class FileUploadService
    {
        private readonly IArquivoPDFRepository _arquivoPDFRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileUploadService> _logger;

        public FileUploadService(
            IArquivoPDFRepository arquivoPDFRepository,
            IAreaRepository areaRepository,
            IConfiguration configuration,
            ILogger<FileUploadService> logger)
        {
            _arquivoPDFRepository = arquivoPDFRepository;
            _areaRepository = areaRepository;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Realiza o upload de um arquivo PDF
        /// </summary>
        public async Task<(bool Sucesso, string Mensagem, ArquivoPDF? Arquivo)> UploadArquivoAsync(
            IFormFile arquivo, int? areaId, string? descricao, string usuarioId)
        {
            try
            {
                // Validações básicas
                var validacao = ValidarArquivo(arquivo);
                if (!validacao.Valido)
                {
                    return (false, validacao.Mensagem, null);
                }

                // Verificar se a área existe
                if (areaId.HasValue)
                {
                    var area = await _areaRepository.ObterPorIdAsync(areaId.Value);
                    if (area == null)
                    {
                        return (false, "Área não encontrada.", null);
                    }
                }

                // Calcular hash MD5 do arquivo
                var hashMD5 = await CalcularHashMD5Async(arquivo);

                // Verificar se já existe um arquivo com o mesmo hash
                var arquivoExistente = await _arquivoPDFRepository.ObterPorHashMD5Async(hashMD5);
                if (arquivoExistente != null)
                {
                    return (false, $"Este arquivo já foi enviado anteriormente em {arquivoExistente.DataUpload:dd/MM/yyyy HH:mm}.", null);
                }

                // Gerar nome único para o arquivo
                var nomeArquivoSalvo = GerarNomeUnicoArquivo(arquivo.FileName);
                var caminhoCompleto = await SalvarArquivoAsync(arquivo, nomeArquivoSalvo);

                // Criar entidade ArquivoPDF
                var arquivoPDF = new ArquivoPDF
                {
                    NomeArquivo = arquivo.FileName,
                    NomeArquivoSalvo = nomeArquivoSalvo,
                    CaminhoArquivo = caminhoCompleto,
                    TamanhoArquivo = arquivo.Length,
                    TipoMime = arquivo.ContentType,
                    DataUpload = DateTime.UtcNow,
                    Descricao = descricao,
                    HashMD5 = hashMD5,
                    AreaId = areaId,
                    UploadedById = usuarioId,
                    Ativo = true
                };

                // Salvar no banco de dados
                var arquivoSalvo = await _arquivoPDFRepository.AdicionarAsync(arquivoPDF);

                _logger.LogInformation("Arquivo {NomeArquivo} enviado com sucesso por usuário {UsuarioId}", 
                    arquivo.FileName, usuarioId);

                return (true, "Arquivo enviado com sucesso!", arquivoSalvo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer upload do arquivo {NomeArquivo}", arquivo.FileName);
                return (false, "Erro interno do servidor. Tente novamente.", null);
            }
        }

        /// <summary>
        /// Valida o arquivo enviado
        /// </summary>
        private (bool Valido, string Mensagem) ValidarArquivo(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return (false, "Nenhum arquivo foi selecionado.");
            }

            // Verificar tamanho máximo
            var tamanhoMaximo = _configuration.GetValue<long>("ApplicationSettings:MaxFileUploadSize", 10485760); // 10MB padrão
            if (arquivo.Length > tamanhoMaximo)
            {
                var tamanhoMB = tamanhoMaximo / 1024 / 1024;
                return (false, $"O arquivo é muito grande. Tamanho máximo permitido: {tamanhoMB}MB.");
            }

            // Verificar extensão
            var extensoesPermitidas = _configuration.GetSection("ApplicationSettings:AllowedFileExtensions").Get<string[]>() 
                ?? new[] { ".pdf" };
            
            var extensaoArquivo = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            if (!extensoesPermitidas.Contains(extensaoArquivo))
            {
                return (false, $"Tipo de arquivo não permitido. Extensões aceitas: {string.Join(", ", extensoesPermitidas)}");
            }

            // Verificar tipo MIME
            if (!arquivo.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Apenas arquivos PDF são permitidos.");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Calcula o hash MD5 do arquivo
        /// </summary>
        private async Task<string> CalcularHashMD5Async(IFormFile arquivo)
        {
            using var md5 = MD5.Create();
            using var stream = arquivo.OpenReadStream();
            var hash = await Task.Run(() => md5.ComputeHash(stream));
            return Convert.ToHexString(hash).ToLowerInvariant();
        }

        /// <summary>
        /// Gera um nome único para o arquivo
        /// </summary>
        private string GerarNomeUnicoArquivo(string nomeOriginal)
        {
            var extensao = Path.GetExtension(nomeOriginal);
            var nomeBase = Path.GetFileNameWithoutExtension(nomeOriginal);
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var guid = Guid.NewGuid().ToString("N")[..8];
            
            return $"{nomeBase}_{timestamp}_{guid}{extensao}";
        }

        /// <summary>
        /// Salva o arquivo no sistema de arquivos
        /// </summary>
        private async Task<string> SalvarArquivoAsync(IFormFile arquivo, string nomeArquivoSalvo)
        {
            var caminhoUpload = _configuration.GetValue<string>("ApplicationSettings:UploadPath", "uploads/pdfs");
            var diretorioCompleto = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", caminhoUpload);
            
            // Criar diretório se não existir
            if (!Directory.Exists(diretorioCompleto))
            {
                Directory.CreateDirectory(diretorioCompleto);
            }

            var caminhoCompleto = Path.Combine(diretorioCompleto, nomeArquivoSalvo);
            
            using var stream = new FileStream(caminhoCompleto, FileMode.Create);
            await arquivo.CopyToAsync(stream);

            return Path.Combine(caminhoUpload, nomeArquivoSalvo).Replace("\\", "/");
        }

        /// <summary>
        /// Remove um arquivo do sistema
        /// </summary>
        public async Task<bool> RemoverArquivoAsync(int arquivoId, string usuarioId)
        {
            try
            {
                var arquivo = await _arquivoPDFRepository.ObterPorIdAsync(arquivoId);
                if (arquivo == null)
                {
                    return false;
                }

                // Remover do banco (soft delete)
                var removido = await _arquivoPDFRepository.RemoverAsync(arquivoId);
                
                if (removido)
                {
                    // Tentar remover o arquivo físico
                    try
                    {
                        var caminhoCompleto = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", arquivo.CaminhoArquivo);
                        if (File.Exists(caminhoCompleto))
                        {
                            File.Delete(caminhoCompleto);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Não foi possível remover o arquivo físico {CaminhoArquivo}", arquivo.CaminhoArquivo);
                    }

                    _logger.LogInformation("Arquivo {NomeArquivo} removido por usuário {UsuarioId}", 
                        arquivo.NomeArquivo, usuarioId);
                }

                return removido;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover arquivo {ArquivoId}", arquivoId);
                return false;
            }
        }

        /// <summary>
        /// Obtém informações de um arquivo para download
        /// </summary>
        public async Task<(bool Encontrado, string CaminhoFisico, string NomeArquivo, string TipoMime)> ObterArquivoParaDownloadAsync(int arquivoId)
        {
            var arquivo = await _arquivoPDFRepository.ObterPorIdAsync(arquivoId);
            if (arquivo == null || !arquivo.Ativo)
            {
                return (false, string.Empty, string.Empty, string.Empty);
            }

            var caminhoFisico = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", arquivo.CaminhoArquivo);
            if (!File.Exists(caminhoFisico))
            {
                return (false, string.Empty, string.Empty, string.Empty);
            }

            // Incrementar contador de visualizações
            await _arquivoPDFRepository.IncrementarVisualizacoesAsync(arquivoId);

            return (true, caminhoFisico, arquivo.NomeArquivo, arquivo.TipoMime);
        }
    }
}
