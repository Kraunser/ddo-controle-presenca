namespace DDO.Core.Enums
{
    /// <summary>
    /// Métodos disponíveis para registro de presença
    /// </summary>
    public enum MetodoRegistro
    {
        /// <summary>
        /// Registro via leitor RFID
        /// </summary>
        RFID = 1,

        /// <summary>
        /// Registro manual pelo sistema web
        /// </summary>
        Manual = 2,

        /// <summary>
        /// Registro via aplicativo móvel
        /// </summary>
        Mobile = 3,

        /// <summary>
        /// Registro via API externa
        /// </summary>
        API = 4,

        /// <summary>
        /// Importação em lote
        /// </summary>
        ImportacaoLote = 5
    }
}
