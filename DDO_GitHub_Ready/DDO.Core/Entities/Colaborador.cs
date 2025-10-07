using System.ComponentModel.DataAnnotations;

namespace DDO.Core.Entities
{
    /// <summary>
    /// Representa um colaborador da empresa
    /// </summary>
    public class Colaborador
    {
        /// <summary>
        /// Identificador único do colaborador
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome completo do colaborador
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Matrícula única do colaborador na empresa
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Matricula { get; set; } = string.Empty;

        /// <summary>
        /// Código RFID do crachá do colaborador
        /// </summary>
        [Required]
        [StringLength(100)]
        public string CodigoRFID { get; set; } = string.Empty;

        /// <summary>
        /// Email do colaborador
        /// </summary>
        [StringLength(200)]
        public string? Email { get; set; }

        /// <summary>
        /// Telefone do colaborador
        /// </summary>
        [StringLength(20)]
        public string? Telefone { get; set; }

        /// <summary>
        /// Indica se o colaborador está ativo
        /// </summary>
        public bool Ativo { get; set; } = true;

        /// <summary>
        /// Data de cadastro do colaborador
        /// </summary>
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data da última atualização dos dados
        /// </summary>
        public DateTime? DataUltimaAtualizacao { get; set; }

        /// <summary>
        /// ID da área à qual o colaborador pertence
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// Área à qual o colaborador pertence
        /// </summary>
        public virtual Area Area { get; set; } = null!;

        /// <summary>
        /// Registros de presença do colaborador
        /// </summary>
        public virtual ICollection<Presenca> Presencas { get; set; } = new List<Presenca>();
    }
}
