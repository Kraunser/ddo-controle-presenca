<<<<<<< HEAD
using DDO.Application.Interfaces;
using DDO.Core.Entities;
using DDO.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DDO.Application.Services
{
    /// <summary>
    /// Serviço para gerenciamento de presenças
    /// </summary>
    public class PresencaService
    {
        private readonly IPresencaRepository _presencaRepository;
        private readonly IColaboradorRepository _colaboradorRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PresencaService> _logger;

        public PresencaService(
            IPresencaRepository presencaRepository,
            IColaboradorRepository colaboradorRepository,
            IConfiguration configuration,
            ILogger<PresencaService> logger)
        {
            _presencaRepository = presencaRepository;
            _colaboradorRepository = colaboradorRepository;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Registra presença via RFID
        /// </summary>
        public async Task<ResultadoRegistroPresenca> RegistrarPresencaRFIDAsync(string codigoRFID, string? dispositivoOrigem = null)
        {
            try
            {
                // Buscar colaborador pelo código RFID
                var colaborador = await _colaboradorRepository.ObterPorRFIDAsync(codigoRFID);
                if (colaborador == null)
                {
                    _logger.LogWarning("Tentativa de registro com RFID não cadastrado: {CodigoRFID}", codigoRFID);
                    return new ResultadoRegistroPresenca
                    {
                        Sucesso = false,
                        Mensagem = "Código RFID não encontrado no sistema.",
                        TipoErro = TipoErroRegistro.ColaboradorNaoEncontrado
                    };
                }

                if (!colaborador.Ativo)
                {
                    _logger.LogWarning("Tentativa de registro com colaborador inativo: {ColaboradorId}", colaborador.Id);
                    return new ResultadoRegistroPresenca
                    {
                        Sucesso = false,
                        Mensagem = "Colaborador inativo no sistema.",
                        TipoErro = TipoErroRegistro.ColaboradorInativo
                    };
                }

                // Verificar se já existe registro na data atual
                var hoje = DateOnly.FromDateTime(DateTime.Now);
                var jaRegistrouHoje = await _presencaRepository.ExistePresencaNaDataAsync(colaborador.Id, hoje);

                // Configurar tempo mínimo entre registros
                var tempoMinimoMinutos = _configuration.GetValue<int>("RFIDSettings:AllowDuplicateRegistrationMinutes", 5);
                var ultimaPresenca = await _presencaRepository.ObterUltimaPresencaColaboradorAsync(colaborador.Id);

                if (ultimaPresenca != null)
                {
                    var tempoDecorrido = DateTime.UtcNow - ultimaPresenca.DataHoraRegistro;
                    if (tempoDecorrido.TotalMinutes < tempoMinimoMinutos)
                    {
                        return new ResultadoRegistroPresenca
                        {
                            Sucesso = false,
                            Mensagem = $"Aguarde {tempoMinimoMinutos} minutos entre registros.",
                            TipoErro = TipoErroRegistro.RegistroMuitoRecente,
                            ColaboradorNome = colaborador.Nome
                        };
                    }
                }

                // Determinar tipo de registro
                var tipoRegistro = DeterminarTipoRegistro(ultimaPresenca, jaRegistrouHoje);

                // Criar registro de presença
                var presenca = new Presenca
                {
                    ColaboradorId = colaborador.Id,
                    TipoRegistro = tipoRegistro.ToString(),
                    MetodoRegistro = MetodoRegistro.RFID.ToString(),
                    DispositivoOrigem = dispositivoOrigem,
                    Validado = true
                };

                var presencaRegistrada = await _presencaRepository.RegistrarPresencaAsync(presenca);

                _logger.LogInformation("Presença registrada com sucesso: Colaborador {ColaboradorId} ({Nome}), Tipo: {TipoRegistro}", 
                    colaborador.Id, colaborador.Nome, tipoRegistro);

                return new ResultadoRegistroPresenca
                {
                    Sucesso = true,
                    Mensagem = $"Presença registrada: {tipoRegistro}",
                    ColaboradorId = colaborador.Id,
                    ColaboradorNome = colaborador.Nome,
                    ColaboradorArea = colaborador.Area?.Nome ?? "",
                    TipoRegistro = tipoRegistro,
                    DataHoraRegistro = presencaRegistrada.DataHoraRegistro,
                    PresencaId = presencaRegistrada.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar presença via RFID: {CodigoRFID}", codigoRFID);
                return new ResultadoRegistroPresenca
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do sistema. Tente novamente.",
                    TipoErro = TipoErroRegistro.ErroInterno
                };
            }
        }

        /// <summary>
        /// Registra presença manual
        /// </summary>
        public async Task<ResultadoRegistroPresenca> RegistrarPresencaManualAsync(
            int colaboradorId, 
            TipoRegistroPresenca tipoRegistro, 
            string? observacoes = null,
            string? usuarioRegistro = null)
        {
            try
            {
                var colaborador = await _colaboradorRepository.ObterPorIdAsync(colaboradorId);
                if (colaborador == null)
                {
                    return new ResultadoRegistroPresenca
                    {
                        Sucesso = false,
                        Mensagem = "Colaborador não encontrado.",
                        TipoErro = TipoErroRegistro.ColaboradorNaoEncontrado
                    };
                }

                if (!colaborador.Ativo)
                {
                    return new ResultadoRegistroPresenca
                    {
                        Sucesso = false,
                        Mensagem = "Colaborador inativo no sistema.",
                        TipoErro = TipoErroRegistro.ColaboradorInativo
                    };
                }

                var presenca = new Presenca
                {
                    ColaboradorId = colaborador.Id,
                    TipoRegistro = tipoRegistro.ToString(),
                    MetodoRegistro = MetodoRegistro.Manual.ToString(),
                    Observacoes = observacoes,
                    DispositivoOrigem = $"Manual - {usuarioRegistro}",
                    Validado = true
                };

                var presencaRegistrada = await _presencaRepository.RegistrarPresencaAsync(presenca);

                _logger.LogInformation("Presença manual registrada: Colaborador {ColaboradorId} ({Nome}), Tipo: {TipoRegistro}, Usuário: {Usuario}", 
                    colaborador.Id, colaborador.Nome, tipoRegistro, usuarioRegistro);

                return new ResultadoRegistroPresenca
                {
                    Sucesso = true,
                    Mensagem = $"Presença registrada manualmente: {tipoRegistro}",
                    ColaboradorId = colaborador.Id,
                    ColaboradorNome = colaborador.Nome,
                    ColaboradorArea = colaborador.Area?.Nome ?? "",
                    TipoRegistro = tipoRegistro,
                    DataHoraRegistro = presencaRegistrada.DataHoraRegistro,
                    PresencaId = presencaRegistrada.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar presença manual: ColaboradorId {ColaboradorId}", colaboradorId);
                return new ResultadoRegistroPresenca
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do sistema. Tente novamente.",
                    TipoErro = TipoErroRegistro.ErroInterno
                };
            }
        }

        /// <summary>
        /// Determina o tipo de registro baseado no histórico
        /// </summary>
        private TipoRegistroPresenca DeterminarTipoRegistro(Presenca? ultimaPresenca, bool jaRegistrouHoje)
        {
            if (ultimaPresenca == null || !jaRegistrouHoje)
            {
                return TipoRegistroPresenca.Entrada;
            }

            // Se o último registro foi hoje, alternar entre entrada e saída
            return ultimaPresenca.TipoRegistro switch
            {
                nameof(TipoRegistroPresenca.Entrada) => TipoRegistroPresenca.Saida,
                nameof(TipoRegistroPresenca.Saida) => TipoRegistroPresenca.RetornoIntervalo,
                nameof(TipoRegistroPresenca.SaidaIntervalo) => TipoRegistroPresenca.RetornoIntervalo,
                nameof(TipoRegistroPresenca.RetornoIntervalo) => TipoRegistroPresenca.Saida,
                _ => TipoRegistroPresenca.Entrada
            };
        }

        /// <summary>
        /// Obtém estatísticas de presença por período
        /// </summary>
        public async Task<EstatisticasPresenca> ObterEstatisticasAsync(DateOnly dataInicio, DateOnly dataFim)
        {
            var totalPresencas = await _presencaRepository.ObterTotalPresencasPorAreaAsync(dataInicio, dataFim);
            var mediaPresencas = await _presencaRepository.ObterMediaPresencasDiariasPorAreaAsync(dataInicio, dataFim);
            var estatisticasPorArea = await _presencaRepository.ObterEstatisticasPorAreaAsync(dataInicio, dataFim);
            var ranking = await _presencaRepository.ObterRankingColaboradoresAsync(dataInicio, dataFim);

            return new EstatisticasPresenca
            {
                DataInicio = dataInicio,
                DataFim = dataFim,
                TotalPresencasPorArea = totalPresencas,
                MediaPresencasDiariasPorArea = mediaPresencas,
                EstatisticasPorArea = estatisticasPorArea.ToList(),
                RankingColaboradores = ranking.ToList()
            };
        }

        /// <summary>
        /// Valida uma presença
        /// </summary>
        public async Task<bool> ValidarPresencaAsync(int presencaId, bool validada, string? observacoes = null)
        {
            try
            {
                var presenca = await _presencaRepository.ObterPorIdAsync(presencaId);
                if (presenca == null) return false;

                presenca.Validado = validada;
                if (!string.IsNullOrEmpty(observacoes))
                {
                    presenca.Observacoes = observacoes;
                }

                await _presencaRepository.AtualizarAsync(presenca);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar presença {PresencaId}", presencaId);
                return false;
            }
        }
    }

    /// <summary>
    /// Resultado do registro de presença
    /// </summary>
    public class ResultadoRegistroPresenca
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public int? ColaboradorId { get; set; }
        public string ColaboradorNome { get; set; } = string.Empty;
        public string ColaboradorArea { get; set; } = string.Empty;
        public TipoRegistroPresenca? TipoRegistro { get; set; }
        public DateTime? DataHoraRegistro { get; set; }
        public int? PresencaId { get; set; }
        public TipoErroRegistro? TipoErro { get; set; }
    }

    /// <summary>
    /// Tipos de erro no registro de presença
    /// </summary>
    public enum TipoErroRegistro
    {
        ColaboradorNaoEncontrado,
        ColaboradorInativo,
        RegistroMuitoRecente,
        ErroInterno
    }

    /// <summary>
    /// Estatísticas de presença
    /// </summary>
    public class EstatisticasPresenca
    {
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }
        public Dictionary<string, int> TotalPresencasPorArea { get; set; } = new();
        public Dictionary<string, double> MediaPresencasDiariasPorArea { get; set; } = new();
        public List<dynamic> EstatisticasPorArea { get; set; } = new();
        public List<dynamic> RankingColaboradores { get; set; } = new();
    }
}
=======
using DDO.Application.Interfaces;
using DDO.Core.Entities;
using DDO.Core.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DDO.Application.Services
{
    /// <summary>
    /// Serviço para gerenciamento de presenças
    /// </summary>
    public class PresencaService
    {
        private readonly IPresencaRepository _presencaRepository;
        private readonly IColaboradorRepository _colaboradorRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PresencaService> _logger;

        public PresencaService(
            IPresencaRepository presencaRepository,
            IColaboradorRepository colaboradorRepository,
            IConfiguration configuration,
            ILogger<PresencaService> logger)
        {
            _presencaRepository = presencaRepository;
            _colaboradorRepository = colaboradorRepository;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Registra presença via RFID
        /// </summary>
        public async Task<ResultadoRegistroPresenca> RegistrarPresencaRFIDAsync(string codigoRFID, string? dispositivoOrigem = null)
        {
            try
            {
                // Buscar colaborador pelo código RFID
                var colaborador = await _colaboradorRepository.ObterPorRFIDAsync(codigoRFID);
                if (colaborador == null)
                {
                    _logger.LogWarning("Tentativa de registro com RFID não cadastrado: {CodigoRFID}", codigoRFID);
                    return new ResultadoRegistroPresenca
                    {
                        Sucesso = false,
                        Mensagem = "Código RFID não encontrado no sistema.",
                        TipoErro = TipoErroRegistro.ColaboradorNaoEncontrado
                    };
                }

                if (!colaborador.Ativo)
                {
                    _logger.LogWarning("Tentativa de registro com colaborador inativo: {ColaboradorId}", colaborador.Id);
                    return new ResultadoRegistroPresenca
                    {
                        Sucesso = false,
                        Mensagem = "Colaborador inativo no sistema.",
                        TipoErro = TipoErroRegistro.ColaboradorInativo
                    };
                }

                // Verificar se já existe registro na data atual
                var hoje = DateOnly.FromDateTime(DateTime.Now);
                var jaRegistrouHoje = await _presencaRepository.ExistePresencaNaDataAsync(colaborador.Id, hoje);

                // Configurar tempo mínimo entre registros
                var tempoMinimoMinutos = _configuration.GetValue<int>("RFIDSettings:AllowDuplicateRegistrationMinutes", 5);
                var ultimaPresenca = await _presencaRepository.ObterUltimaPresencaColaboradorAsync(colaborador.Id);

                if (ultimaPresenca != null)
                {
                    var tempoDecorrido = DateTime.UtcNow - ultimaPresenca.DataHoraRegistro;
                    if (tempoDecorrido.TotalMinutes < tempoMinimoMinutos)
                    {
                        return new ResultadoRegistroPresenca
                        {
                            Sucesso = false,
                            Mensagem = $"Aguarde {tempoMinimoMinutos} minutos entre registros.",
                            TipoErro = TipoErroRegistro.RegistroMuitoRecente,
                            ColaboradorNome = colaborador.Nome
                        };
                    }
                }

                // Determinar tipo de registro
                var tipoRegistro = DeterminarTipoRegistro(ultimaPresenca, jaRegistrouHoje);

                // Criar registro de presença
                var presenca = new Presenca
                {
                    ColaboradorId = colaborador.Id,
                    TipoRegistro = tipoRegistro.ToString(),
                    MetodoRegistro = MetodoRegistro.RFID.ToString(),
                    DispositivoOrigem = dispositivoOrigem,
                    Validado = true
                };

                var presencaRegistrada = await _presencaRepository.RegistrarPresencaAsync(presenca);

                _logger.LogInformation("Presença registrada com sucesso: Colaborador {ColaboradorId} ({Nome}), Tipo: {TipoRegistro}", 
                    colaborador.Id, colaborador.Nome, tipoRegistro);

                return new ResultadoRegistroPresenca
                {
                    Sucesso = true,
                    Mensagem = $"Presença registrada: {tipoRegistro}",
                    ColaboradorId = colaborador.Id,
                    ColaboradorNome = colaborador.Nome,
                    ColaboradorArea = colaborador.Area?.Nome ?? "",
                    TipoRegistro = tipoRegistro,
                    DataHoraRegistro = presencaRegistrada.DataHoraRegistro,
                    PresencaId = presencaRegistrada.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar presença via RFID: {CodigoRFID}", codigoRFID);
                return new ResultadoRegistroPresenca
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do sistema. Tente novamente.",
                    TipoErro = TipoErroRegistro.ErroInterno
                };
            }
        }

        /// <summary>
        /// Registra presença manual
        /// </summary>
        public async Task<ResultadoRegistroPresenca> RegistrarPresencaManualAsync(
            int colaboradorId, 
            TipoRegistroPresenca tipoRegistro, 
            string? observacoes = null,
            string? usuarioRegistro = null)
        {
            try
            {
                var colaborador = await _colaboradorRepository.ObterPorIdAsync(colaboradorId);
                if (colaborador == null)
                {
                    return new ResultadoRegistroPresenca
                    {
                        Sucesso = false,
                        Mensagem = "Colaborador não encontrado.",
                        TipoErro = TipoErroRegistro.ColaboradorNaoEncontrado
                    };
                }

                if (!colaborador.Ativo)
                {
                    return new ResultadoRegistroPresenca
                    {
                        Sucesso = false,
                        Mensagem = "Colaborador inativo no sistema.",
                        TipoErro = TipoErroRegistro.ColaboradorInativo
                    };
                }

                var presenca = new Presenca
                {
                    ColaboradorId = colaborador.Id,
                    TipoRegistro = tipoRegistro.ToString(),
                    MetodoRegistro = MetodoRegistro.Manual.ToString(),
                    Observacoes = observacoes,
                    DispositivoOrigem = $"Manual - {usuarioRegistro}",
                    Validado = true
                };

                var presencaRegistrada = await _presencaRepository.RegistrarPresencaAsync(presenca);

                _logger.LogInformation("Presença manual registrada: Colaborador {ColaboradorId} ({Nome}), Tipo: {TipoRegistro}, Usuário: {Usuario}", 
                    colaborador.Id, colaborador.Nome, tipoRegistro, usuarioRegistro);

                return new ResultadoRegistroPresenca
                {
                    Sucesso = true,
                    Mensagem = $"Presença registrada manualmente: {tipoRegistro}",
                    ColaboradorId = colaborador.Id,
                    ColaboradorNome = colaborador.Nome,
                    ColaboradorArea = colaborador.Area?.Nome ?? "",
                    TipoRegistro = tipoRegistro,
                    DataHoraRegistro = presencaRegistrada.DataHoraRegistro,
                    PresencaId = presencaRegistrada.Id
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar presença manual: ColaboradorId {ColaboradorId}", colaboradorId);
                return new ResultadoRegistroPresenca
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do sistema. Tente novamente.",
                    TipoErro = TipoErroRegistro.ErroInterno
                };
            }
        }

        /// <summary>
        /// Determina o tipo de registro baseado no histórico
        /// </summary>
        private TipoRegistroPresenca DeterminarTipoRegistro(Presenca? ultimaPresenca, bool jaRegistrouHoje)
        {
            if (ultimaPresenca == null || !jaRegistrouHoje)
            {
                return TipoRegistroPresenca.Entrada;
            }

            // Se o último registro foi hoje, alternar entre entrada e saída
            return ultimaPresenca.TipoRegistro switch
            {
                nameof(TipoRegistroPresenca.Entrada) => TipoRegistroPresenca.Saida,
                nameof(TipoRegistroPresenca.Saida) => TipoRegistroPresenca.RetornoIntervalo,
                nameof(TipoRegistroPresenca.SaidaIntervalo) => TipoRegistroPresenca.RetornoIntervalo,
                nameof(TipoRegistroPresenca.RetornoIntervalo) => TipoRegistroPresenca.Saida,
                _ => TipoRegistroPresenca.Entrada
            };
        }

        /// <summary>
        /// Obtém estatísticas de presença por período
        /// </summary>
        public async Task<EstatisticasPresenca> ObterEstatisticasAsync(DateOnly dataInicio, DateOnly dataFim)
        {
            var totalPresencas = await _presencaRepository.ObterTotalPresencasPorAreaAsync(dataInicio, dataFim);
            var mediaPresencas = await _presencaRepository.ObterMediaPresencasDiariasPorAreaAsync(dataInicio, dataFim);
            var estatisticasPorArea = await _presencaRepository.ObterEstatisticasPorAreaAsync(dataInicio, dataFim);
            var ranking = await _presencaRepository.ObterRankingColaboradoresAsync(dataInicio, dataFim);

            return new EstatisticasPresenca
            {
                DataInicio = dataInicio,
                DataFim = dataFim,
                TotalPresencasPorArea = totalPresencas,
                MediaPresencasDiariasPorArea = mediaPresencas,
                EstatisticasPorArea = estatisticasPorArea.ToList(),
                RankingColaboradores = ranking.ToList()
            };
        }

        /// <summary>
        /// Valida uma presença
        /// </summary>
        public async Task<bool> ValidarPresencaAsync(int presencaId, bool validada, string? observacoes = null)
        {
            try
            {
                var presenca = await _presencaRepository.ObterPorIdAsync(presencaId);
                if (presenca == null) return false;

                presenca.Validado = validada;
                if (!string.IsNullOrEmpty(observacoes))
                {
                    presenca.Observacoes = observacoes;
                }

                await _presencaRepository.AtualizarAsync(presenca);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar presença {PresencaId}", presencaId);
                return false;
            }
        }
    }

    /// <summary>
    /// Resultado do registro de presença
    /// </summary>
    public class ResultadoRegistroPresenca
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public int? ColaboradorId { get; set; }
        public string ColaboradorNome { get; set; } = string.Empty;
        public string ColaboradorArea { get; set; } = string.Empty;
        public TipoRegistroPresenca? TipoRegistro { get; set; }
        public DateTime? DataHoraRegistro { get; set; }
        public int? PresencaId { get; set; }
        public TipoErroRegistro? TipoErro { get; set; }
    }

    /// <summary>
    /// Tipos de erro no registro de presença
    /// </summary>
    public enum TipoErroRegistro
    {
        ColaboradorNaoEncontrado,
        ColaboradorInativo,
        RegistroMuitoRecente,
        ErroInterno
    }

    /// <summary>
    /// Estatísticas de presença
    /// </summary>
    public class EstatisticasPresenca
    {
        public DateOnly DataInicio { get; set; }
        public DateOnly DataFim { get; set; }
        public Dictionary<string, int> TotalPresencasPorArea { get; set; } = new();
        public Dictionary<string, double> MediaPresencasDiariasPorArea { get; set; } = new();
        public List<dynamic> EstatisticasPorArea { get; set; } = new();
        public List<dynamic> RankingColaboradores { get; set; } = new();
    }
}
>>>>>>> b90a182 (Initial commit of DDO project)
