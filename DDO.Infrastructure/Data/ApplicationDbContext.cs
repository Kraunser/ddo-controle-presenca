using DDO.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DDO.Infrastructure.Data
{
    /// <summary>
    /// Contexto principal do banco de dados da aplicação
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        #region DbSets - Entidades do Domínio

        /// <summary>
        /// Áreas da empresa
        /// </summary>
        public DbSet<Area> Areas { get; set; }

        /// <summary>
        /// Colaboradores da empresa
        /// </summary>
        public DbSet<Colaborador> Colaboradores { get; set; }

        /// <summary>
        /// Registros de presença
        /// </summary>
        public DbSet<Presenca> Presencas { get; set; }

        /// <summary>
        /// Arquivos PDF do DDO
        /// </summary>
        public DbSet<ArquivoPDF> ArquivosPDF { get; set; }

        /// <summary>
        /// Logs do sistema para auditoria
        /// </summary>
        public DbSet<LogSistema> LogsSistema { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configurações das entidades
            ConfigurarArea(builder);
            ConfigurarColaborador(builder);
            ConfigurarPresenca(builder);
            ConfigurarArquivoPDF(builder);
            ConfigurarLogSistema(builder);

            // Dados iniciais (Seed Data)
            SeedData(builder);
        }

        /// <summary>
        /// Configurações específicas da entidade Area
        /// </summary>
        private static void ConfigurarArea(ModelBuilder builder)
        {
            builder.Entity<Area>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descricao).HasMaxLength(500);
                entity.Property(e => e.DataCriacao).HasDefaultValueSql("GETUTCDATE()");

                // Índices
                entity.HasIndex(e => e.Nome).IsUnique();
                entity.HasIndex(e => e.Ativa);
            });
        }

        /// <summary>
        /// Configurações específicas da entidade Colaborador
        /// </summary>
        private static void ConfigurarColaborador(ModelBuilder builder)
        {
            builder.Entity<Colaborador>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Matricula).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CodigoRFID).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property(e => e.Telefone).HasMaxLength(20);
                entity.Property(e => e.DataCadastro).HasDefaultValueSql("GETUTCDATE()");

                // Relacionamentos
                entity.HasOne(e => e.Area)
                      .WithMany(a => a.Colaboradores)
                      .HasForeignKey(e => e.AreaId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Índices únicos
                entity.HasIndex(e => e.Matricula).IsUnique();
                entity.HasIndex(e => e.CodigoRFID).IsUnique();
                entity.HasIndex(e => new { e.Email }).IsUnique().HasFilter("[Email] IS NOT NULL");
                
                // Índices de performance
                entity.HasIndex(e => e.Ativo);
                entity.HasIndex(e => e.AreaId);
            });
        }

        /// <summary>
        /// Configurações específicas da entidade Presenca
        /// </summary>
        private static void ConfigurarPresenca(ModelBuilder builder)
        {
            builder.Entity<Presenca>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DataHoraRegistro).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.TipoRegistro).HasMaxLength(50).HasDefaultValue("Entrada");
                entity.Property(e => e.MetodoRegistro).HasMaxLength(50).HasDefaultValue("RFID");
                entity.Property(e => e.Observacoes).HasMaxLength(500);
                entity.Property(e => e.DispositivoOrigem).HasMaxLength(100);

                // Relacionamentos
                entity.HasOne(e => e.Colaborador)
                      .WithMany(c => c.Presencas)
                      .HasForeignKey(e => e.ColaboradorId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Índices de performance
                entity.HasIndex(e => e.ColaboradorId);
                entity.HasIndex(e => e.DataPresenca);
                entity.HasIndex(e => e.DataHoraRegistro);
                entity.HasIndex(e => new { e.ColaboradorId, e.DataPresenca });
                entity.HasIndex(e => e.Validado);

                // Índice composto para evitar registros duplicados no mesmo dia
                entity.HasIndex(e => new { e.ColaboradorId, e.DataPresenca, e.TipoRegistro })
                      .HasDatabaseName("IX_Presenca_ColaboradorId_DataPresenca_TipoRegistro");
            });
        }

        /// <summary>
        /// Configurações específicas da entidade ArquivoPDF
        /// </summary>
        private static void ConfigurarArquivoPDF(ModelBuilder builder)
        {
            builder.Entity<ArquivoPDF>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NomeArquivo).IsRequired().HasMaxLength(255);
                entity.Property(e => e.NomeArquivoSalvo).IsRequired().HasMaxLength(255);
                entity.Property(e => e.CaminhoArquivo).IsRequired().HasMaxLength(500);
                entity.Property(e => e.TipoMime).HasMaxLength(100).HasDefaultValue("application/pdf");
                entity.Property(e => e.DataUpload).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.Descricao).HasMaxLength(1000);
                entity.Property(e => e.HashMD5).HasMaxLength(32);
                entity.Property(e => e.UploadedById).HasMaxLength(450);

                // Relacionamentos
                entity.HasOne(e => e.Area)
                      .WithMany(a => a.ArquivosPDF)
                      .HasForeignKey(e => e.AreaId)
                      .OnDelete(DeleteBehavior.SetNull);

                // Índices
                entity.HasIndex(e => e.DataUpload);
                entity.HasIndex(e => e.DataReferencia);
                entity.HasIndex(e => e.AreaId);
                entity.HasIndex(e => e.Ativo);
                entity.HasIndex(e => e.HashMD5);
            });
        }

        /// <summary>
        /// Configurações específicas da entidade LogSistema
        /// </summary>
        private static void ConfigurarLogSistema(ModelBuilder builder)
        {
            builder.Entity<LogSistema>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DataHora).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.Nivel).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Categoria).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Mensagem).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.UsuarioId).HasMaxLength(450);
                entity.Property(e => e.UsuarioNome).HasMaxLength(200);
                entity.Property(e => e.EnderecoIP).HasMaxLength(45);
                entity.Property(e => e.UserAgent).HasMaxLength(500);
                entity.Property(e => e.Acao).HasMaxLength(500);
                entity.Property(e => e.TipoEntidade).HasMaxLength(100);

                // Índices de performance
                entity.HasIndex(e => e.DataHora);
                entity.HasIndex(e => e.Nivel);
                entity.HasIndex(e => e.Categoria);
                entity.HasIndex(e => e.UsuarioId);
                entity.HasIndex(e => new { e.TipoEntidade, e.EntidadeId });
            });
        }

        /// <summary>
        /// Dados iniciais para popular o banco
        /// </summary>
        private static void SeedData(ModelBuilder builder)
        {
            // Áreas padrão
            builder.Entity<Area>().HasData(
                new Area
                {
                    Id = 1,
                    Nome = "Tecnologia da Informação",
                    Descricao = "Área responsável pela infraestrutura e desenvolvimento de sistemas",
                    Ativa = true,
                    DataCriacao = DateTime.UtcNow
                },
                new Area
                {
                    Id = 2,
                    Nome = "Recursos Humanos",
                    Descricao = "Área responsável pela gestão de pessoas e processos administrativos",
                    Ativa = true,
                    DataCriacao = DateTime.UtcNow
                },
                new Area
                {
                    Id = 3,
                    Nome = "Financeiro",
                    Descricao = "Área responsável pela gestão financeira e contábil",
                    Ativa = true,
                    DataCriacao = DateTime.UtcNow
                },
                new Area
                {
                    Id = 4,
                    Nome = "Operações",
                    Descricao = "Área responsável pelas operações e processos produtivos",
                    Ativa = true,
                    DataCriacao = DateTime.UtcNow
                },
                new Area
                {
                    Id = 5,
                    Nome = "Comercial",
                    Descricao = "Área responsável pelas vendas e relacionamento com clientes",
                    Ativa = true,
                    DataCriacao = DateTime.UtcNow
                }
            );
        }
    }
}
