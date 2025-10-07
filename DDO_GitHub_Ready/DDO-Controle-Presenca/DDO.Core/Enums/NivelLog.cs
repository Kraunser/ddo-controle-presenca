namespace DDO.Core.Enums
{
    /// <summary>
    /// Níveis de log disponíveis no sistema
    /// </summary>
    public enum NivelLog
    {
        /// <summary>
        /// Informação geral
        /// </summary>
        Info = 1,

        /// <summary>
        /// Aviso ou alerta
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Erro não crítico
        /// </summary>
        Error = 3,

        /// <summary>
        /// Erro crítico
        /// </summary>
        Critical = 4,

        /// <summary>
        /// Debug (apenas em desenvolvimento)
        /// </summary>
        Debug = 5,

        /// <summary>
        /// Trace detalhado
        /// </summary>
        Trace = 6
    }
}
