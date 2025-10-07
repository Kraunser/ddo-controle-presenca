<<<<<<< HEAD
using DDO.Application.Interfaces;
using DDO.Core.Entities;
using DDO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DDO.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação do repositório de áreas
    /// </summary>
    public class AreaRepository : IAreaRepository
    {
        private readonly ApplicationDbContext _context;

        public AreaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Area>> ObterTodasAtivasAsync()
        {
            return await _context.Areas
                .Where(a => a.Ativa)
                .OrderBy(a => a.Nome)
                .ToListAsync();
        }

        public async Task<Area?> ObterPorIdAsync(int id)
        {
            return await _context.Areas
                .Include(a => a.Colaboradores)
                .Include(a => a.ArquivosPDF)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Area?> ObterPorNomeAsync(string nome)
        {
            return await _context.Areas
                .FirstOrDefaultAsync(a => a.Nome == nome);
        }

        public async Task<Area> AdicionarAsync(Area area)
        {
            _context.Areas.Add(area);
            await _context.SaveChangesAsync();
            return area;
        }

        public async Task<Area> AtualizarAsync(Area area)
        {
            _context.Entry(area).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return area;
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area == null) return false;

            area.Ativa = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteComNomeAsync(string nome, int? idExcluir = null)
        {
            var query = _context.Areas.Where(a => a.Nome == nome && a.Ativa);
            
            if (idExcluir.HasValue)
            {
                query = query.Where(a => a.Id != idExcluir.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<dynamic>> ObterEstatisticasPresencaAsync(DateOnly dataInicio, DateOnly dataFim)
        {
            return await _context.Areas
                .Where(a => a.Ativa)
                .Select(a => new
                {
                    AreaId = a.Id,
                    AreaNome = a.Nome,
                    TotalColaboradores = a.Colaboradores.Count(c => c.Ativo),
                    TotalPresencas = a.Colaboradores
                        .SelectMany(c => c.Presencas)
                        .Count(p => p.DataPresenca >= dataInicio && p.DataPresenca <= dataFim),
                    MediaPresencasDiarias = a.Colaboradores
                        .SelectMany(c => c.Presencas)
                        .Where(p => p.DataPresenca >= dataInicio && p.DataPresenca <= dataFim)
                        .GroupBy(p => p.DataPresenca)
                        .Average(g => (double?)g.Count()) ?? 0
                })
                .OrderByDescending(x => x.TotalPresencas)
                .ToListAsync();
        }
    }
}
=======
using DDO.Application.Interfaces;
using DDO.Core.Entities;
using DDO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DDO.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação do repositório de áreas
    /// </summary>
    public class AreaRepository : IAreaRepository
    {
        private readonly ApplicationDbContext _context;

        public AreaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Area>> ObterTodasAtivasAsync()
        {
            return await _context.Areas
                .Where(a => a.Ativa)
                .OrderBy(a => a.Nome)
                .ToListAsync();
        }

        public async Task<Area?> ObterPorIdAsync(int id)
        {
            return await _context.Areas
                .Include(a => a.Colaboradores)
                .Include(a => a.ArquivosPDF)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Area?> ObterPorNomeAsync(string nome)
        {
            return await _context.Areas
                .FirstOrDefaultAsync(a => a.Nome == nome);
        }

        public async Task<Area> AdicionarAsync(Area area)
        {
            _context.Areas.Add(area);
            await _context.SaveChangesAsync();
            return area;
        }

        public async Task<Area> AtualizarAsync(Area area)
        {
            _context.Entry(area).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return area;
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var area = await _context.Areas.FindAsync(id);
            if (area == null) return false;

            area.Ativa = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteComNomeAsync(string nome, int? idExcluir = null)
        {
            var query = _context.Areas.Where(a => a.Nome == nome && a.Ativa);
            
            if (idExcluir.HasValue)
            {
                query = query.Where(a => a.Id != idExcluir.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<dynamic>> ObterEstatisticasPresencaAsync(DateOnly dataInicio, DateOnly dataFim)
        {
            return await _context.Areas
                .Where(a => a.Ativa)
                .Select(a => new
                {
                    AreaId = a.Id,
                    AreaNome = a.Nome,
                    TotalColaboradores = a.Colaboradores.Count(c => c.Ativo),
                    TotalPresencas = a.Colaboradores
                        .SelectMany(c => c.Presencas)
                        .Count(p => p.DataPresenca >= dataInicio && p.DataPresenca <= dataFim),
                    MediaPresencasDiarias = a.Colaboradores
                        .SelectMany(c => c.Presencas)
                        .Where(p => p.DataPresenca >= dataInicio && p.DataPresenca <= dataFim)
                        .GroupBy(p => p.DataPresenca)
                        .Average(g => (double?)g.Count()) ?? 0
                })
                .OrderByDescending(x => x.TotalPresencas)
                .ToListAsync();
        }
    }
}
>>>>>>> b90a182 (Initial commit of DDO project)
