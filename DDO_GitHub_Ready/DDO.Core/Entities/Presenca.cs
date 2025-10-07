using System.ComponentModel.DataAnnotations;

namespace DDO.Core.Entities
{
    /// <summary>
    /// Representa um registro de presença de um colaborador
    /// </summary>
    public class Presenca
    {
        /// <summary>
        /// Identificador único do registro de presença
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Data e hora do registro de presença
        /// </summary>
        public DateTime DataHoraRegistro { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data da presença (sem horário, para facilitar consultas por dia)
        /// </summary>
        public DateOnly DataPresenca { get; set; }

        /// <summary>
        /// Horário da presença (sem data, para facilitar análises de horário)
        /// </summary>
        public TimeOnly HorarioPresenca { get; set; }

        /// <summary>
        /// Tipo de registro (Entrada, Saída, etc.)
        /// </summary>
        [StringLength(50)]
        public string TipoRegistro { get; set; } = "Entrada";

        /// <summary>
        /// Observações sobre o registro
        /// </summary>
        [StringLength(500)]
        public string? Observacoes { get; set; }

        /// <summary>
        /// Indica se o registro foi validado/aprovado
        /// </summary>
        public bool Validado { get; set; } = true;

        /// <summary>
        /// ID do colaborador que registrou a presença
        /// </summary>
        public int ColaboradorId { get; set; }

        /// <summary>
        /// Colaborador que registrou a presença
        /// </summary>
        public virtual Colaborador Colaborador { get; set; } = null!;

        /// <summary>
        /// Método de registro (RFID, Manual, etc.)
        /// </summary>
        [StringLength(50)]
        public string MetodoRegistro { get; set; } = "RFID";

        /// <summary>
        /// IP ou identificação do dispositivo que registrou
        /// </summary>
        [StringLength(100)]
        public string? DispositivoOrigem { get; set; }
    }
}
