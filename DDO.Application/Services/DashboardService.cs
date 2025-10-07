using DDO.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDO.Application.Services
{
    /// <summary>
    /// Serviço para geração de dados do dashboard
    /// </summary>
    public class DashboardService
    {
        private readonly IPresencaRepository _presencaRepository;
        private readonly IColaboradorRepository _colaboradorRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly IArquivoPDFRepository _arquivoPDFRepository;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IPresencaRepository presencaRepository,
            IColaboradorRepository colaboradorRepository,
            IAreaRepository areaRepository,
            IArquivoPDFRepository arquivoPDFRepository,
            ILogger<DashboardService> logger)
        {
            _presencaRepository = presencaRepository;
            _colaboradorRepository = colaboradorRepository;
            _areaRepository = areaRepository;
            _arquivoPDFRepository = arquivoPDFRepository;
            _logger = logger;
        }

        /// <summary>
        /// Obtém dados completos do dashboard
        /// </summary>
        public async Task<DashboardData> ObterDadosDashboardAsync(DateOnly? dataInicio = null, DateOnly? dataFim = null)
        {
            try
            {
                // Definir período padrão (últimos 30 dias)
                var inicio = dataInicio ?? DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
                var fim = dataFim ?? DateOnly.FromDateTime(DateTime.Now);

                var dashboardData = new DashboardData
                {
                    PeriodoInicio = inicio,
                    PeriodoFim = fim
                };

                // Executar consultas em paralelo para melhor performance
                var tasks = new Task<object>[]
                {
                    ObterEstatisticasGeraisAsync(inicio, fim).ContinueWith(t => (object)t.Result),
                    ObterDadosPresencaPorAreaAsync(inicio, fim).ContinueWith(t => (object)t.Result),
                    ObterTendenciaPresencaAsync(inicio, fim).ContinueWith(t => (object)t.Result),
                    ObterRankingColaboradoresAsync(inicio, fim).ContinueWith(t => (object)t.Result),
                    ObterDistribuicaoHorariaAsync(inicio, fim).ContinueWith(t => (object)t.Result),
                    ObterComparativoPeriodicosAsync(inicio, fim).ContinueWith(t => (object)t.Result),
                    ObterEstatisticasArquivosAsync(inicio, fim).ContinueWith(t => (object)t.Result)
                };

                var resultados = await Task.WhenAll(tasks);

                dashboardData.EstatisticasGerais = (EstatisticasGerais)resultados[0];
                dashboardData.PresencaPorArea = (List<PresencaPorArea>)resultados[1];
                dashboardData.TendenciaPresenca = (List<TendenciaPresenca>)resultados[2];
                dashboardData.RankingColaboradores = (List<RankingColaborador>)resultados[3];
                dashboardData.DistribuicaoHoraria = (List<DistribuicaoHoraria>)resultados[4];
                dashboardData.ComparativoPeriodos = (ComparativoPeriodos)resultados[5];
                dashboardData.EstatisticasArquivos = (EstatisticasArquivos)resultados[6];

                return dashboardData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter dados do dashboard");
                throw;
            }
        }

        /// <summary>
        /// Obtém estatísticas gerais do sistema
        /// </summary>
        private async Task<EstatisticasGerais> ObterEstatisticasGeraisAsync(DateOnly inicio, DateOnly fim)
        {
            var presencas = await _presencaRepository.ObterPorPeriodoAsync(inicio, fim);
            var colaboradores = await _colaboradorRepository.ObterTodosAtivosAsync();
            var areas = await _areaRepository.ObterTodasAtivasAsync();

            var totalPresencas = presencas.Count();
            var colaboradoresComPresenca = presencas.Select(p => p.ColaboradorId).Distinct().Count();
            var areasComPresenca = presencas.Select(p => p.Colaborador.AreaId).Distinct().Count();
            var diasPeriodo = (fim.ToDateTime(TimeOnly.MinValue) - inicio.ToDateTime(TimeOnly.MinValue)).Days + 1;
            var mediaPresencasDiarias = diasPeriodo > 0 ? (double)totalPresencas / diasPeriodo : 0;

            // Presença hoje
            var hoje = DateOnly.FromDateTime(DateTime.Now);
            var presencasHoje = presencas.Count(p => p.DataPresenca == hoje);

            return new EstatisticasGerais
            {
                TotalPresencas = totalPresencas,
                PresencasHoje = presencasHoje,
                TotalColaboradores = colaboradores.Count(),
                ColaboradoresComPresenca = colaboradoresComPresenca,
                TotalAreas = areas.Count(),
                AreasComPresenca = areasComPresenca,
                MediaPresencasDiarias = Math.Round(mediaPresencasDiarias, 1),
                TaxaPresenca = colaboradores.Any() ? Math.Round((double)colaboradoresComPresenca / colaboradores.Count() * 100, 1) : 0
            };
        }

        /// <summary>
        /// Obtém dados de presença por área
        /// </summary>
        private async Task<List<PresencaPorArea>> ObterDadosPresencaPorAreaAsync(DateOnly inicio, DateOnly fim)
        {
            var estatisticasPorArea = await _presencaRepository.ObterEstatisticasPorAreaAsync(inicio, fim);
            
            return estatisticasPorArea.Select(e => new PresencaPorArea
            {
                NomeArea = (string)e.GetType().GetProperty("Area")?.GetValue(e) ?? "",
                TotalPresencas = (int)e.GetType().GetProperty("TotalPresencas")?.GetValue(e),
                ColaboradoresUnicos = (int)e.GetType().GetProperty("ColaboradoresUnicos")?.GetValue(e),
                MediaPresencasDiarias = Math.Round((double)e.GetType().GetProperty("MediaPresencasDiarias")?.GetValue(e), 1)
            }).ToList();
        }

        /// <summary>
        /// Obtém tendência de presença ao longo do tempo
        /// </summary>
        private async Task<List<TendenciaPresenca>> ObterTendenciaPresencaAsync(DateOnly inicio, DateOnly fim)
        {
            var dadosGrafico = await _presencaRepository.ObterDadosGraficoPresencaTemporalAsync(inicio, fim, "dia");
            
            return dadosGrafico.Select(d => new TendenciaPresenca
            {
                Periodo = (string)d.GetType().GetProperty("Periodo")?.GetValue(d) ?? "",
                TotalPresencas = (int)d.GetType().GetProperty("TotalPresencas")?.GetValue(d),
                ColaboradoresUnicos = (int)d.GetType().GetProperty("ColaboradoresUnicos")?.GetValue(d)
            }).ToList();
        }

        /// <summary>
        /// Obtém ranking dos colaboradores mais presentes
        /// </summary>
        private async Task<List<RankingColaborador>> ObterRankingColaboradoresAsync(DateOnly inicio, DateOnly fim)
        {
            var ranking = await _presencaRepository.ObterRankingColaboradoresAsync(inicio, fim, 10);
            
            return ranking.Select(r => new RankingColaborador
            {
                NomeColaborador = (string)r.GetType().GetProperty("NomeColaborador")?.GetValue(r) ?? "",
                Area = (string)r.GetType().GetProperty("Area")?.GetValue(r) ?? "",
                TotalPresencas = (int)r.GetType().GetProperty("TotalPresencas")?.GetValue(r),
                DiasPresentes = (int)r.GetType().GetProperty("DiasPresentes")?.GetValue(r)
            }).ToList();
        }

        /// <summary>
        /// Obtém distribuição horária das presenças
        /// </summary>
        private async Task<List<DistribuicaoHoraria>> ObterDistribuicaoHorariaAsync(DateOnly inicio, DateOnly fim)
        {
            var presencas = await _presencaRepository.ObterPorPeriodoAsync(inicio, fim);
            
            var distribuicao = presencas
                .GroupBy(p => p.HorarioPresenca.Hour)
                .Select(g => new DistribuicaoHoraria
                {
                    Hora = g.Key,
                    TotalPresencas = g.Count(),
                    PorcentagemTotal = presencas.Any() ? Math.Round((double)g.Count() / presencas.Count() * 100, 1) : 0
                })
                .OrderBy(d => d.Hora)
                .ToList();

            // Preencher horas sem registros
            for (int hora = 0; hora < 24; hora++)
            {
                if (!distribuicao.Any(d => d.Hora == hora))
                {
                    distribuicao.Add(new DistribuicaoHoraria
                    {
                        Hora = hora,
                        TotalPresencas = 0,
                        PorcentagemTotal = 0
                    });
                }
            }

            return distribuicao.OrderBy(d => d.Hora).ToList();
        }

        /// <summary>
        /// Obtém comparativo entre períodos
        /// </summary>
        private async Task<ComparativoPeriodos> ObterComparativoPeriodicosAsync(DateOnly inicio, DateOnly fim)
        {
            var diasPeriodo = (fim.ToDateTime(TimeOnly.MinValue) - inicio.ToDateTime(TimeOnly.MinValue)).Days + 1;
            var inicioAnterior = inicio.AddDays(-diasPeriodo);
            var fimAnterior = inicio.AddDays(-1);

            var presencasAtual = await _presencaRepository.ObterPorPeriodoAsync(inicio, fim);
            var presencasAnterior = await _presencaRepository.ObterPorPeriodoAsync(inicioAnterior, fimAnterior);

            var totalAtual = presencasAtual.Count();
            var totalAnterior = presencasAnterior.Count();
            var variacao = totalAnterior > 0 ? Math.Round((double)(totalAtual - totalAnterior) / totalAnterior * 100, 1) : 0;

            var colaboradoresAtual = presencasAtual.Select(p => p.ColaboradorId).Distinct().Count();
            var colaboradoresAnterior = presencasAnterior.Select(p => p.ColaboradorId).Distinct().Count();
            var variacaoColaboradores = colaboradoresAnterior > 0 ? Math.Round((double)(colaboradoresAtual - colaboradoresAnterior) / colaboradoresAnterior * 100, 1) : 0;

            return new ComparativoPeriodos
            {
                PeriodoAtual = $"{inicio:dd/MM} - {fim:dd/MM}",
                PeriodoAnterior = $"{inicioAnterior:dd/MM} - {fimAnterior:dd/MM}",
                PresencasAtual = totalAtual,
                PresencasAnterior = totalAnterior,
                VariacaoPresencas = variacao,
                ColaboradoresAtual = colaboradoresAtual,
                ColaboradoresAnterior = colaboradoresAnterior,
                VariacaoColaboradores = variacaoColaboradores
            };
        }

        /// <summary>
        /// Obtém estatísticas de arquivos PDF
        /// </summary>
        private async Task<EstatisticasArquivos> ObterEstatisticasArquivosAsync(DateOnly inicio, DateOnly fim)
        {
            // TODO: Implementar quando o repositório de arquivos estiver completo
            return new EstatisticasArquivos
            {
                TotalArquivos = 0,
                ArquivosNoPeriodo = 0,
                TamanhoTotalMB = 0,
                ArquivosPorArea = new Dictionary<string, int>()
            };
        }

        /// <summary>
        /// Obtém dados para gráfico de pizza por área
        /// </summary>
        public async Task<List<GraficoPizzaArea>> ObterDadosGraficoPizzaAreaAsync(DateOnly inicio, DateOnly fim)
        {
            var totalPorArea = await _presencaRepository.ObterTotalPresencasPorAreaAsync(inicio, fim);
            var total = totalPorArea.Values.Sum();

            return totalPorArea.Select(kvp => new GraficoPizzaArea
            {
                Area = kvp.Key,
                TotalPresencas = kvp.Value,
                Porcentagem = total > 0 ? Math.Round((double)kvp.Value / total * 100, 1) : 0
            }).OrderByDescending(g => g.TotalPresencas).ToList();
        }

        /// <summary>
        /// Obtém dados para gráfico de barras comparativo mensal
        /// </summary>
        public async Task<List<ComparativoMensal>> ObterComparativoMensalAsync(int meses = 6)
        {
            var resultado = new List<ComparativoMensal>();
            var dataAtual = DateOnly.FromDateTime(DateTime.Now);

            for (int i = meses - 1; i >= 0; i--)
            {
                var mesReferencia = dataAtual.AddMonths(-i);
                var inicioMes = new DateOnly(mesReferencia.Year, mesReferencia.Month, 1);
                var fimMes = inicioMes.AddMonths(1).AddDays(-1);

                var presencas = await _presencaRepository.ObterPorPeriodoAsync(inicioMes, fimMes);
                var totalPresencas = presencas.Count();
                var colaboradoresUnicos = presencas.Select(p => p.ColaboradorId).Distinct().Count();

                resultado.Add(new ComparativoMensal
                {
                    Mes = mesReferencia.ToString("MMM/yyyy"),
                    TotalPresencas = totalPresencas,
                    ColaboradoresUnicos = colaboradoresUnicos,
                    MediaDiaria = Math.Round((double)totalPresencas / DateTime.DaysInMonth(mesReferencia.Year, mesReferencia.Month), 1)
                });
            }

            return resultado;
        }
    }

    // Modelos de dados para o dashboard
    public class DashboardData
    {
        public DateOnly PeriodoInicio { get; set; }
        public DateOnly PeriodoFim { get; set; }
        public EstatisticasGerais EstatisticasGerais { get; set; } = new();
        public List<PresencaPorArea> PresencaPorArea { get; set; } = new();
        public List<TendenciaPresenca> TendenciaPresenca { get; set; } = new();
        public List<RankingColaborador> RankingColaboradores { get; set; } = new();
        public List<DistribuicaoHoraria> DistribuicaoHoraria { get; set; } = new();
        public ComparativoPeriodos ComparativoPeriodos { get; set; } = new();
        public EstatisticasArquivos EstatisticasArquivos { get; set; } = new();
    }

    public class EstatisticasGerais
    {
        public int TotalPresencas { get; set; }
        public int PresencasHoje { get; set; }
        public int TotalColaboradores { get; set; }
        public int ColaboradoresComPresenca { get; set; }
        public int TotalAreas { get; set; }
        public int AreasComPresenca { get; set; }
        public double MediaPresencasDiarias { get; set; }
        public double TaxaPresenca { get; set; }
    }

    public class PresencaPorArea
    {
        public string NomeArea { get; set; } = "";
        public int TotalPresencas { get; set; }
        public int ColaboradoresUnicos { get; set; }
        public double MediaPresencasDiarias { get; set; }
    }

    public class TendenciaPresenca
    {
        public string Periodo { get; set; } = "";
        public int TotalPresencas { get; set; }
        public int ColaboradoresUnicos { get; set; }
    }

    public class RankingColaborador
    {
        public string NomeColaborador { get; set; } = "";
        public string Area { get; set; } = "";
        public int TotalPresencas { get; set; }
        public int DiasPresentes { get; set; }
    }

    public class DistribuicaoHoraria
    {
        public int Hora { get; set; }
        public int TotalPresencas { get; set; }
        public double PorcentagemTotal { get; set; }
    }

    public class ComparativoPeriodos
    {
        public string PeriodoAtual { get; set; } = "";
        public string PeriodoAnterior { get; set; } = "";
        public int PresencasAtual { get; set; }
        public int PresencasAnterior { get; set; }
        public double VariacaoPresencas { get; set; }
        public int ColaboradoresAtual { get; set; }
        public int ColaboradoresAnterior { get; set; }
        public double VariacaoColaboradores { get; set; }
    }

    public class EstatisticasArquivos
    {
        public int TotalArquivos { get; set; }
        public int ArquivosNoPeriodo { get; set; }
        public double TamanhoTotalMB { get; set; }
        public Dictionary<string, int> ArquivosPorArea { get; set; } = new();
    }

    public class GraficoPizzaArea
    {
        public string Area { get; set; } = "";
        public int TotalPresencas { get; set; }
        public double Porcentagem { get; set; }
    }

    public class ComparativoMensal
    {
        public string Mes { get; set; } = "";
        public int TotalPresencas { get; set; }
        public int ColaboradoresUnicos { get; set; }
        public double MediaDiaria { get; set; }
    }
}
