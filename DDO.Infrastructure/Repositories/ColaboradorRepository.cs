<<<<<<< HEAD
using DDO.Application.Interfaces;
using DDO.Core.Entities;
using DDO.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DDO.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação do repositório de colaboradores
    /// </summary>
    public class ColaboradorRepository : IColaboradorRepository
    {
        private readonly ApplicationDbContext _context;

        public ColaboradorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Colaborador>> ObterTodosAtivosAsync()
        {
            return await _context.Colaboradores
                .Include(c => c.Area)
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<Colaborador?> ObterPorIdAsync(int id)
        {
            return await _context.Colaboradores
                .Include(c => c.Area)
                .Include(c => c.Presencas)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Colaborador?> ObterPorMatriculaAsync(string matricula)
        {
            return await _context.Colaboradores
                .Include(c => c.Area)
                .FirstOrDefaultAsync(c => c.Matricula == matricula && c.Ativo);
        }

        public async Task<Colaborador?> ObterPorRFIDAsync(string codigoRFID)
        {
            return await _context.Colaboradores
                .Include(c => c.Area)
                .FirstOrDefaultAsync(c => c.CodigoRFID == codigoRFID && c.Ativo);
        }

        public async Task<IEnumerable<Colaborador>> ObterPorAreaAsync(int areaId)
        {
            return await _context.Colaboradores
                .Include(c => c.Area)
                .Where(c => c.AreaId == areaId && c.Ativo)
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<Colaborador> AdicionarAsync(Colaborador colaborador)
        {
            colaborador.DataCadastro = DateTime.UtcNow;
            _context.Colaboradores.Add(colaborador);
            await _context.SaveChangesAsync();
            return colaborador;
        }

        public async Task<Colaborador> AtualizarAsync(Colaborador colaborador)
        {
            colaborador.DataUltimaAtualizacao = DateTime.UtcNow;
            _context.Entry(colaborador).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return colaborador;
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var colaborador = await _context.Colaboradores.FindAsync(id);
            if (colaborador == null) return false;

            colaborador.Ativo = false;
            colaborador.DataUltimaAtualizacao = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteComMatriculaAsync(string matricula, int? idExcluir = null)
        {
            var query = _context.Colaboradores.Where(c => c.Matricula == matricula && c.Ativo);
            
            if (idExcluir.HasValue)
            {
                query = query.Where(c => c.Id != idExcluir.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ExisteComRFIDAsync(string codigoRFID, int? idExcluir = null)
        {
            var query = _context.Colaboradores.Where(c => c.CodigoRFID == codigoRFID && c.Ativo);
            
            if (idExcluir.HasValue)
            {
                query = query.Where(c => c.Id != idExcluir.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<(int Sucesso, int Erro, List<string> Erros)> ImportarLoteAsync(IEnumerable<Colaborador> colaboradores)
        {
            var sucesso = 0;
            var erro = 0;
            var erros = new List<string>();

            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                foreach (var colaborador in colaboradores)
                {
                    try
                    {
                        // Verificar se já existe colaborador com a mesma matrícula
                        if (await ExisteComMatriculaAsync(colaborador.Matricula))
                        {
                            erros.Add($"Linha {erro + sucesso + 1}: Matrícula '{colaborador.Matricula}' já existe.");
                            erro++;
                            continue;
                        }

                        // Verificar se já existe colaborador com o mesmo RFID
                        if (await ExisteComRFIDAsync(colaborador.CodigoRFID))
                        {
                            erros.Add($"Linha {erro + sucesso + 1}: RFID '{colaborador.CodigoRFID}' já existe.");
                            erro++;
                            continue;
                        }

                        colaborador.DataCadastro = DateTime.UtcNow;
                        colaborador.Ativo = true;
                        
                        _context.Colaboradores.Add(colaborador);
                        sucesso++;
                    }
                    catch (Exception ex)
                    {
                        erros.Add($"Linha {erro + sucesso + 1}: {ex.Message}");
                        erro++;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                erros.Add($"Erro geral na importação: {ex.Message}");
                throw;
            }

            return (sucesso, erro, erros);
        }

        public async Task<IEnumerable<Colaborador>> BuscarAsync(string termo)
        {
            return await _context.Colaboradores
                .Include(c => c.Area)
                .Where(c => c.Ativo && 
                           (c.Nome.Contains(termo) || 
                            c.Matricula.Contains(termo) || 
                            (c.Email != null && c.Email.Contains(termo))))
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Colaborador> Colaboradores, int Total)> ObterComPaginacaoAsync(
            int pagina, int tamanhoPagina, string? filtroNome = null, int? areaId = null)
        {
            var query = _context.Colaboradores
                .Include(c => c.Area)
                .Where(c => c.Ativo);

            if (!string.IsNullOrEmpty(filtroNome))
            {
                query = query.Where(c => c.Nome.Contains(filtroNome) || 
                                        c.Matricula.Contains(filtroNome) ||
                                        (c.Email != null && c.Email.Contains(filtroNome)));
            }

            if (areaId.HasValue)
            {
                query = query.Where(c => c.AreaId == areaId.Value);
            }

            var total = await query.CountAsync();
            var colaboradores = await query
                .OrderBy(c => c.Nome)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return (colaboradores, total);
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
    /// Implementação do repositório de colaboradores
    /// </summary>
    public class ColaboradorRepository : IColaboradorRepository
    {
        private readonly ApplicationDbContext _context;

        public ColaboradorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Colaborador>> ObterTodosAtivosAsync()
        {
            return await _context.Colaboradores
                .Include(c => c.Area)
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<Colaborador?> ObterPorIdAsync(int id)
        {
            return await _context.Colaboradores
                .Include(c => c.Area)
                .Include(c => c.Presencas)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Colaborador?> ObterPorMatriculaAsync(string matricula)
        {
            return await _context.Colaboradores
                .Include(c => c.Area)
                .FirstOrDefaultAsync(c => c.Matricula == matricula && c.Ativo);
        }

        public async Task<Colaborador?> ObterPorRFIDAsync(string codigoRFID)
        {
            return await _context.Colaboradores
                .Include(c => c.Area)
                .FirstOrDefaultAsync(c => c.CodigoRFID == codigoRFID && c.Ativo);
        }

        public async Task<IEnumerable<Colaborador>> ObterPorAreaAsync(int areaId)
        {
            return await _context.Colaboradores
                .Include(c => c.Area)
                .Where(c => c.AreaId == areaId && c.Ativo)
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<Colaborador> AdicionarAsync(Colaborador colaborador)
        {
            colaborador.DataCadastro = DateTime.UtcNow;
            _context.Colaboradores.Add(colaborador);
            await _context.SaveChangesAsync();
            return colaborador;
        }

        public async Task<Colaborador> AtualizarAsync(Colaborador colaborador)
        {
            colaborador.DataUltimaAtualizacao = DateTime.UtcNow;
            _context.Entry(colaborador).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return colaborador;
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var colaborador = await _context.Colaboradores.FindAsync(id);
            if (colaborador == null) return false;

            colaborador.Ativo = false;
            colaborador.DataUltimaAtualizacao = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteComMatriculaAsync(string matricula, int? idExcluir = null)
        {
            var query = _context.Colaboradores.Where(c => c.Matricula == matricula && c.Ativo);
            
            if (idExcluir.HasValue)
            {
                query = query.Where(c => c.Id != idExcluir.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ExisteComRFIDAsync(string codigoRFID, int? idExcluir = null)
        {
            var query = _context.Colaboradores.Where(c => c.CodigoRFID == codigoRFID && c.Ativo);
            
            if (idExcluir.HasValue)
            {
                query = query.Where(c => c.Id != idExcluir.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<(int Sucesso, int Erro, List<string> Erros)> ImportarLoteAsync(IEnumerable<Colaborador> colaboradores)
        {
            var sucesso = 0;
            var erro = 0;
            var erros = new List<string>();

            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                foreach (var colaborador in colaboradores)
                {
                    try
                    {
                        // Verificar se já existe colaborador com a mesma matrícula
                        if (await ExisteComMatriculaAsync(colaborador.Matricula))
                        {
                            erros.Add($"Linha {erro + sucesso + 1}: Matrícula '{colaborador.Matricula}' já existe.");
                            erro++;
                            continue;
                        }

                        // Verificar se já existe colaborador com o mesmo RFID
                        if (await ExisteComRFIDAsync(colaborador.CodigoRFID))
                        {
                            erros.Add($"Linha {erro + sucesso + 1}: RFID '{colaborador.CodigoRFID}' já existe.");
                            erro++;
                            continue;
                        }

                        colaborador.DataCadastro = DateTime.UtcNow;
                        colaborador.Ativo = true;
                        
                        _context.Colaboradores.Add(colaborador);
                        sucesso++;
                    }
                    catch (Exception ex)
                    {
                        erros.Add($"Linha {erro + sucesso + 1}: {ex.Message}");
                        erro++;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                erros.Add($"Erro geral na importação: {ex.Message}");
                throw;
            }

            return (sucesso, erro, erros);
        }

        public async Task<IEnumerable<Colaborador>> BuscarAsync(string termo)
        {
            return await _context.Colaboradores
                .Include(c => c.Area)
                .Where(c => c.Ativo && 
                           (c.Nome.Contains(termo) || 
                            c.Matricula.Contains(termo) || 
                            (c.Email != null && c.Email.Contains(termo))))
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Colaborador> Colaboradores, int Total)> ObterComPaginacaoAsync(
            int pagina, int tamanhoPagina, string? filtroNome = null, int? areaId = null)
        {
            var query = _context.Colaboradores
                .Include(c => c.Area)
                .Where(c => c.Ativo);

            if (!string.IsNullOrEmpty(filtroNome))
            {
                query = query.Where(c => c.Nome.Contains(filtroNome) || 
                                        c.Matricula.Contains(filtroNome) ||
                                        (c.Email != null && c.Email.Contains(filtroNome)));
            }

            if (areaId.HasValue)
            {
                query = query.Where(c => c.AreaId == areaId.Value);
            }

            var total = await query.CountAsync();
            var colaboradores = await query
                .OrderBy(c => c.Nome)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync();

            return (colaboradores, total);
        }
    }
}
>>>>>>> b90a182 (Initial commit of DDO project)
