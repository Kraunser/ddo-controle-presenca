using DDO.Application.Interfaces;
using DDO.Core.Entities;
using DDO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DDO.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação do repositório de presenças
    /// </summary>
    public class PresencaRepository : IPresencaRepository
    {
        private readonly ApplicationDbContext _context;

        public PresencaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Presenca>> ObterPorPeriodoAsync(DateOnly dataInicio, DateOnly dataFim)
        {
            return await _context.Presencas
                .Include(p => p.Colaborador)
                .ThenInclude(c => c.Area)
                .Where(p => p.DataPresenca >= dataInicio && p.DataPresenca <= dataFim)
                .OrderByDescending(p => p.DataHoraRegistro)
                .ToListAsync();
        }

        public async Task<IEnumerable<Presenca>> ObterPorColaboradorPeriodoAsync(int colaboradorId, DateOnly dataInicio, DateOnly dataFim)
        {
            return await _context.Presencas
                .Include(p => p.Colaborador)
                .ThenInclude(c => c.Area)
                .Where(p => p.ColaboradorId == colaboradorId && 
                           p.DataPresenca >= dataInicio && 
                           p.DataPresenca <= dataFim)
                .OrderByDescending(p => p.DataHoraRegistro)
                .ToListAsync();
        }

        public async Task<IEnumerable<Presenca>> ObterPorAreaPeriodoAsync(int areaId, DateOnly dataInicio, DateOnly dataFim)
        {
            return await _context.Presencas
                .Include(p => p.Colaborador)
                .ThenInclude(c => c.Area)
                .Where(p => p.Colaborador.AreaId == areaId && 
                           p.DataPresenca >= dataInicio && 
                           p.DataPresenca <= dataFim)
                .OrderByDescending(p => p.DataHoraRegistro)
                .ToListAsync();
        }

        public async Task<Presenca?> ObterPorIdAsync(int id)
        {
            return await _context.Presencas
                .Include(p => p.Colaborador)
                .ThenInclude(c => c.Area)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Presenca> RegistrarPresencaAsync(Presenca presenca)
        {
            presenca.DataHoraRegistro = DateTime.UtcNow;
            presenca.DataPresenca = DateOnly.FromDateTime(presenca.DataHoraRegistro);
            presenca.HorarioPresenca = TimeOnly.FromDateTime(presenca.DataHoraRegistro);
            
            _context.Presencas.Add(presenca);
            await _context.SaveChangesAsync();
            return presenca;
        }

        public async Task<Presenca> AtualizarAsync(Presenca presenca)
        {
            _context.Entry(presenca).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return presenca;
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var presenca = await _context.Presencas.FindAsync(id);
            if (presenca == null) return false;

            _context.Presencas.Remove(presenca);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistePresencaNaDataAsync(int colaboradorId, DateOnly data, string? tipoRegistro = null)
        {
            var query = _context.Presencas
                .Where(p => p.ColaboradorId == colaboradorId && p.DataPresenca == data);

            if (!string.IsNullOrEmpty(tipoRegistro))
            {
                query = query.Where(p => p.TipoRegistro == tipoRegistro);
            }

            return await query.AnyAsync();
        }

        public async Task<Presenca?> ObterUltimaPresencaColaboradorAsync(int colaboradorId)
        {
            return await _context.Presencas
                .Include(p => p.Colaborador)
                .ThenInclude(c => c.Area)
                .Where(p => p.ColaboradorId == colaboradorId)
                .OrderByDescending(p => p.DataHoraRegistro)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<dynamic>> ObterEstatisticasPorAreaAsync(DateOnly dataInicio, DateOnly dataFim)
        {
            return await _context.Presencas
                .Include(p => p.Colaborador)
                .ThenInclude(c => c.Area)
                .Where(p => p.DataPresenca >= dataInicio && p.DataPresenca <= dataFim)
                .GroupBy(p => p.Colaborador.Area!.Nome)
                .Select(g => new
                {
                    Area = g.Key,
                    TotalPresencas = g.Count(),
                    ColaboradoresUnicos = g.Select(p => p.ColaboradorId).Distinct().Count(),
                    MediaPresencasDiarias = g.GroupBy(p => p.DataPresenca).Average(d => (double)d.Count())
                })
                .OrderByDescending(x => x.TotalPresencas)
                .ToListAsync();
        }

        public async Task<IEnumerable<dynamic>> ObterEstatisticasPorDiaAsync(DateOnly dataInicio, DateOnly dataFim)
        {
            return await _context.Presencas
                .Where(p => p.DataPresenca >= dataInicio && p.DataPresenca <= dataFim)
                .GroupBy(p => p.DataPresenca)
                .Select(g => new
                {
                    Data = g.Key,
                    TotalPresencas = g.Count(),
                    ColaboradoresUnicos = g.Select(p => p.ColaboradorId).Distinct().Count()
                })
                .OrderBy(x => x.Data)
                .ToListAsync();
        }

        public async Task<IEnumerable<dynamic>> ObterRankingColaboradoresAsync(DateOnly dataInicio, DateOnly dataFim, int top = 10)
        {
            return await _context.Presencas
                .Include(p => p.Colaborador)
                .ThenInclude(c => c.Area)
                .Where(p => p.DataPresenca >= dataInicio && p.DataPresenca <= dataFim)
                .GroupBy(p => new { p.ColaboradorId, NomeColaborador = p.Colaborador.Nome, NomeArea = p.Colaborador.Area!.Nome })
                .Select(g => new
                {
                    ColaboradorId = g.Key.ColaboradorId,
                    NomeColaborador = g.Key.NomeColaborador,
                    Area = g.Key.NomeArea,
                    TotalPresencas = g.Count(),
                    DiasPresentes = g.Select(p => p.DataPresenca).Distinct().Count()
                })
                .OrderByDescending(x => x.TotalPresencas)
                .Take(top)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Presenca> Presencas, int Total)> ObterComPaginacaoAsync(
            int pagina, int tamanhoPagina, DateOnly? dataInicio = null, DateOnly? dataFim = null,
            int? colaboradorId = null, int? areaId = null, bool? validado = null)
        {
            var query = _context.Presencas
                .Include(p => p.Colaborador)
                .ThenInclude(c => c.Area)
                .AsQueryable();

            if (dataInicio.HasValue)
                query = query.Where(p => p.DataPresenca >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(p => p.DataPresenca <= dataFim.Value);

            if (colaboradorId.HasValue)
                query = query.Where(p => p.ColaboradorId == colaboradorId.Value);

            if (areaId.HasValue)
                query = query.Where(p => p.Colaborador.AreaId == areaId.Value);

            if (validado.HasValue)
                query = query.Where(p => p.Validado == validado.Value);

            var total = await query.CountAsync();
            var presencas = await query
                .OrderByDescending(p => p.DataHoraRegistro)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return (presencas, total);
        }

        public async Task<Dictionary<string, int>> ObterTotalPresencasPorAreaAsync(DateOnly dataInicio, DateOnly dataFim)
        {
            var resultado = await _context.Presencas
                .Include(p => p.Colaborador)
                .ThenInclude(c => c.Area)
                .Where(p => p.DataPresenca >= dataInicio && p.DataPresenca <= dataFim)
                .GroupBy(p => p.Colaborador.Area!.Nome)
                .Select(g => new
                {
                    Area = g.Key,
                    Total = g.Count()
                })
                .ToListAsync();

            return resultado.ToDictionary(x => x.Area, x => x.Total);
        }

        public async Task<Dictionary<string, double>> ObterMediaPresencasDiariasPorAreaAsync(DateOnly dataInicio, DateOnly dataFim)
        {
            var resultado = await _context.Presencas
                .Include(p => p.Colaborador)
                .ThenInclude(c => c.Area)
                .Where(p => p.DataPresenca >= dataInicio && p.DataPresenca <= dataFim)
                .GroupBy(p => p.Colaborador.Area!.Nome)
                .Select(g => new
                {
                    Area = g.Key,
                    Media = g.GroupBy(p => p.DataPresenca).Average(d => (double)d.Count())
                })
                .ToListAsync();

            return resultado.ToDictionary(x => x.Area, x => x.Media);
        }

        public async Task<IEnumerable<dynamic>> ObterDadosGraficoPresencaTemporalAsync(DateOnly dataInicio, DateOnly dataFim, string agrupamento = "dia")
        {
            var query = _context.Presencas
                .Include(p => p.Colaborador)
                .ThenInclude(c => c.Area)
                .Where(p => p.DataPresenca >= dataInicio && p.DataPresenca <= dataFim);

            return agrupamento.ToLower() switch
            {
                "hora" => await query
                    .GroupBy(p => new { p.DataPresenca, Hora = p.HorarioPresenca.Hour })
                    .Select(g => new
                    {
                        Periodo = g.Key.DataPresenca.ToString("dd/MM") + " " + g.Key.Hora.ToString("00") + "h",
                        TotalPresencas = g.Count(),
                        ColaboradoresUnicos = g.Select(p => p.ColaboradorId).Distinct().Count()
                    })
                    .OrderBy(x => x.Periodo)
                    .ToListAsync(),

                "semana" => await query
                    .GroupBy(p => new { Ano = p.DataPresenca.Year, Semana = GetWeekOfYear(p.DataPresenca) })
                    .Select(g => new
                    {
                        Periodo = $"Semana {g.Key.Semana}/{g.Key.Ano}",
                        TotalPresencas = g.Count(),
                        ColaboradoresUnicos = g.Select(p => p.ColaboradorId).Distinct().Count()
                    })
                    .OrderBy(x => x.Periodo)
                    .ToListAsync(),

                "mes" => await query
                    .GroupBy(p => new { p.DataPresenca.Year, p.DataPresenca.Month })
                    .Select(g => new
                    {
                        Periodo = $"{g.Key.Month:00}/{g.Key.Year}",
                        TotalPresencas = g.Count(),
                        ColaboradoresUnicos = g.Select(p => p.ColaboradorId).Distinct().Count()
                    })
                    .OrderBy(x => x.Periodo)
                    .ToListAsync(),

                _ => await query
                    .GroupBy(p => p.DataPresenca)
                    .Select(g => new
                    {
                        Periodo = g.Key.ToString("dd/MM/yyyy"),
                        TotalPresencas = g.Count(),
                        ColaboradoresUnicos = g.Select(p => p.ColaboradorId).Distinct().Count()
                    })
                    .OrderBy(x => x.Periodo)
                    .ToListAsync()
            };
        }

        private static int GetWeekOfYear(DateOnly date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            var calendar = culture.Calendar;
            return calendar.GetWeekOfYear(date.ToDateTime(TimeOnly.MinValue), 
                culture.DateTimeFormat.CalendarWeekRule, 
                culture.DateTimeFormat.FirstDayOfWeek);
        }
    }
}
