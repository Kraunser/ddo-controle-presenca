<<<<<<< HEAD
using System.ComponentModel.DataAnnotations;

namespace DDO.Core.Entities
{
    /// <summary>
    /// Representa um arquivo PDF do DDO carregado no sistema
    /// </summary>
    public class ArquivoPDF
    {
        /// <summary>
        /// Identificador único do arquivo
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome original do arquivo
        /// </summary>
        [Required]
        [StringLength(255)]
        public string NomeArquivo { get; set; } = string.Empty;

        /// <summary>
        /// Nome do arquivo salvo no servidor (pode ser diferente do original)
        /// </summary>
        [Required]
        [StringLength(255)]
        public string NomeArquivoSalvo { get; set; } = string.Empty;

        /// <summary>
        /// Caminho completo do arquivo no servidor
        /// </summary>
        [Required]
        [StringLength(500)]
        public string CaminhoArquivo { get; set; } = string.Empty;

        /// <summary>
        /// Tamanho do arquivo em bytes
        /// </summary>
        public long TamanhoArquivo { get; set; }

        /// <summary>
        /// Tipo MIME do arquivo
        /// </summary>
        [StringLength(100)]
        public string TipoMime { get; set; } = "application/pdf";

        /// <summary>
        /// Data de upload do arquivo
        /// </summary>
        public DateTime DataUpload { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data de referência do DDO (data do documento)
        /// </summary>
        public DateOnly? DataReferencia { get; set; }

        /// <summary>
        /// Descrição ou observações sobre o arquivo
        /// </summary>
        [StringLength(1000)]
        public string? Descricao { get; set; }

        /// <summary>
        /// Indica se o arquivo está ativo/disponível
        /// </summary>
        public bool Ativo { get; set; } = true;

        /// <summary>
        /// Hash MD5 do arquivo para verificação de integridade
        /// </summary>
        [StringLength(32)]
        public string? HashMD5 { get; set; }

        /// <summary>
        /// ID da área relacionada ao arquivo
        /// </summary>
        public int? AreaId { get; set; }

        /// <summary>
        /// Área relacionada ao arquivo
        /// </summary>
        public virtual Area? Area { get; set; }

        /// <summary>
        /// ID do usuário que fez o upload
        /// </summary>
        [StringLength(450)] // Tamanho padrão do ID do Identity
        public string? UploadedById { get; set; }

        /// <summary>
        /// Número de downloads/visualizações do arquivo
        /// </summary>
        public int NumeroVisualizacoes { get; set; } = 0;

        /// <summary>
        /// Data da última visualização
        /// </summary>
        public DateTime? DataUltimaVisualizacao { get; set; }
    }
}
=======
using System.ComponentModel.DataAnnotations;

namespace DDO.Core.Entities
{
    /// <summary>
    /// Representa um arquivo PDF do DDO carregado no sistema
    /// </summary>
    public class ArquivoPDF
    {
        /// <summary>
        /// Identificador único do arquivo
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome original do arquivo
        /// </summary>
        [Required]
        [StringLength(255)]
        public string NomeArquivo { get; set; } = string.Empty;

        /// <summary>
        /// Nome do arquivo salvo no servidor (pode ser diferente do original)
        /// </summary>
        [Required]
        [StringLength(255)]
        public string NomeArquivoSalvo { get; set; } = string.Empty;

        /// <summary>
        /// Caminho completo do arquivo no servidor
        /// </summary>
        [Required]
        [StringLength(500)]
        public string CaminhoArquivo { get; set; } = string.Empty;

        /// <summary>
        /// Tamanho do arquivo em bytes
        /// </summary>
        public long TamanhoArquivo { get; set; }

        /// <summary>
        /// Tipo MIME do arquivo
        /// </summary>
        [StringLength(100)]
        public string TipoMime { get; set; } = "application/pdf";

        /// <summary>
        /// Data de upload do arquivo
        /// </summary>
        public DateTime DataUpload { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Data de referência do DDO (data do documento)
        /// </summary>
        public DateOnly? DataReferencia { get; set; }

        /// <summary>
        /// Descrição ou observações sobre o arquivo
        /// </summary>
        [StringLength(1000)]
        public string? Descricao { get; set; }

        /// <summary>
        /// Indica se o arquivo está ativo/disponível
        /// </summary>
        public bool Ativo { get; set; } = true;

        /// <summary>
        /// Hash MD5 do arquivo para verificação de integridade
        /// </summary>
        [StringLength(32)]
        public string? HashMD5 { get; set; }

        /// <summary>
        /// ID da área relacionada ao arquivo
        /// </summary>
        public int? AreaId { get; set; }

        /// <summary>
        /// Área relacionada ao arquivo
        /// </summary>
        public virtual Area? Area { get; set; }

        /// <summary>
        /// ID do usuário que fez o upload
        /// </summary>
        [StringLength(450)] // Tamanho padrão do ID do Identity
        public string? UploadedById { get; set; }

        /// <summary>
        /// Número de downloads/visualizações do arquivo
        /// </summary>
        public int NumeroVisualizacoes { get; set; } = 0;

        /// <summary>
        /// Data da última visualização
        /// </summary>
        public DateTime? DataUltimaVisualizacao { get; set; }
    }
}
>>>>>>> b90a182 (Initial commit of DDO project)
