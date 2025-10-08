# 🚀 Como Rodar e Testar o DDO - Controle de Presença

## 📋 Pré-requisitos

### 1. **Software Necessário:**
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) ou [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-editions-express)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/)

### 2. **Verificar Instalação:**
```bash
dotnet --version
# Deve retornar 8.0.x ou superior
```

## 🔧 Configuração Inicial

### 1. **Clone o Repositório:**
```bash
git clone https://github.com/Kraunser/ddo-controle-presenca.git
cd ddo-controle-presenca
```

### 2. **Configure o Banco de Dados:**

#### **Opção A: SQL Server Local**
Edite o arquivo `DDO.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DDO_ControlePonto;Integrated Security=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

#### **Opção B: SQL Server com Usuário/Senha**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DDO_ControlePonto;User Id=sa;Password=SuaSenha123;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

#### **Opção C: SQLite (Para Testes Rápidos)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=ddo_database.db"
  }
}
```

### 3. **Instalar Dependências:**
```bash
dotnet restore
```

### 4. **Compilar o Projeto:**
```bash
dotnet build
```

## 🗄️ Configurar Banco de Dados

### 1. **Instalar EF Core Tools:**
```bash
dotnet tool install --global dotnet-ef
```

### 2. **Criar Migrations (se necessário):**
```bash
cd DDO.Web
dotnet ef migrations add InitialCreate
```

### 3. **Aplicar Migrations:**
```bash
dotnet ef database update
```

## ▶️ Executar a Aplicação

### **Método 1: Via Linha de Comando**
```bash
cd DDO.Web
dotnet run
```

### **Método 2: Com URL Específica**
```bash
cd DDO.Web
dotnet run --urls="https://localhost:7252;http://localhost:5000"
```

### **Método 3: Via Visual Studio**
1. Abra o arquivo `DDO.ControlePonto.sln`
2. Defina `DDO.Web` como projeto de inicialização
3. Pressione `F5` ou clique em "Executar"

## 🌐 Acessar a Aplicação

### **URLs Padrão:**
- **HTTPS:** https://localhost:7252
- **HTTP:** http://localhost:5000

### **Credenciais Padrão:**
- **Email:** `admin@randoncorp.com`
- **Senha:** `Admin@123456`

## 🧪 Como Testar as Funcionalidades

### **1. Dashboard Principal**
- Acesse a página inicial
- Verifique se os gráficos carregam
- Teste os filtros por data e área

### **2. Upload de PDFs**
- Vá para "Arquivos" → "Upload"
- Teste upload de um arquivo PDF
- Verifique se aparece na listagem

### **3. Importação de Colaboradores**
- Vá para "Colaboradores" → "Importar"
- Use o arquivo de exemplo: `Scripts/colaboradores-exemplo.csv`
- Verifique se os dados foram importados

### **4. Monitor de Presença**
- Acesse "Presença" → "Monitor"
- Teste o registro manual de presença
- Verifique atualizações em tempo real

### **5. Simulação de RFID**
- Abra o console do navegador (F12)
- Execute o comando:
```javascript
// Simular leitura de RFID
connection.invoke("RegistrarPresencaRFID", "12345678", "Leitor-01");
```

## 🔧 Solução de Problemas

### **Erro de Conexão com Banco:**
```bash
# Verificar se o SQL Server está rodando
services.msc
# Procurar por "SQL Server"
```

### **Erro de Compilação:**
```bash
# Limpar e recompilar
dotnet clean
dotnet restore
dotnet build
```

### **Erro de Migrations:**
```bash
# Remover migrations existentes
rm -rf Migrations/
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### **Porta em Uso:**
```bash
# Verificar processos na porta
netstat -ano | findstr :5000
# Matar processo se necessário
taskkill /PID [PID_NUMBER] /F
```

## 📊 Dados de Teste

### **Colaboradores de Exemplo:**
O sistema inclui dados de teste que podem ser importados via:
- Arquivo: `Scripts/colaboradores-exemplo.csv`
- Script SQL: `Scripts/seed-data.sql`

### **Áreas Pré-configuradas:**
- TI
- RH  
- Engenharia
- Vendas
- Marketing
- Financeiro

## 🔌 Integração com Leitor RFID

### **Exemplo de Cliente RFID (C#):**
```csharp
using Microsoft.AspNetCore.SignalR.Client;
using System.IO.Ports;

class Program
{
    static async Task Main(string[] args)
    {
        // Conectar ao SignalR Hub
        var connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/presencahub")
            .Build();

        await connection.StartAsync();

        // Configurar porta serial
        var serialPort = new SerialPort("COM3", 9600);
        serialPort.DataReceived += async (sender, e) =>
        {
            var rfidCode = serialPort.ReadLine().Trim();
            await connection.InvokeAsync("RegistrarPresencaRFID", rfidCode, "Leitor-Principal");
        };

        serialPort.Open();
        Console.WriteLine("Leitor RFID ativo. Pressione Enter para sair.");
        Console.ReadLine();
    }
}
```

## 📱 Teste em Dispositivos Móveis

### **Acessar via Rede Local:**
1. Descubra seu IP local:
```bash
ipconfig
# Windows

ifconfig
# Linux/Mac
```

2. Execute com IP específico:
```bash
dotnet run --urls="http://0.0.0.0:5000"
```

3. Acesse do celular:
```
http://[SEU_IP]:5000
```

## 🚀 Deploy em Produção

### **Publicar Aplicação:**
```bash
dotnet publish -c Release -o ./publish
```

### **Configurar IIS (Windows):**
1. Instalar [ASP.NET Core Hosting Bundle](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Criar site no IIS apontando para pasta `publish`
3. Configurar connection string de produção

### **Docker (Opcional):**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY publish/ .
EXPOSE 80
ENTRYPOINT ["dotnet", "DDO.Web.dll"]
```

## 📞 Suporte

### **Logs da Aplicação:**
- Localização: `DDO.Web/Logs/`
- Configuração: `appsettings.json` → `Logging`

### **Problemas Comuns:**
1. **Banco não conecta:** Verificar connection string
2. **Porta ocupada:** Alterar porta no `launchSettings.json`
3. **Erro 500:** Verificar logs da aplicação
4. **RFID não funciona:** Verificar configuração SignalR

---

**🎉 Pronto! Sua aplicação DDO está rodando e pronta para uso!**

Para mais informações, consulte a documentação no arquivo `README.md` ou abra uma issue no GitHub.
