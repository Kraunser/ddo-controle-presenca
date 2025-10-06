using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using DDO.Application.Services;
using System.Security.Claims;

namespace DDO.Web.Hubs
{
    /// <summary>
    /// Hub SignalR para comunicação em tempo real de presenças
    /// </summary>
    [Authorize]
    public class PresencaHub : Hub
    {
        private readonly PresencaService _presencaService;
        private readonly ILogger<PresencaHub> _logger;

        public PresencaHub(PresencaService presencaService, ILogger<PresencaHub> logger)
        {
            _presencaService = presencaService;
            _logger = logger;
        }

        /// <summary>
        /// Conecta cliente ao hub
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = Context.User?.Identity?.Name;
            
            _logger.LogInformation("Cliente conectado ao PresencaHub: {ConnectionId}, Usuário: {UserName}", 
                Context.ConnectionId, userName);

            // Adicionar à grupo geral de monitoramento
            await Groups.AddToGroupAsync(Context.ConnectionId, "MonitoramentoPresenca");
            
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Desconecta cliente do hub
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userName = Context.User?.Identity?.Name;
            
            _logger.LogInformation("Cliente desconectado do PresencaHub: {ConnectionId}, Usuário: {UserName}", 
                Context.ConnectionId, userName);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "MonitoramentoPresenca");
            
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Registra presença via RFID (chamado pelo cliente leitor RFID)
        /// </summary>
        public async Task RegistrarPresencaRFID(string codigoRFID, string? dispositivoOrigem = null)
        {
            try
            {
                var resultado = await _presencaService.RegistrarPresencaRFIDAsync(codigoRFID, dispositivoOrigem);
                
                // Enviar resultado para o cliente que fez a solicitação
                await Clients.Caller.SendAsync("ResultadoRegistroPresenca", resultado);
                
                // Se o registro foi bem-sucedido, notificar todos os clientes conectados
                if (resultado.Sucesso)
                {
                    await Clients.Group("MonitoramentoPresenca").SendAsync("NovaPresencaRegistrada", new
                    {
                        resultado.ColaboradorId,
                        resultado.ColaboradorNome,
                        resultado.ColaboradorArea,
                        resultado.TipoRegistro,
                        resultado.DataHoraRegistro,
                        resultado.PresencaId,
                        DispositivoOrigem = dispositivoOrigem
                    });

                    _logger.LogInformation("Presença RFID registrada e notificada: {ColaboradorNome} - {TipoRegistro}", 
                        resultado.ColaboradorNome, resultado.TipoRegistro);
                }
                else
                {
                    _logger.LogWarning("Falha no registro RFID: {Mensagem}, RFID: {CodigoRFID}", 
                        resultado.Mensagem, codigoRFID);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no hub ao registrar presença RFID: {CodigoRFID}", codigoRFID);
                
                await Clients.Caller.SendAsync("ResultadoRegistroPresenca", new ResultadoRegistroPresenca
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do sistema. Tente novamente.",
                    TipoErro = TipoErroRegistro.ErroInterno
                });
            }
        }

        /// <summary>
        /// Registra presença manual (chamado pela interface web)
        /// </summary>
        public async Task RegistrarPresencaManual(int colaboradorId, string tipoRegistro, string? observacoes = null)
        {
            try
            {
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = Context.User?.Identity?.Name;

                if (!Enum.TryParse<DDO.Core.Enums.TipoRegistroPresenca>(tipoRegistro, out var tipo))
                {
                    await Clients.Caller.SendAsync("ResultadoRegistroPresenca", new ResultadoRegistroPresenca
                    {
                        Sucesso = false,
                        Mensagem = "Tipo de registro inválido.",
                        TipoErro = TipoErroRegistro.ErroInterno
                    });
                    return;
                }

                var resultado = await _presencaService.RegistrarPresencaManualAsync(
                    colaboradorId, tipo, observacoes, userName);
                
                // Enviar resultado para o cliente que fez a solicitação
                await Clients.Caller.SendAsync("ResultadoRegistroPresenca", resultado);
                
                // Se o registro foi bem-sucedido, notificar todos os clientes conectados
                if (resultado.Sucesso)
                {
                    await Clients.Group("MonitoramentoPresenca").SendAsync("NovaPresencaRegistrada", new
                    {
                        resultado.ColaboradorId,
                        resultado.ColaboradorNome,
                        resultado.ColaboradorArea,
                        resultado.TipoRegistro,
                        resultado.DataHoraRegistro,
                        resultado.PresencaId,
                        DispositivoOrigem = $"Manual - {userName}",
                        Observacoes = observacoes
                    });

                    _logger.LogInformation("Presença manual registrada: {ColaboradorNome} - {TipoRegistro} por {Usuario}", 
                        resultado.ColaboradorNome, resultado.TipoRegistro, userName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no hub ao registrar presença manual: ColaboradorId {ColaboradorId}", colaboradorId);
                
                await Clients.Caller.SendAsync("ResultadoRegistroPresenca", new ResultadoRegistroPresenca
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do sistema. Tente novamente.",
                    TipoErro = TipoErroRegistro.ErroInterno
                });
            }
        }

        /// <summary>
        /// Entra no grupo de monitoramento de uma área específica
        /// </summary>
        public async Task EntrarGrupoArea(string nomeArea)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Area_{nomeArea}");
            _logger.LogDebug("Cliente {ConnectionId} entrou no grupo da área: {Area}", Context.ConnectionId, nomeArea);
        }

        /// <summary>
        /// Sai do grupo de monitoramento de uma área específica
        /// </summary>
        public async Task SairGrupoArea(string nomeArea)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Area_{nomeArea}");
            _logger.LogDebug("Cliente {ConnectionId} saiu do grupo da área: {Area}", Context.ConnectionId, nomeArea);
        }

        /// <summary>
        /// Solicita estatísticas em tempo real
        /// </summary>
        public async Task SolicitarEstatisticas(string dataInicio, string dataFim)
        {
            try
            {
                if (DateOnly.TryParse(dataInicio, out var inicio) && DateOnly.TryParse(dataFim, out var fim))
                {
                    var estatisticas = await _presencaService.ObterEstatisticasAsync(inicio, fim);
                    await Clients.Caller.SendAsync("EstatisticasAtualizadas", estatisticas);
                }
                else
                {
                    await Clients.Caller.SendAsync("ErroEstatisticas", "Formato de data inválido.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter estatísticas via SignalR");
                await Clients.Caller.SendAsync("ErroEstatisticas", "Erro interno do sistema.");
            }
        }

        /// <summary>
        /// Testa conexão RFID
        /// </summary>
        public async Task TestarConexaoRFID()
        {
            await Clients.Caller.SendAsync("TesteConexaoRFID", new
            {
                Sucesso = true,
                Mensagem = "Conexão com o hub estabelecida com sucesso!",
                DataHora = DateTime.Now,
                ConnectionId = Context.ConnectionId
            });
        }
    }
}
