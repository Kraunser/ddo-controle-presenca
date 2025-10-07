<<<<<<< HEAD
using DDO.Core.Entities;

namespace DDO.Application.Interfaces
{
    /// <summary>
    /// Interface para repositório de colaboradores
    /// </summary>
    public interface IColaboradorRepository
    {
        /// <summary>
        /// Obtém todos os colaboradores ativos
        /// </summary>
        Task<IEnumerable<Colaborador>> ObterTodosAtivosAsync();

        /// <summary>
        /// Obtém um colaborador por ID
        /// </summary>
        Task<Colaborador?> ObterPorIdAsync(int id);

        /// <summary>
        /// Obtém um colaborador por matrícula
        /// </summary>
        Task<Colaborador?> ObterPorMatriculaAsync(string matricula);

        /// <summary>
        /// Obtém um colaborador por código RFID
        /// </summary>
        Task<Colaborador?> ObterPorRFIDAsync(string codigoRFID);

        /// <summary>
        /// Obtém colaboradores por área
        /// </summary>
        Task<IEnumerable<Colaborador>> ObterPorAreaAsync(int areaId);

        /// <summary>
        /// Adiciona um novo colaborador
        /// </summary>
        Task<Colaborador> AdicionarAsync(Colaborador colaborador);

        /// <summary>
        /// Atualiza um colaborador existente
        /// </summary>
        Task<Colaborador> AtualizarAsync(Colaborador colaborador);

        /// <summary>
        /// Remove um colaborador (soft delete)
        /// </summary>
        Task<bool> RemoverAsync(int id);

        /// <summary>
        /// Verifica se existe um colaborador com a matrícula especificada
        /// </summary>
        Task<bool> ExisteComMatriculaAsync(string matricula, int? idExcluir = null);

        /// <summary>
        /// Verifica se existe um colaborador com o código RFID especificado
        /// </summary>
        Task<bool> ExisteComRFIDAsync(string codigoRFID, int? idExcluir = null);

        /// <summary>
        /// Importa colaboradores em lote a partir de uma lista
        /// </summary>
        Task<(int Sucesso, int Erro, List<string> Erros)> ImportarLoteAsync(IEnumerable<Colaborador> colaboradores);

        /// <summary>
        /// Busca colaboradores por termo (nome, matrícula, email)
        /// </summary>
        Task<IEnumerable<Colaborador>> BuscarAsync(string termo);

        /// <summary>
        /// Obtém colaboradores com paginação
        /// </summary>
        Task<(IEnumerable<Colaborador> Colaboradores, int Total)> ObterComPaginacaoAsync(
            int pagina, int tamanhoPagina, string? filtroNome = null, int? areaId = null);
    }
}
=======
using DDO.Core.Entities;

namespace DDO.Application.Interfaces
{
    /// <summary>
    /// Interface para repositório de colaboradores
    /// </summary>
    public interface IColaboradorRepository
    {
        /// <summary>
        /// Obtém todos os colaboradores ativos
        /// </summary>
        Task<IEnumerable<Colaborador>> ObterTodosAtivosAsync();

        /// <summary>
        /// Obtém um colaborador por ID
        /// </summary>
        Task<Colaborador?> ObterPorIdAsync(int id);

        /// <summary>
        /// Obtém um colaborador por matrícula
        /// </summary>
        Task<Colaborador?> ObterPorMatriculaAsync(string matricula);

        /// <summary>
        /// Obtém um colaborador por código RFID
        /// </summary>
        Task<Colaborador?> ObterPorRFIDAsync(string codigoRFID);

        /// <summary>
        /// Obtém colaboradores por área
        /// </summary>
        Task<IEnumerable<Colaborador>> ObterPorAreaAsync(int areaId);

        /// <summary>
        /// Adiciona um novo colaborador
        /// </summary>
        Task<Colaborador> AdicionarAsync(Colaborador colaborador);

        /// <summary>
        /// Atualiza um colaborador existente
        /// </summary>
        Task<Colaborador> AtualizarAsync(Colaborador colaborador);

        /// <summary>
        /// Remove um colaborador (soft delete)
        /// </summary>
        Task<bool> RemoverAsync(int id);

        /// <summary>
        /// Verifica se existe um colaborador com a matrícula especificada
        /// </summary>
        Task<bool> ExisteComMatriculaAsync(string matricula, int? idExcluir = null);

        /// <summary>
        /// Verifica se existe um colaborador com o código RFID especificado
        /// </summary>
        Task<bool> ExisteComRFIDAsync(string codigoRFID, int? idExcluir = null);

        /// <summary>
        /// Importa colaboradores em lote a partir de uma lista
        /// </summary>
        Task<(int Sucesso, int Erro, List<string> Erros)> ImportarLoteAsync(IEnumerable<Colaborador> colaboradores);

        /// <summary>
        /// Busca colaboradores por termo (nome, matrícula, email)
        /// </summary>
        Task<IEnumerable<Colaborador>> BuscarAsync(string termo);

        /// <summary>
        /// Obtém colaboradores com paginação
        /// </summary>
        Task<(IEnumerable<Colaborador> Colaboradores, int Total)> ObterComPaginacaoAsync(
            int pagina, int tamanhoPagina, string? filtroNome = null, int? areaId = null);
    }
}
>>>>>>> b90a182 (Initial commit of DDO project)
