<<<<<<< HEAD
using DDO.Application.Interfaces;
using DDO.Core.Entities;
using DDO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DDO.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação do repositório de arquivos PDF
    /// </summary>
    public class ArquivoPDFRepository : IArquivoPDFRepository
    {
        private readonly ApplicationDbContext _context;

        public ArquivoPDFRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ArquivoPDF>> ObterTodosAtivosAsync()
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.Ativo)
                .OrderByDescending(a => a.DataUpload)
                .ToListAsync();
        }

        public async Task<ArquivoPDF?> ObterPorIdAsync(int id)
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<ArquivoPDF>> ObterPorAreaAsync(int areaId)
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.AreaId == areaId && a.Ativo)
                .OrderByDescending(a => a.DataUpload)
                .ToListAsync();
        }

        public async Task<IEnumerable<ArquivoPDF>> ObterPorPeriodoUploadAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.DataUpload >= dataInicio && a.DataUpload <= dataFim && a.Ativo)
                .OrderByDescending(a => a.DataUpload)
                .ToListAsync();
        }

        public async Task<IEnumerable<ArquivoPDF>> ObterPorPeriodoReferenciaAsync(DateOnly dataInicio, DateOnly dataFim)
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.DataReferencia >= dataInicio && a.DataReferencia <= dataFim && a.Ativo)
                .OrderByDescending(a => a.DataReferencia)
                .ToListAsync();
        }

        public async Task<ArquivoPDF> AdicionarAsync(ArquivoPDF arquivo)
        {
            _context.ArquivosPDF.Add(arquivo);
            await _context.SaveChangesAsync();
            return arquivo;
        }

        public async Task<ArquivoPDF> AtualizarAsync(ArquivoPDF arquivo)
        {
            _context.Entry(arquivo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return arquivo;
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var arquivo = await _context.ArquivosPDF.FindAsync(id);
            if (arquivo == null) return false;

            arquivo.Ativo = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ArquivoPDF?> ObterPorHashMD5Async(string hashMD5)
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .FirstOrDefaultAsync(a => a.HashMD5 == hashMD5 && a.Ativo);
        }

        public async Task IncrementarVisualizacoesAsync(int id)
        {
            var arquivo = await _context.ArquivosPDF.FindAsync(id);
            if (arquivo != null)
            {
                arquivo.NumeroVisualizacoes++;
                arquivo.DataUltimaVisualizacao = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(IEnumerable<ArquivoPDF> Arquivos, int Total)> ObterComPaginacaoAsync(
            int pagina, int tamanhoPagina, string? filtroNome = null, int? areaId = null,
            DateOnly? dataReferenciaInicio = null, DateOnly? dataReferenciaFim = null)
        {
            var query = _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.Ativo);

            if (!string.IsNullOrEmpty(filtroNome))
            {
                query = query.Where(a => a.NomeArquivo.Contains(filtroNome) || 
                                        (a.Descricao != null && a.Descricao.Contains(filtroNome)));
            }

            if (areaId.HasValue)
            {
                query = query.Where(a => a.AreaId == areaId.Value);
            }

            if (dataReferenciaInicio.HasValue)
            {
                query = query.Where(a => a.DataReferencia >= dataReferenciaInicio.Value);
            }

            if (dataReferenciaFim.HasValue)
            {
                query = query.Where(a => a.DataReferencia <= dataReferenciaFim.Value);
            }

            var total = await query.CountAsync();
            var arquivos = await query
                .OrderByDescending(a => a.DataUpload)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return (arquivos, total);
        }

        public async Task<IEnumerable<ArquivoPDF>> BuscarAsync(string termo)
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.Ativo && 
                           (a.NomeArquivo.Contains(termo) || 
                            (a.Descricao != null && a.Descricao.Contains(termo)) ||
                            (a.Area != null && a.Area.Nome.Contains(termo))))
                .OrderByDescending(a => a.DataUpload)
                .ToListAsync();
        }

        public async Task<IEnumerable<dynamic>> ObterEstatisticasPorAreaAsync()
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.Ativo)
                .GroupBy(a => a.Area!.Nome)
                .Select(g => new
                {
                    Area = g.Key,
                    TotalArquivos = g.Count(),
                    TamanhoTotal = g.Sum(a => a.TamanhoArquivo),
                    UltimoUpload = g.Max(a => a.DataUpload)
                })
                .OrderByDescending(x => x.TotalArquivos)
                .ToListAsync();
        }

        public async Task<IEnumerable<ArquivoPDF>> ObterMaisVisualizadosAsync(int top = 10)
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.Ativo)
                .OrderByDescending(a => a.NumeroVisualizacoes)
                .Take(top)
                .ToListAsync();
        }

        public async Task<Dictionary<string, long>> ObterTamanhoTotalPorAreaAsync()
        {
            var resultado = await _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.Ativo)
                .GroupBy(a => a.Area!.Nome)
                .Select(g => new
                {
                    Area = g.Key,
                    TamanhoTotal = g.Sum(a => a.TamanhoArquivo)
                })
                .ToListAsync();

            return resultado.ToDictionary(x => x.Area, x => x.TamanhoTotal);
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
    /// Implementação do repositório de arquivos PDF
    /// </summary>
    public class ArquivoPDFRepository : IArquivoPDFRepository
    {
        private readonly ApplicationDbContext _context;

        public ArquivoPDFRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ArquivoPDF>> ObterTodosAtivosAsync()
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.Ativo)
                .OrderByDescending(a => a.DataUpload)
                .ToListAsync();
        }

        public async Task<ArquivoPDF?> ObterPorIdAsync(int id)
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<ArquivoPDF>> ObterPorAreaAsync(int areaId)
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.AreaId == areaId && a.Ativo)
                .OrderByDescending(a => a.DataUpload)
                .ToListAsync();
        }

        public async Task<IEnumerable<ArquivoPDF>> ObterPorPeriodoUploadAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.DataUpload >= dataInicio && a.DataUpload <= dataFim && a.Ativo)
                .OrderByDescending(a => a.DataUpload)
                .ToListAsync();
        }

        public async Task<IEnumerable<ArquivoPDF>> ObterPorPeriodoReferenciaAsync(DateOnly dataInicio, DateOnly dataFim)
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.DataReferencia >= dataInicio && a.DataReferencia <= dataFim && a.Ativo)
                .OrderByDescending(a => a.DataReferencia)
                .ToListAsync();
        }

        public async Task<ArquivoPDF> AdicionarAsync(ArquivoPDF arquivo)
        {
            _context.ArquivosPDF.Add(arquivo);
            await _context.SaveChangesAsync();
            return arquivo;
        }

        public async Task<ArquivoPDF> AtualizarAsync(ArquivoPDF arquivo)
        {
            _context.Entry(arquivo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return arquivo;
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var arquivo = await _context.ArquivosPDF.FindAsync(id);
            if (arquivo == null) return false;

            arquivo.Ativo = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ArquivoPDF?> ObterPorHashMD5Async(string hashMD5)
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .FirstOrDefaultAsync(a => a.HashMD5 == hashMD5 && a.Ativo);
        }

        public async Task IncrementarVisualizacoesAsync(int id)
        {
            var arquivo = await _context.ArquivosPDF.FindAsync(id);
            if (arquivo != null)
            {
                arquivo.NumeroVisualizacoes++;
                arquivo.DataUltimaVisualizacao = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<(IEnumerable<ArquivoPDF> Arquivos, int Total)> ObterComPaginacaoAsync(
            int pagina, int tamanhoPagina, string? filtroNome = null, int? areaId = null,
            DateOnly? dataReferenciaInicio = null, DateOnly? dataReferenciaFim = null)
        {
            var query = _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.Ativo);

            if (!string.IsNullOrEmpty(filtroNome))
            {
                query = query.Where(a => a.NomeArquivo.Contains(filtroNome) || 
                                        (a.Descricao != null && a.Descricao.Contains(filtroNome)));
            }

            if (areaId.HasValue)
            {
                query = query.Where(a => a.AreaId == areaId.Value);
            }

            if (dataReferenciaInicio.HasValue)
            {
                query = query.Where(a => a.DataReferencia >= dataReferenciaInicio.Value);
            }

            if (dataReferenciaFim.HasValue)
            {
                query = query.Where(a => a.DataReferencia <= dataReferenciaFim.Value);
            }

            var total = await query.CountAsync();
            var arquivos = await query
                .OrderByDescending(a => a.DataUpload)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return (arquivos, total);
        }

        public async Task<IEnumerable<ArquivoPDF>> BuscarAsync(string termo)
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.Ativo && 
                           (a.NomeArquivo.Contains(termo) || 
                            (a.Descricao != null && a.Descricao.Contains(termo)) ||
                            (a.Area != null && a.Area.Nome.Contains(termo))))
                .OrderByDescending(a => a.DataUpload)
                .ToListAsync();
        }

        public async Task<IEnumerable<dynamic>> ObterEstatisticasPorAreaAsync()
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.Ativo)
                .GroupBy(a => a.Area!.Nome)
                .Select(g => new
                {
                    Area = g.Key,
                    TotalArquivos = g.Count(),
                    TamanhoTotal = g.Sum(a => a.TamanhoArquivo),
                    UltimoUpload = g.Max(a => a.DataUpload)
                })
                .OrderByDescending(x => x.TotalArquivos)
                .ToListAsync();
        }

        public async Task<IEnumerable<ArquivoPDF>> ObterMaisVisualizadosAsync(int top = 10)
        {
            return await _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.Ativo)
                .OrderByDescending(a => a.NumeroVisualizacoes)
                .Take(top)
                .ToListAsync();
        }

        public async Task<Dictionary<string, long>> ObterTamanhoTotalPorAreaAsync()
        {
            var resultado = await _context.ArquivosPDF
                .Include(a => a.Area)
                .Where(a => a.Ativo)
                .GroupBy(a => a.Area!.Nome)
                .Select(g => new
                {
                    Area = g.Key,
                    TamanhoTotal = g.Sum(a => a.TamanhoArquivo)
                })
                .ToListAsync();

            return resultado.ToDictionary(x => x.Area, x => x.TamanhoTotal);
        }
    }
}
>>>>>>> b90a182 (Initial commit of DDO project)
