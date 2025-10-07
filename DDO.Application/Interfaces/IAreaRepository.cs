<<<<<<< HEAD
using DDO.Core.Entities;

namespace DDO.Application.Interfaces
{
    /// <summary>
    /// Interface para repositório de áreas
    /// </summary>
    public interface IAreaRepository
    {
        /// <summary>
        /// Obtém todas as áreas ativas
        /// </summary>
        Task<IEnumerable<Area>> ObterTodasAtivasAsync();

        /// <summary>
        /// Obtém uma área por ID
        /// </summary>
        Task<Area?> ObterPorIdAsync(int id);

        /// <summary>
        /// Obtém uma área por nome
        /// </summary>
        Task<Area?> ObterPorNomeAsync(string nome);

        /// <summary>
        /// Adiciona uma nova área
        /// </summary>
        Task<Area> AdicionarAsync(Area area);

        /// <summary>
        /// Atualiza uma área existente
        /// </summary>
        Task<Area> AtualizarAsync(Area area);

        /// <summary>
        /// Remove uma área (soft delete)
        /// </summary>
        Task<bool> RemoverAsync(int id);

        /// <summary>
        /// Verifica se existe uma área com o nome especificado
        /// </summary>
        Task<bool> ExisteComNomeAsync(string nome, int? idExcluir = null);

        /// <summary>
        /// Obtém estatísticas de presença por área
        /// </summary>
        Task<IEnumerable<dynamic>> ObterEstatisticasPresencaAsync(DateOnly dataInicio, DateOnly dataFim);
    }
}
=======
using DDO.Core.Entities;

namespace DDO.Application.Interfaces
{
    /// <summary>
    /// Interface para repositório de áreas
    /// </summary>
    public interface IAreaRepository
    {
        /// <summary>
        /// Obtém todas as áreas ativas
        /// </summary>
        Task<IEnumerable<Area>> ObterTodasAtivasAsync();

        /// <summary>
        /// Obtém uma área por ID
        /// </summary>
        Task<Area?> ObterPorIdAsync(int id);

        /// <summary>
        /// Obtém uma área por nome
        /// </summary>
        Task<Area?> ObterPorNomeAsync(string nome);

        /// <summary>
        /// Adiciona uma nova área
        /// </summary>
        Task<Area> AdicionarAsync(Area area);

        /// <summary>
        /// Atualiza uma área existente
        /// </summary>
        Task<Area> AtualizarAsync(Area area);

        /// <summary>
        /// Remove uma área (soft delete)
        /// </summary>
        Task<bool> RemoverAsync(int id);

        /// <summary>
        /// Verifica se existe uma área com o nome especificado
        /// </summary>
        Task<bool> ExisteComNomeAsync(string nome, int? idExcluir = null);

        /// <summary>
        /// Obtém estatísticas de presença por área
        /// </summary>
        Task<IEnumerable<dynamic>> ObterEstatisticasPresencaAsync(DateOnly dataInicio, DateOnly dataFim);
    }
}
>>>>>>> b90a182 (Initial commit of DDO project)
