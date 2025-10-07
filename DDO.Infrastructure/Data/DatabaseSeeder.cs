using DDO.Core.Entities;
using DDO.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DDO.Infrastructure.Data
{
    /// <summary>
    /// Classe responsável por popular o banco de dados com dados de teste
    /// </summary>
    public static class DatabaseSeeder
    {
        /// <summary>
        /// Popula o banco de dados com dados de teste se estiver vazio
        /// </summary>
        /// <param name="context">Contexto do banco de dados</param>
        /// <param name="logger">Logger para registrar operações</param>
        public static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                // Verifica se já existem dados no banco
                if (await context.Areas.AnyAsync())
                {
                    logger.LogInformation("Banco de dados já contém dados. Seed cancelado.");
                    return;
                }

                logger.LogInformation("Iniciando população do banco de dados com dados de teste...");

                // Criar áreas
                var areas = await CreateAreasAsync(context);
                
                // Criar colaboradores
                var colaboradores = await CreateColaboradoresAsync(context, areas);
                
                // Criar registros de presença
                await CreatePresencasAsync(context, colaboradores);
                
                // Criar arquivos PDF de exemplo
                await CreateArquivosPDFAsync(context, areas);

                logger.LogInformation("População do banco de dados concluída com sucesso!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao popular o banco de dados com dados de teste.");
                throw;
            }
        }

        /// <summary>
        /// Cria as áreas de exemplo
        /// </summary>
        private static async Task<List<Area>> CreateAreasAsync(ApplicationDbContext context)
        {
            var areas = new List<Area>
            {
                new Area
                {
                    Nome = "Tecnologia da Informação",
                    Descricao = "Área responsável por desenvolvimento de sistemas e infraestrutura de TI",
                    Ativa = true,
                    DataCriacao = DateTime.UtcNow.AddMonths(-6)
                },
                new Area
                {
                    Nome = "Recursos Humanos",
                    Descricao = "Área responsável pela gestão de pessoas e processos de RH",
                    Ativa = true,
                    DataCriacao = DateTime.UtcNow.AddMonths(-5)
                },
                new Area
                {
                    Nome = "Engenharia",
                    Descricao = "Área de desenvolvimento de produtos e soluções de engenharia",
                    Ativa = true,
                    DataCriacao = DateTime.UtcNow.AddMonths(-4)
                },
                new Area
                {
                    Nome = "Vendas",
                    Descricao = "Área comercial responsável pelas vendas e relacionamento com clientes",
                    Ativa = true,
                    DataCriacao = DateTime.UtcNow.AddMonths(-3)
                },
                new Area
                {
                    Nome = "Marketing",
                    Descricao = "Área responsável por marketing digital e comunicação",
                    Ativa = true,
                    DataCriacao = DateTime.UtcNow.AddMonths(-2)
                },
                new Area
                {
                    Nome = "Financeiro",
                    Descricao = "Área responsável pela gestão financeira e contábil",
                    Ativa = true,
                    DataCriacao = DateTime.UtcNow.AddMonths(-1)
                }
            };

            context.Areas.AddRange(areas);
            await context.SaveChangesAsync();
            return areas;
        }

        /// <summary>
        /// Cria os colaboradores de exemplo
        /// </summary>
        private static async Task<List<Colaborador>> CreateColaboradoresAsync(ApplicationDbContext context, List<Area> areas)
        {
            var colaboradores = new List<Colaborador>();
            var random = new Random();

            // Nomes fictícios para os colaboradores
            var nomes = new[]
            {
                "Ana Silva Santos", "Bruno Costa Lima", "Carla Oliveira Souza", "Daniel Ferreira Alves",
                "Eduarda Martins Rocha", "Felipe Rodrigues Nunes", "Gabriela Pereira Castro", "Henrique Santos Dias",
                "Isabela Lima Cardoso", "João Pedro Almeida", "Karina Fernandes Silva", "Lucas Barbosa Mendes",
                "Mariana Gomes Ribeiro", "Nicolas Araújo Costa", "Patrícia Moreira Santos", "Rafael Carvalho Lopes",
                "Sophia Nascimento Cruz", "Thiago Ramos Oliveira", "Vitória Azevedo Pinto", "William Teixeira Sousa",
                "Amanda Correia Freitas", "Carlos Eduardo Melo", "Débora Campos Vieira", "Evandro Silva Monteiro",
                "Fernanda Torres Macedo", "Gustavo Henrique Reis", "Helena Batista Cunha", "Igor Moura Farias",
                "Juliana Rezende Borges", "Kevin Andrade Coelho", "Larissa Viana Duarte", "Mateus Caldeira Pires",
                "Natália Siqueira Moraes", "Otávio Brandão Tavares", "Priscila Nogueira Fonseca"
            };

            var contador = 1;
            foreach (var area in areas)
            {
                // Criar entre 5 e 8 colaboradores por área
                var quantidadeColaboradores = random.Next(5, 9);
                
                for (int i = 0; i < quantidadeColaboradores && contador <= nomes.Length; i++)
                {
                    var colaborador = new Colaborador
                    {
                        Nome = nomes[contador - 1],
                        Matricula = $"RDC{contador:D4}",
                        CodigoRFID = $"RFID{random.Next(100000, 999999):D6}",
                        Email = $"{nomes[contador - 1].Split(' ')[0].ToLower()}.{nomes[contador - 1].Split(' ')[1].ToLower()}@randoncorp.com",
                        Telefone = $"(51) 9{random.Next(1000, 9999)}-{random.Next(1000, 9999)}",
                        Ativo = random.Next(1, 11) > 1, // 90% dos colaboradores ativos
                        DataCadastro = DateTime.UtcNow.AddDays(-random.Next(30, 180)),
                        AreaId = area.Id
                    };

                    colaboradores.Add(colaborador);
                    contador++;
                }
            }

            context.Colaboradores.AddRange(colaboradores);
            await context.SaveChangesAsync();
            return colaboradores;
        }

        /// <summary>
        /// Cria registros de presença de exemplo
        /// </summary>
        private static async Task CreatePresencasAsync(ApplicationDbContext context, List<Colaborador> colaboradores)
        {
            var presencas = new List<Presenca>();
            var random = new Random();

            // Criar presenças para os últimos 30 dias
            var dataInicio = DateTime.Today.AddDays(-30);
            var dataFim = DateTime.Today;

            for (var data = dataInicio; data <= dataFim; data = data.AddDays(1))
            {
                // Pular fins de semana (sábado e domingo)
                if (data.DayOfWeek == DayOfWeek.Saturday || data.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                // Simular que nem todos os colaboradores comparecem todos os dias
                var colaboradoresPresentes = colaboradores
                    .Where(c => c.Ativo && random.Next(1, 11) > 2) // 80% de chance de comparecer
                    .ToList();

                foreach (var colaborador in colaboradoresPresentes)
                {
                    // Criar entre 1 e 3 registros de presença por dia (entrada, saída para almoço, retorno)
                    var numeroRegistros = random.Next(1, 4);
                    
                    for (int i = 0; i < numeroRegistros; i++)
                    {
                        var horaBase = data.AddHours(8 + (i * 4) + random.Next(-30, 31) / 60.0);
                        
                        var presenca = new Presenca
                        {
                            ColaboradorId = colaborador.Id,
                            DataPresenca = horaBase,
                            TipoRegistro = (TipoRegistroPresenca)(i % 2), // Alterna entre Entrada e Saída
                            MetodoRegistro = MetodoRegistro.RFID,
                            LocalRegistro = $"Leitor-{random.Next(1, 6):D2}",
                            Observacoes = i == 0 ? "Entrada principal" : 
                                         i == 1 ? "Saída para almoço" : 
                                         "Retorno do almoço"
                        };

                        presencas.Add(presenca);
                    }
                }
            }

            context.Presencas.AddRange(presencas);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Cria arquivos PDF de exemplo
        /// </summary>
        private static async Task CreateArquivosPDFAsync(ApplicationDbContext context, List<Area> areas)
        {
            var arquivos = new List<ArquivoPDF>();
            var random = new Random();

            var nomesArquivos = new[]
            {
                "DDO_Janeiro_2024.pdf", "DDO_Fevereiro_2024.pdf", "DDO_Marco_2024.pdf",
                "DDO_Abril_2024.pdf", "DDO_Maio_2024.pdf", "DDO_Junho_2024.pdf",
                "DDO_Julho_2024.pdf", "DDO_Agosto_2024.pdf", "DDO_Setembro_2024.pdf",
                "Apresentacao_DDO_TI.pdf", "Relatorio_Presenca_RH.pdf", "Manual_Sistema_DDO.pdf"
            };

            foreach (var nomeArquivo in nomesArquivos)
            {
                var area = areas[random.Next(areas.Count)];
                
                var arquivo = new ArquivoPDF
                {
                    NomeArquivo = nomeArquivo,
                    CaminhoArquivo = $"/uploads/pdfs/{nomeArquivo}",
                    TamanhoBytes = random.Next(500000, 5000000), // Entre 500KB e 5MB
                    TipoConteudo = "application/pdf",
                    DataUpload = DateTime.UtcNow.AddDays(-random.Next(1, 90)),
                    UploadedBy = "admin@randoncorp.com",
                    AreaId = area.Id,
                    Descricao = $"Arquivo DDO da área {area.Nome}",
                    Ativo = true
                };

                arquivos.Add(arquivo);
            }

            context.ArquivosPDF.AddRange(arquivos);
            await context.SaveChangesAsync();
        }
    }
}
