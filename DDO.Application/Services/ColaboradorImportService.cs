using DDO.Application.Interfaces;
using DDO.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;

namespace DDO.Application.Services
{
    /// <summary>
    /// Serviço para importação de colaboradores via arquivo CSV
    /// </summary>
    public class ColaboradorImportService
    {
        private readonly IColaboradorRepository _colaboradorRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly ILogger<ColaboradorImportService> _logger;

        public ColaboradorImportService(
            IColaboradorRepository colaboradorRepository,
            IAreaRepository areaRepository,
            ILogger<ColaboradorImportService> logger)
        {
            _colaboradorRepository = colaboradorRepository;
            _areaRepository = areaRepository;
            _logger = logger;
        }

        /// <summary>
        /// Importa colaboradores a partir de um arquivo CSV
        /// </summary>
        public async Task<ResultadoImportacao> ImportarColaboradoresAsync(IFormFile arquivo)
        {
            var resultado = new ResultadoImportacao();

            try
            {
                // Validar arquivo
                var validacao = ValidarArquivo(arquivo);
                if (!validacao.Valido)
                {
                    resultado.Sucesso = false;
                    resultado.Mensagem = validacao.Mensagem;
                    return resultado;
                }

                // Ler e processar CSV
                var colaboradores = await ProcessarCSVAsync(arquivo, resultado);
                
                if (colaboradores.Any())
                {
                    // Importar colaboradores
                    var resultadoImportacao = await _colaboradorRepository.ImportarLoteAsync(colaboradores);
                    
                    resultado.TotalProcessados = colaboradores.Count();
                    resultado.TotalSucesso = resultadoImportacao.Sucesso;
                    resultado.TotalErros = resultadoImportacao.Erro;
                    resultado.Erros.AddRange(resultadoImportacao.Erros);
                    
                    if (resultadoImportacao.Sucesso > 0)
                    {
                        resultado.Sucesso = true;
                        resultado.Mensagem = $"Importação concluída: {resultadoImportacao.Sucesso} colaboradores importados com sucesso.";
                        
                        if (resultadoImportacao.Erro > 0)
                        {
                            resultado.Mensagem += $" {resultadoImportacao.Erro} registros com erro.";
                        }
                    }
                    else
                    {
                        resultado.Sucesso = false;
                        resultado.Mensagem = "Nenhum colaborador foi importado. Verifique os erros abaixo.";
                    }
                }
                else
                {
                    resultado.Sucesso = false;
                    resultado.Mensagem = "Nenhum colaborador válido encontrado no arquivo.";
                }

                _logger.LogInformation("Importação de colaboradores concluída: {Sucesso} sucessos, {Erros} erros", 
                    resultado.TotalSucesso, resultado.TotalErros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante importação de colaboradores");
                resultado.Sucesso = false;
                resultado.Mensagem = "Erro interno durante a importação. Tente novamente.";
                resultado.Erros.Add($"Erro interno: {ex.Message}");
            }

            return resultado;
        }

        /// <summary>
        /// Valida o arquivo CSV enviado
        /// </summary>
        private (bool Valido, string Mensagem) ValidarArquivo(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return (false, "Nenhum arquivo foi selecionado.");
            }

            // Verificar extensão
            var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            if (extensao != ".csv")
            {
                return (false, "Apenas arquivos CSV são aceitos.");
            }

            // Verificar tamanho (máximo 5MB)
            if (arquivo.Length > 5 * 1024 * 1024)
            {
                return (false, "O arquivo é muito grande. Tamanho máximo: 5MB.");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Processa o arquivo CSV e converte em lista de colaboradores
        /// </summary>
        private async Task<List<Colaborador>> ProcessarCSVAsync(IFormFile arquivo, ResultadoImportacao resultado)
        {
            var colaboradores = new List<Colaborador>();
            var areas = (await _areaRepository.ObterTodasAtivasAsync()).ToList();
            var numeroLinha = 0;

            using var reader = new StreamReader(arquivo.OpenReadStream(), Encoding.UTF8);
            
            // Ler cabeçalho
            var cabecalho = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(cabecalho))
            {
                resultado.Erros.Add("Arquivo CSV vazio ou inválido.");
                return colaboradores;
            }

            // Validar cabeçalho esperado
            var colunas = cabecalho.Split(',', ';').Select(c => c.Trim().ToLowerInvariant()).ToArray();
            var colunasEsperadas = new[] { "matricula", "nome", "rfid", "area", "email", "telefone" };
            
            var colunasObrigatorias = new[] { "matricula", "nome", "rfid", "area" };
            var colunasFaltando = colunasObrigatorias.Where(c => !colunas.Contains(c)).ToList();
            
            if (colunasFaltando.Any())
            {
                resultado.Erros.Add($"Colunas obrigatórias faltando no cabeçalho: {string.Join(", ", colunasFaltando)}");
                resultado.Erros.Add("Cabeçalho esperado: Matricula,Nome,RFID,Area,Email,Telefone");
                return colaboradores;
            }

            // Mapear índices das colunas
            var indiceMatricula = Array.IndexOf(colunas, "matricula");
            var indiceNome = Array.IndexOf(colunas, "nome");
            var indiceRFID = Array.IndexOf(colunas, "rfid");
            var indiceArea = Array.IndexOf(colunas, "area");
            var indiceEmail = Array.IndexOf(colunas, "email");
            var indiceTelefone = Array.IndexOf(colunas, "telefone");

            // Processar linhas de dados
            string? linha;
            while ((linha = await reader.ReadLineAsync()) != null)
            {
                numeroLinha++;
                
                if (string.IsNullOrWhiteSpace(linha))
                    continue;

                try
                {
                    var valores = ParseCSVLine(linha);
                    
                    if (valores.Length < 4) // Mínimo: matricula, nome, rfid, area
                    {
                        resultado.Erros.Add($"Linha {numeroLinha}: Número insuficiente de colunas.");
                        continue;
                    }

                    var colaborador = new Colaborador
                    {
                        Matricula = ObterValor(valores, indiceMatricula)?.Trim() ?? "",
                        Nome = ObterValor(valores, indiceNome)?.Trim() ?? "",
                        CodigoRFID = ObterValor(valores, indiceRFID)?.Trim() ?? "",
                        Email = ObterValor(valores, indiceEmail)?.Trim(),
                        Telefone = ObterValor(valores, indiceTelefone)?.Trim()
                    };

                    // Validar campos obrigatórios
                    var errosValidacao = ValidarColaborador(colaborador, numeroLinha);
                    if (errosValidacao.Any())
                    {
                        resultado.Erros.AddRange(errosValidacao);
                        continue;
                    }

                    // Buscar área
                    var nomeArea = ObterValor(valores, indiceArea)?.Trim() ?? "";
                    var area = areas.FirstOrDefault(a => 
                        a.Nome.Equals(nomeArea, StringComparison.OrdinalIgnoreCase));
                    
                    if (area == null)
                    {
                        resultado.Erros.Add($"Linha {numeroLinha}: Área '{nomeArea}' não encontrada.");
                        continue;
                    }

                    colaborador.AreaId = area.Id;
                    colaboradores.Add(colaborador);
                }
                catch (Exception ex)
                {
                    resultado.Erros.Add($"Linha {numeroLinha}: Erro ao processar - {ex.Message}");
                }
            }

            return colaboradores;
        }

        /// <summary>
        /// Faz o parse de uma linha CSV considerando aspas e vírgulas
        /// </summary>
        private string[] ParseCSVLine(string linha)
        {
            var valores = new List<string>();
            var valorAtual = new StringBuilder();
            var dentroAspas = false;
            var separadores = new[] { ',', ';' };

            for (int i = 0; i < linha.Length; i++)
            {
                var caractere = linha[i];

                if (caractere == '"')
                {
                    dentroAspas = !dentroAspas;
                }
                else if (separadores.Contains(caractere) && !dentroAspas)
                {
                    valores.Add(valorAtual.ToString());
                    valorAtual.Clear();
                }
                else
                {
                    valorAtual.Append(caractere);
                }
            }

            valores.Add(valorAtual.ToString());
            return valores.ToArray();
        }

        /// <summary>
        /// Obtém valor de um array de forma segura
        /// </summary>
        private string? ObterValor(string[] valores, int indice)
        {
            return indice >= 0 && indice < valores.Length ? valores[indice] : null;
        }

        /// <summary>
        /// Valida os dados de um colaborador
        /// </summary>
        private List<string> ValidarColaborador(Colaborador colaborador, int numeroLinha)
        {
            var erros = new List<string>();

            if (string.IsNullOrWhiteSpace(colaborador.Matricula))
                erros.Add($"Linha {numeroLinha}: Matrícula é obrigatória.");
            else if (colaborador.Matricula.Length > 50)
                erros.Add($"Linha {numeroLinha}: Matrícula muito longa (máximo 50 caracteres).");

            if (string.IsNullOrWhiteSpace(colaborador.Nome))
                erros.Add($"Linha {numeroLinha}: Nome é obrigatório.");
            else if (colaborador.Nome.Length > 200)
                erros.Add($"Linha {numeroLinha}: Nome muito longo (máximo 200 caracteres).");

            if (string.IsNullOrWhiteSpace(colaborador.CodigoRFID))
                erros.Add($"Linha {numeroLinha}: Código RFID é obrigatório.");
            else if (colaborador.CodigoRFID.Length > 100)
                erros.Add($"Linha {numeroLinha}: Código RFID muito longo (máximo 100 caracteres).");

            if (!string.IsNullOrEmpty(colaborador.Email))
            {
                if (colaborador.Email.Length > 200)
                    erros.Add($"Linha {numeroLinha}: Email muito longo (máximo 200 caracteres).");
                else if (!IsValidEmail(colaborador.Email))
                    erros.Add($"Linha {numeroLinha}: Email inválido.");
            }

            if (!string.IsNullOrEmpty(colaborador.Telefone) && colaborador.Telefone.Length > 20)
                erros.Add($"Linha {numeroLinha}: Telefone muito longo (máximo 20 caracteres).");

            return erros;
        }

        /// <summary>
        /// Valida formato de email
        /// </summary>
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gera um arquivo CSV de exemplo para download
        /// </summary>
        public byte[] GerarArquivoExemplo()
        {
            var csv = new StringBuilder();
            csv.AppendLine("Matricula,Nome,RFID,Area,Email,Telefone");
            csv.AppendLine("001,João Silva,1234567890,Tecnologia da Informação,joao.silva@empresa.com,11999999999");
            csv.AppendLine("002,Maria Santos,0987654321,Recursos Humanos,maria.santos@empresa.com,11888888888");
            csv.AppendLine("003,Pedro Costa,1122334455,Financeiro,pedro.costa@empresa.com,11777777777");

            return Encoding.UTF8.GetBytes(csv.ToString());
        }
    }

    /// <summary>
    /// Resultado da operação de importação
    /// </summary>
    public class ResultadoImportacao
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public int TotalProcessados { get; set; }
        public int TotalSucesso { get; set; }
        public int TotalErros { get; set; }
        public List<string> Erros { get; set; } = new();
    }
}
