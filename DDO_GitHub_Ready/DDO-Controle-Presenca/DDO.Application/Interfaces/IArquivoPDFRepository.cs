using DDO.Core.Entities;

namespace DDO.Application.Interfaces
{
    /// <summary>
    /// Interface para repositório de arquivos PDF
    /// </summary>
    public interface IArquivoPDFRepository
    {
        /// <summary>
        /// Obtém todos os arquivos ativos
        /// </summary>
        Task<IEnumerable<ArquivoPDF>> ObterTodosAtivosAsync();

        /// <summary>
        /// Obtém um arquivo por ID
        /// </summary>
        Task<ArquivoPDF?> ObterPorIdAsync(int id);

        /// <summary>
        /// Obtém arquivos por área
        /// </summary>
        Task<IEnumerable<ArquivoPDF>> ObterPorAreaAsync(int areaId);

        /// <summary>
        /// Obtém arquivos por período de upload
        /// </summary>
        Task<IEnumerable<ArquivoPDF>> ObterPorPeriodoUploadAsync(DateTime dataInicio, DateTime dataFim);

        /// <summary>
        /// Obtém arquivos por período de referência
        /// </summary>
        Task<IEnumerable<ArquivoPDF>> ObterPorPeriodoReferenciaAsync(DateOnly dataInicio, DateOnly dataFim);

        /// <summary>
        /// Adiciona um novo arquivo
        /// </summary>
        Task<ArquivoPDF> AdicionarAsync(ArquivoPDF arquivo);

        /// <summary>
        /// Atualiza um arquivo existente
        /// </summary>
        Task<ArquivoPDF> AtualizarAsync(ArquivoPDF arquivo);

        /// <summary>
        /// Remove um arquivo (soft delete)
        /// </summary>
        Task<bool> RemoverAsync(int id);

        /// <summary>
        /// Verifica se existe um arquivo com o mesmo hash MD5
        /// </summary>
        Task<ArquivoPDF?> ObterPorHashMD5Async(string hashMD5);

        /// <summary>
        /// Incrementa o contador de visualizações
        /// </summary>
        Task IncrementarVisualizacoesAsync(int id);

        /// <summary>
        /// Obtém arquivos com paginação e filtros
        /// </summary>
        Task<(IEnumerable<ArquivoPDF> Arquivos, int Total)> ObterComPaginacaoAsync(
            int pagina, int tamanhoPagina, string? filtroNome = null, int? areaId = null, 
            DateOnly? dataReferenciaInicio = null, DateOnly? dataReferenciaFim = null);

        /// <summary>
        /// Busca arquivos por termo (nome, descrição)
        /// </summary>
        Task<IEnumerable<ArquivoPDF>> BuscarAsync(string termo);

        /// <summary>
        /// Obtém estatísticas de arquivos por área
        /// </summary>
        Task<IEnumerable<dynamic>> ObterEstatisticasPorAreaAsync();

        /// <summary>
        /// Obtém arquivos mais visualizados
        /// </summary>
        Task<IEnumerable<ArquivoPDF>> ObterMaisVisualizadosAsync(int top = 10);

        /// <summary>
        /// Obtém tamanho total de arquivos por área
        /// </summary>
        Task<Dictionary<string, long>> ObterTamanhoTotalPorAreaAsync();
    }
}
