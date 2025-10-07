<<<<<<< HEAD
using DDO.Core.Entities;

namespace DDO.Application.Interfaces
{
    /// <summary>
    /// Interface para repositório de presenças
    /// </summary>
    public interface IPresencaRepository
    {
        /// <summary>
        /// Obtém todas as presenças de um período
        /// </summary>
        Task<IEnumerable<Presenca>> ObterPorPeriodoAsync(DateOnly dataInicio, DateOnly dataFim);

        /// <summary>
        /// Obtém presenças de um colaborador em um período
        /// </summary>
        Task<IEnumerable<Presenca>> ObterPorColaboradorPeriodoAsync(int colaboradorId, DateOnly dataInicio, DateOnly dataFim);

        /// <summary>
        /// Obtém presenças de uma área em um período
        /// </summary>
        Task<IEnumerable<Presenca>> ObterPorAreaPeriodoAsync(int areaId, DateOnly dataInicio, DateOnly dataFim);

        /// <summary>
        /// Obtém uma presença por ID
        /// </summary>
        Task<Presenca?> ObterPorIdAsync(int id);

        /// <summary>
        /// Registra uma nova presença
        /// </summary>
        Task<Presenca> RegistrarPresencaAsync(Presenca presenca);

        /// <summary>
        /// Atualiza uma presença existente
        /// </summary>
        Task<Presenca> AtualizarAsync(Presenca presenca);

        /// <summary>
        /// Remove uma presença
        /// </summary>
        Task<bool> RemoverAsync(int id);

        /// <summary>
        /// Verifica se já existe registro de presença para o colaborador na data
        /// </summary>
        Task<bool> ExistePresencaNaDataAsync(int colaboradorId, DateOnly data, string? tipoRegistro = null);

        /// <summary>
        /// Obtém a última presença de um colaborador
        /// </summary>
        Task<Presenca?> ObterUltimaPresencaColaboradorAsync(int colaboradorId);

        /// <summary>
        /// Obtém estatísticas de presença por área
        /// </summary>
        Task<IEnumerable<dynamic>> ObterEstatisticasPorAreaAsync(DateOnly dataInicio, DateOnly dataFim);

        /// <summary>
        /// Obtém estatísticas de presença por dia
        /// </summary>
        Task<IEnumerable<dynamic>> ObterEstatisticasPorDiaAsync(DateOnly dataInicio, DateOnly dataFim);

        /// <summary>
        /// Obtém ranking de colaboradores por presença
        /// </summary>
        Task<IEnumerable<dynamic>> ObterRankingColaboradoresAsync(DateOnly dataInicio, DateOnly dataFim, int top = 10);

        /// <summary>
        /// Obtém presenças com paginação e filtros
        /// </summary>
        Task<(IEnumerable<Presenca> Presencas, int Total)> ObterComPaginacaoAsync(
            int pagina, int tamanhoPagina, DateOnly? dataInicio = null, DateOnly? dataFim = null, 
            int? colaboradorId = null, int? areaId = null, bool? validado = null);

        /// <summary>
        /// Obtém total de presenças por área em um período
        /// </summary>
        Task<Dictionary<string, int>> ObterTotalPresencasPorAreaAsync(DateOnly dataInicio, DateOnly dataFim);

        /// <summary>
        /// Obtém média de presenças diárias por área
        /// </summary>
        Task<Dictionary<string, double>> ObterMediaPresencasDiariasPorAreaAsync(DateOnly dataInicio, DateOnly dataFim);

        /// <summary>
        /// Obtém dados para gráfico de presença ao longo do tempo
        /// </summary>
        Task<IEnumerable<dynamic>> ObterDadosGraficoPresencaTemporalAsync(DateOnly dataInicio, DateOnly dataFim, string agrupamento = "dia");
    }
}
=======
using DDO.Core.Entities;

namespace DDO.Application.Interfaces
{
    /// <summary>
    /// Interface para repositório de presenças
    /// </summary>
    public interface IPresencaRepository
    {
        /// <summary>
        /// Obtém todas as presenças de um período
        /// </summary>
        Task<IEnumerable<Presenca>> ObterPorPeriodoAsync(DateOnly dataInicio, DateOnly dataFim);

        /// <summary>
        /// Obtém presenças de um colaborador em um período
        /// </summary>
        Task<IEnumerable<Presenca>> ObterPorColaboradorPeriodoAsync(int colaboradorId, DateOnly dataInicio, DateOnly dataFim);

        /// <summary>
        /// Obtém presenças de uma área em um período
        /// </summary>
        Task<IEnumerable<Presenca>> ObterPorAreaPeriodoAsync(int areaId, DateOnly dataInicio, DateOnly dataFim);

        /// <summary>
        /// Obtém uma presença por ID
        /// </summary>
        Task<Presenca?> ObterPorIdAsync(int id);

        /// <summary>
        /// Registra uma nova presença
        /// </summary>
        Task<Presenca> RegistrarPresencaAsync(Presenca presenca);

        /// <summary>
        /// Atualiza uma presença existente
        /// </summary>
        Task<Presenca> AtualizarAsync(Presenca presenca);

        /// <summary>
        /// Remove uma presença
        /// </summary>
        Task<bool> RemoverAsync(int id);

        /// <summary>
        /// Verifica se já existe registro de presença para o colaborador na data
        /// </summary>
        Task<bool> ExistePresencaNaDataAsync(int colaboradorId, DateOnly data, string? tipoRegistro = null);

        /// <summary>
        /// Obtém a última presença de um colaborador
        /// </summary>
        Task<Presenca?> ObterUltimaPresencaColaboradorAsync(int colaboradorId);

        /// <summary>
        /// Obtém estatísticas de presença por área
        /// </summary>
        Task<IEnumerable<dynamic>> ObterEstatisticasPorAreaAsync(DateOnly dataInicio, DateOnly dataFim);

        /// <summary>
        /// Obtém estatísticas de presença por dia
        /// </summary>
        Task<IEnumerable<dynamic>> ObterEstatisticasPorDiaAsync(DateOnly dataInicio, DateOnly dataFim);

        /// <summary>
        /// Obtém ranking de colaboradores por presença
        /// </summary>
        Task<IEnumerable<dynamic>> ObterRankingColaboradoresAsync(DateOnly dataInicio, DateOnly dataFim, int top = 10);

        /// <summary>
        /// Obtém presenças com paginação e filtros
        /// </summary>
        Task<(IEnumerable<Presenca> Presencas, int Total)> ObterComPaginacaoAsync(
            int pagina, int tamanhoPagina, DateOnly? dataInicio = null, DateOnly? dataFim = null, 
            int? colaboradorId = null, int? areaId = null, bool? validado = null);

        /// <summary>
        /// Obtém total de presenças por área em um período
        /// </summary>
        Task<Dictionary<string, int>> ObterTotalPresencasPorAreaAsync(DateOnly dataInicio, DateOnly dataFim);

        /// <summary>
        /// Obtém média de presenças diárias por área
        /// </summary>
        Task<Dictionary<string, double>> ObterMediaPresencasDiariasPorAreaAsync(DateOnly dataInicio, DateOnly dataFim);

        /// <summary>
        /// Obtém dados para gráfico de presença ao longo do tempo
        /// </summary>
        Task<IEnumerable<dynamic>> ObterDadosGraficoPresencaTemporalAsync(DateOnly dataInicio, DateOnly dataFim, string agrupamento = "dia");
    }
}
>>>>>>> b90a182 (Initial commit of DDO project)
