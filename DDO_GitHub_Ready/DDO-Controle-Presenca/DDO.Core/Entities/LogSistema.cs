using System.ComponentModel.DataAnnotations;

namespace DDO.Core.Entities
{
    /// <summary>
    /// Representa um log de eventos do sistema para auditoria
    /// </summary>
    public class LogSistema
    {
        /// <summary>
        /// Identificador único do log
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Data e hora do evento
        /// </summary>
        public DateTime DataHora { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Nível do log (Info, Warning, Error, etc.)
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Nivel { get; set; } = string.Empty;

        /// <summary>
        /// Categoria ou módulo do sistema
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Categoria { get; set; } = string.Empty;

        /// <summary>
        /// Mensagem do log
        /// </summary>
        [Required]
        [StringLength(2000)]
        public string Mensagem { get; set; } = string.Empty;

        /// <summary>
        /// Detalhes adicionais (stack trace, dados JSON, etc.)
        /// </summary>
        public string? Detalhes { get; set; }

        /// <summary>
        /// ID do usuário relacionado ao evento (se aplicável)
        /// </summary>
        [StringLength(450)]
        public string? UsuarioId { get; set; }

        /// <summary>
        /// Nome do usuário relacionado ao evento
        /// </summary>
        [StringLength(200)]
        public string? UsuarioNome { get; set; }

        /// <summary>
        /// Endereço IP de origem
        /// </summary>
        [StringLength(45)]
        public string? EnderecoIP { get; set; }

        /// <summary>
        /// User Agent do navegador/aplicação
        /// </summary>
        [StringLength(500)]
        public string? UserAgent { get; set; }

        /// <summary>
        /// URL ou ação que gerou o evento
        /// </summary>
        [StringLength(500)]
        public string? Acao { get; set; }

        /// <summary>
        /// ID da entidade relacionada (se aplicável)
        /// </summary>
        public int? EntidadeId { get; set; }

        /// <summary>
        /// Tipo da entidade relacionada
        /// </summary>
        [StringLength(100)]
        public string? TipoEntidade { get; set; }
    }
}
