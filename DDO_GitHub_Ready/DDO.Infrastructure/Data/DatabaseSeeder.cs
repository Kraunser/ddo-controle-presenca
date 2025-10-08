using DDO.Core.Entities;
using DDO.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DDO.Infrastructure.Data
{
    /// <summary>
    /// Classe responsável por popular o banco de dados com dados iniciais
    /// </summary>
    public static class DatabaseSeeder
    {
        /// <summary>
        /// Popula o banco de dados com dados iniciais
        /// </summary>
        /// <param name="context">Contexto do banco de dados</param>
        /// <param name="userManager">Gerenciador de usuários</param>
        /// <param name="roleManager">Gerenciador de roles</param>
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            try
            {
                // Garantir que o banco foi criado
                await context.Database.EnsureCreatedAsync();

                // Criar roles se não existirem
                await SeedRolesAsync(roleManager);

                // Criar usuário administrador se não existir
                await SeedAdminUserAsync(userManager);

                // Criar áreas se não existirem
                await SeedAreasAsync(context);

                // Criar colaboradores de exemplo se não existirem
                await SeedColaboradoresAsync(context);

                // Criar registros de presença de exemplo se não existirem
                await SeedPresencasAsync(context);

                // Criar arquivos PDF de exemplo se não existirem
                await SeedArquivosPDFAsync(context);

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log do erro (em produção, usar um logger apropriado)
                Console.WriteLine($"Erro ao popular banco de dados: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Cria as roles do sistema
        /// </summary>
        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Manager", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        /// <summary>
        /// Cria o usuário administrador padrão
        /// </summary>
        private static async Task SeedAdminUserAsync(UserManager<IdentityUser> userManager)
        {
            var adminEmail = "admin@randoncorp.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123456");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        /// <summary>
        /// Cria as áreas da empresa
        /// </summary>
        private static async Task SeedAreasAsync(ApplicationDbContext context)
        {
            if (!await context.Areas.AnyAsync())
            {
                var areas = new List<Area>
                {
                    new Area { Nome = "Tecnologia da Informação", Descricao = "Área responsável pela infraestrutura e sistemas", Ativa = true },
                    new Area { Nome = "Recursos Humanos", Descricao = "Gestão de pessoas e desenvolvimento organizacional", Ativa = true },
                    new Area { Nome = "Engenharia", Descricao = "Desenvolvimento de produtos e processos", Ativa = true },
                    new Area { Nome = "Vendas", Descricao = "Comercialização e relacionamento com clientes", Ativa = true },
                    new Area { Nome = "Marketing", Descricao = "Comunicação e estratégias de mercado", Ativa = true },
                    new Area { Nome = "Financeiro", Descricao = "Gestão financeira e contábil", Ativa = true }
                };

                await context.Areas.AddRangeAsync(areas);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Cria colaboradores de exemplo
        /// </summary>
        private static async Task SeedColaboradoresAsync(ApplicationDbContext context)
        {
            if (!await context.Colaboradores.AnyAsync())
            {
                var areas = await context.Areas.ToListAsync();
                var colaboradores = new List<Colaborador>();

                // Colaboradores para TI
                var areaIT = areas.FirstOrDefault(a => a.Nome.Contains("Tecnologia"));
                if (areaIT != null)
                {
                    colaboradores.AddRange(new[]
                    {
                        new Colaborador { Nome = "João Silva", Matricula = "TI001", CodigoRFID = "1234567890", Email = "joao.silva@randoncorp.com", AreaId = areaIT.Id, Ativo = true },
                        new Colaborador { Nome = "Maria Santos", Matricula = "TI002", CodigoRFID = "1234567891", Email = "maria.santos@randoncorp.com", AreaId = areaIT.Id, Ativo = true },
                        new Colaborador { Nome = "Pedro Oliveira", Matricula = "TI003", CodigoRFID = "1234567892", Email = "pedro.oliveira@randoncorp.com", AreaId = areaIT.Id, Ativo = true }
                    });
                }

                // Colaboradores para RH
                var areaRH = areas.FirstOrDefault(a => a.Nome.Contains("Recursos"));
                if (areaRH != null)
                {
                    colaboradores.AddRange(new[]
                    {
                        new Colaborador { Nome = "Ana Costa", Matricula = "RH001", CodigoRFID = "1234567893", Email = "ana.costa@randoncorp.com", AreaId = areaRH.Id, Ativo = true },
                        new Colaborador { Nome = "Carlos Ferreira", Matricula = "RH002", CodigoRFID = "1234567894", Email = "carlos.ferreira@randoncorp.com", AreaId = areaRH.Id, Ativo = true }
                    });
                }

                // Colaboradores para Engenharia
                var areaEng = areas.FirstOrDefault(a => a.Nome.Contains("Engenharia"));
                if (areaEng != null)
                {
                    colaboradores.AddRange(new[]
                    {
                        new Colaborador { Nome = "Roberto Lima", Matricula = "ENG001", CodigoRFID = "1234567895", Email = "roberto.lima@randoncorp.com", AreaId = areaEng.Id, Ativo = true },
                        new Colaborador { Nome = "Fernanda Rocha", Matricula = "ENG002", CodigoRFID = "1234567896", Email = "fernanda.rocha@randoncorp.com", AreaId = areaEng.Id, Ativo = true },
                        new Colaborador { Nome = "Lucas Almeida", Matricula = "ENG003", CodigoRFID = "1234567897", Email = "lucas.almeida@randoncorp.com", AreaId = areaEng.Id, Ativo = true }
                    });
                }

                // Colaboradores para Vendas
                var areaVendas = areas.FirstOrDefault(a => a.Nome.Contains("Vendas"));
                if (areaVendas != null)
                {
                    colaboradores.AddRange(new[]
                    {
                        new Colaborador { Nome = "Juliana Martins", Matricula = "VEN001", CodigoRFID = "1234567898", Email = "juliana.martins@randoncorp.com", AreaId = areaVendas.Id, Ativo = true },
                        new Colaborador { Nome = "Ricardo Souza", Matricula = "VEN002", CodigoRFID = "1234567899", Email = "ricardo.souza@randoncorp.com", AreaId = areaVendas.Id, Ativo = true }
                    });
                }

                await context.Colaboradores.AddRangeAsync(colaboradores);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Cria registros de presença de exemplo
        /// </summary>
        private static async Task SeedPresencasAsync(ApplicationDbContext context)
        {
            if (!await context.Presencas.AnyAsync())
            {
                var colaboradores = await context.Colaboradores.Take(5).ToListAsync();
                var presencas = new List<Presenca>();
                var random = new Random();

                // Criar presenças para os últimos 7 dias
                for (int dia = 0; dia < 7; dia++)
                {
                    var data = DateOnly.FromDateTime(DateTime.Today.AddDays(-dia));
                    
                    foreach (var colaborador in colaboradores)
                    {
                        // Entrada manhã
                        var horaEntrada = new TimeOnly(8, random.Next(0, 30)); // Entre 8:00 e 8:30
                        presencas.Add(new Presenca
                        {
                            ColaboradorId = colaborador.Id,
                            DataPresenca = data,
                            HorarioPresenca = horaEntrada,
                            DataHoraRegistro = data.ToDateTime(horaEntrada),
                            TipoRegistro = "Entrada",
                            MetodoRegistro = "RFID",
                            Validado = true,
                            DispositivoOrigem = "Leitor-Principal"
                        });

                        // Saída almoço
                        var horaSaidaAlmoco = new TimeOnly(12, random.Next(0, 30)); // Entre 12:00 e 12:30
                        presencas.Add(new Presenca
                        {
                            ColaboradorId = colaborador.Id,
                            DataPresenca = data,
                            HorarioPresenca = horaSaidaAlmoco,
                            DataHoraRegistro = data.ToDateTime(horaSaidaAlmoco),
                            TipoRegistro = "Saída Almoço",
                            MetodoRegistro = "RFID",
                            Validado = true,
                            DispositivoOrigem = "Leitor-Principal"
                        });

                        // Volta almoço
                        var horaVoltaAlmoco = new TimeOnly(13, random.Next(0, 30)); // Entre 13:00 e 13:30
                        presencas.Add(new Presenca
                        {
                            ColaboradorId = colaborador.Id,
                            DataPresenca = data,
                            HorarioPresenca = horaVoltaAlmoco,
                            DataHoraRegistro = data.ToDateTime(horaVoltaAlmoco),
                            TipoRegistro = "Volta Almoço",
                            MetodoRegistro = "RFID",
                            Validado = true,
                            DispositivoOrigem = "Leitor-Principal"
                        });

                        // Saída final
                        var horaSaida = new TimeOnly(17, random.Next(30, 60)); // Entre 17:30 e 18:00
                        presencas.Add(new Presenca
                        {
                            ColaboradorId = colaborador.Id,
                            DataPresenca = data,
                            HorarioPresenca = horaSaida,
                            DataHoraRegistro = data.ToDateTime(horaSaida),
                            TipoRegistro = "Saída",
                            MetodoRegistro = "RFID",
                            Validado = true,
                            DispositivoOrigem = "Leitor-Principal"
                        });
                    }
                }

                await context.Presencas.AddRangeAsync(presencas);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Cria arquivos PDF de exemplo
        /// </summary>
        private static async Task SeedArquivosPDFAsync(ApplicationDbContext context)
        {
            if (!await context.ArquivosPDF.AnyAsync())
            {
                var areas = await context.Areas.ToListAsync();
                var arquivos = new List<ArquivoPDF>();

                foreach (var area in areas)
                {
                    arquivos.Add(new ArquivoPDF
                    {
                        NomeArquivo = $"DDO_{area.Nome.Replace(" ", "_")}_2024.pdf",
                        NomeArquivoSalvo = $"ddo_{area.Id}_{Guid.NewGuid()}.pdf",
                        CaminhoArquivo = $"/uploads/pdfs/ddo_{area.Id}_{Guid.NewGuid()}.pdf",
                        TamanhoArquivo = 1024 * 1024, // 1MB
                        TipoMime = "application/pdf",
                        DataUpload = DateTime.UtcNow.AddDays(-new Random().Next(1, 30)),
                        DataReferencia = DateOnly.FromDateTime(DateTime.Today.AddDays(-new Random().Next(1, 30))),
                        Descricao = $"Arquivo DDO da área {area.Nome}",
                        Ativo = true,
                        AreaId = area.Id,
                        NumeroVisualizacoes = new Random().Next(0, 50)
                    });
                }

                await context.ArquivosPDF.AddRangeAsync(arquivos);
                await context.SaveChangesAsync();
            }
        }
    }
}
