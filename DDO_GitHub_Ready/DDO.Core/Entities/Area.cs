using System.ComponentModel.DataAnnotations;

namespace DDO.Core.Entities
{
    /// <summary>
    /// Representa uma área da empresa para organização dos colaboradores
    /// </summary>
    public class Area
    {
        /// <summary>
        /// Identificador único da área
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome da área (ex: TI, RH, Financeiro, etc.)
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Descrição opcional da área
        /// </summary>
        [StringLength(500)]
        public string? Descricao { get; set; }

        /// <summary>
        /// Indica se a área está ativa
        /// </summary>
        public bool Ativa { get; set; } = true;

        /// <summary>
        /// Data de criação da área
        /// </summary>
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Colaboradores vinculados a esta área
        /// </summary>
        public virtual ICollection<Colaborador> Colaboradores { get; set; } = new List<Colaborador>();

        /// <summary>
        /// Arquivos PDF associados a esta área
        /// </summary>
        public virtual ICollection<ArquivoPDF> ArquivosPDF { get; set; } = new List<ArquivoPDF>();
    }
}
