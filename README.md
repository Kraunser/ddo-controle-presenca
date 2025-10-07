<<<<<<< HEAD
## 1. Visão Geral do Projeto

O **DDO - Controle de Presença** é um sistema web interno desenvolvido para a Randoncorp com o objetivo de modernizar e automatizar o registro de presença dos colaboradores no Dia de Campo (DDO). A aplicação permite o upload de arquivos PDF relacionados ao evento, o registro de presença através de crachás com tecnologia RFID e a geração de indicadores e dashboards para análise gerencial.

O sistema foi construído utilizando as tecnologias mais recentes do ecossistema .NET, com uma arquitetura robusta e escalável, garantindo segurança, performance e uma experiência de usuário moderna e intuitiva, alinhada à identidade visual da Randoncorp.

## 2. Funcionalidades Principais

- **Autenticação e Autorização:** Sistema de login seguro com perfis de usuário (Administrador, Gerente, Usuário).
- **Upload de Arquivos PDF:** Funcionalidade para upload e armazenamento de arquivos PDF do DDO, com listagem e filtros.
- **Importação de Colaboradores:** Cadastro em lote de colaboradores através de um arquivo CSV contendo matrícula, nome, RFID e área.
- **Registro de Presença via RFID:** Captura em tempo real do código RFID via leitor conectado (USB/Serial) e registro automático da presença.
- **Monitoramento em Tempo Real:** Tela para acompanhamento ao vivo dos registros de presença.
- **Dashboard de Indicadores:** Painel visual com gráficos e estatísticas sobre a presença por área, ranking de participação e comparativos.
- **Consulta e Filtros:** Ferramentas para pesquisar e filtrar os registros de presença por colaborador, área, período, etc.

## 3. Arquitetura e Tecnologias

O projeto foi desenvolvido seguindo os princípios da **Clean Architecture**, promovendo a separação de responsabilidades, baixo acoplamento e alta coesão.

### 3.1. Estrutura dos Projetos

- **`DDO.Core`**: Camada de Domínio. Contém as entidades, enums e regras de negócio mais puras da aplicação. Não depende de nenhuma outra camada.
- **`DDO.Application`**: Camada de Aplicação. Orquestra os casos de uso do sistema, contendo os serviços, interfaces de repositório e DTOs. Depende apenas da camada `Core`.
- **`DDO.Infrastructure`**: Camada de Infraestrutura. Implementa as interfaces da camada de Aplicação, lidando com acesso a dados (Entity Framework Core), comunicação com sistemas externos e outros detalhes de infraestrutura. Depende da camada `Application`.
- **`DDO.Web`**: Camada de Apresentação. É a interface do usuário, desenvolvida com Blazor Web App. Responsável por exibir os dados e capturar as interações do usuário. Depende das camadas `Application` e `Infrastructure`.

### 3.2. Tecnologias Utilizadas
=======
<p align="center">
  <img src="https://i.imgur.com/YOUR_LOGO_URL.png" alt="Randoncorp DDO Logo" width="150"/>
</p>

<h1 align="center">DDO - Controle de Presença</h1>

<p align="center">
  <strong>Sistema web para controle de presença no Dia de Campo (DDO) da Randoncorp</strong>
</p>

<p align="center">
  <a href="#-visão-geral">Visão Geral</a> •
  <a href="#-funcionalidades">Funcionalidades</a> •
  <a href="#-arquitetura-e-tecnologias">Arquitetura</a> •
  <a href="#-como-executar">Como Executar</a> •
  <a href="#-integração-com-leitor-rfid">Leitor RFID</a> •
  <a href="#-como-contribuir">Como Contribuir</a>
</p>

---

## 🚀 Visão Geral

O **DDO - Controle de Presença** é um sistema web interno desenvolvido para a Randoncorp com o objetivo de modernizar e automatizar o registro de presença dos colaboradores no evento "Dia de Campo" (DDO). A aplicação permite o upload de arquivos PDF relacionados ao evento, o registro de presença em tempo real através de crachás com tecnologia RFID e a geração de indicadores e dashboards para análise gerencial.

O sistema foi construído utilizando as tecnologias mais recentes do ecossistema .NET, com uma arquitetura robusta e escalável, garantindo segurança, performance e uma experiência de usuário moderna e intuitiva, alinhada à identidade visual da Randoncorp.

## ✨ Funcionalidades

- **Autenticação e Autorização:** Sistema de login seguro com perfis de usuário (Administrador, Gerente, Usuário) utilizando ASP.NET Core Identity.
- **Upload de Arquivos PDF:** Funcionalidade para upload e armazenamento de arquivos PDF do DDO, com listagem e filtros por área e data.
- **Importação de Colaboradores:** Cadastro em lote de colaboradores através de um arquivo CSV, contendo matrícula, nome, RFID e área.
- **Registro de Presença via RFID:** Captura em tempo real do código RFID via leitor conectado (USB/Serial) e registro automático da presença, com bloqueio de duplicidade.
- **Monitoramento em Tempo Real:** Tela para acompanhamento ao vivo dos registros de presença, atualizada via SignalR.
- **Dashboard de Indicadores:** Painel visual com gráficos e estatísticas sobre a presença por área, ranking de participação e comparativos entre períodos.
- **Consulta e Filtros:** Ferramentas avançadas para pesquisar e filtrar os registros de presença por colaborador, área, período, etc.

## 🛠️ Arquitetura e Tecnologias

O projeto foi desenvolvido seguindo os princípios da **Clean Architecture**, promovendo a separação de responsabilidades, baixo acoplamento e alta coesão para facilitar a manutenção e evolução do sistema.

### Estrutura dos Projetos

- **`DDO.Core`**: Camada de Domínio. Contém as entidades, enums e regras de negócio mais puras da aplicação.
- **`DDO.Application`**: Camada de Aplicação. Orquestra os casos de uso, contendo os serviços, interfaces de repositório e DTOs.
- **`DDO.Infrastructure`**: Camada de Infraestrutura. Implementa as interfaces da camada de Aplicação, lidando com acesso a dados (Entity Framework Core) e outros detalhes de infraestrutura.
- **`DDO.Web`**: Camada de Apresentação. Interface do usuário desenvolvida com Blazor Web App, responsável por exibir os dados e capturar as interações.

### Tecnologias Utilizadas
>>>>>>> b90a182 (Initial commit of DDO project)

| Categoria             | Tecnologia/Framework                               |
| --------------------- | -------------------------------------------------- |
| **Backend**           | ASP.NET Core 8                                     |
| **Frontend**          | Blazor Web App (Server-side)                       |
| **Banco de Dados**    | SQL Server (via Entity Framework Core 8)           |
| **Comunicação Real-Time** | SignalR                                            |
| **Autenticação**      | ASP.NET Core Identity                              |
| **Gráficos**          | Chart.js                                           |
| **Design/UI**         | Bootstrap 5, CSS customizado (Tema Randoncorp)     |

<<<<<<< HEAD
## 4. Configuração do Ambiente de Desenvolvimento

Siga os passos abaixo para configurar e executar o projeto em seu ambiente local.

### 4.1. Pré-requisitos
=======
## 🏁 Como Executar

Siga os passos abaixo para configurar e executar o projeto em seu ambiente local.

### Pré-requisitos
>>>>>>> b90a182 (Initial commit of DDO project)

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (ou SQL Server Express)
- Um editor de código, como [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/).

<<<<<<< HEAD
### 4.2. Passos para Instalação

1.  **Clone o repositório:**
    ```bash
    git clone <url-do-repositorio>
    cd DDO
    ```

2.  **Configure a Connection String do Banco de Dados:**
    Abra o arquivo `DDO.Web/appsettings.Development.json` e altere a `DefaultConnection` para apontar para a sua instância do SQL Server.
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Server=SEU_SERVIDOR;Database=DDO.ControlePonto;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
      }
    }
    ```

3.  **Aplique as Migrations do Entity Framework:**
    As *migrations* são responsáveis por criar a estrutura do banco de dados (tabelas, relacionamentos, etc.). Execute os comandos abaixo no terminal, a partir da pasta raiz do projeto (`/DDO`):
    ```bash
    dotnet tool install --global dotnet-ef
    cd DDO.Web
=======
### Passos para Instalação

1.  **Clone o repositório:**
    ```bash
    git clone https://github.com/Kraunser/DDO-Controle-Presenca.git
    cd DDO-Controle-Presenca
    ```

2.  **Configure a Connection String:**
    Renomeie o arquivo `appsettings.Development.example.json` para `appsettings.Development.json` e altere a `DefaultConnection` para apontar para a sua instância do SQL Server.

3.  **Aplique as Migrations do Entity Framework:**
    As *migrations* são responsáveis por criar a estrutura do banco de dados. Execute os comandos abaixo no terminal, a partir da pasta `DDO.Web`:
    ```bash
    dotnet tool install --global dotnet-ef
>>>>>>> b90a182 (Initial commit of DDO project)
    dotnet ef database update
    ```
    Isso criará o banco de dados `DDO.ControlePonto` e todas as tabelas necessárias.

4.  **Execute a Aplicação:**
    Ainda no terminal, na pasta `DDO.Web`, execute o comando:
    ```bash
    dotnet run
    ```
<<<<<<< HEAD
    A aplicação estará disponível em `https://localhost:7252` (ou outra porta indicada no terminal).

### 4.3. Criando o Usuário Administrador

Ao acessar a aplicação pela primeira vez, será necessário criar um usuário. O primeiro usuário registrado no sistema receberá automaticamente a role de **Administrator**.

1.  Acesse a aplicação no navegador.
2.  Clique em "Registrar" e crie uma nova conta.
3.  Este primeiro usuário terá acesso a todas as funcionalidades administrativas do sistema.

## 5. Integração com Leitor RFID

A comunicação entre o leitor RFID e a aplicação web é feita através de um pequeno **aplicativo cliente** que deve ser executado na máquina onde o leitor está conectado.

### 5.1. Funcionamento

1.  O **aplicativo cliente** (desenvolvido em .NET, Python, etc.) se conecta à porta serial/USB para ouvir os dados do leitor RFID.
2.  Quando um crachá é lido, o cliente captura o código RFID.
3.  O cliente então envia este código para o **Hub SignalR** da aplicação web (`/presencahub`), utilizando um método como `RegistrarPresencaRFID`.
4.  A aplicação web recebe o código, processa o registro de presença e notifica todos os clientes conectados (como a tela de monitoramento) em tempo real.

### 5.2. Exemplo de Cliente (Conceitual)

Um exemplo simples de um cliente em C# para console que realiza essa tarefa:

```csharp
// Este é um exemplo conceitual.
// Requer a biblioteca SignalR Client e SerialPort.

using Microsoft.AspNetCore.SignalR.Client;
using System.IO.Ports;

var hubConnection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7252/presencahub", options =>
    {
        // Adicionar token de autenticação se necessário
        // options.AccessTokenProvider = () => Task.FromResult("seu-token-jwt");
    })
    .Build();

await hubConnection.StartAsync();

var serialPort = new SerialPort("COM3", 9600); // Ajuste a porta e o baud rate
serialPort.DataReceived += async (sender, e) =>
{
    var rfidCode = serialPort.ReadLine().Trim();
    Console.WriteLine($"RFID Lido: {rfidCode}");

    await hubConnection.InvokeAsync("RegistrarPresencaRFID", rfidCode, "Leitor-Mesa-01");
};

serialPort.Open();
Console.WriteLine("Leitor RFID conectado e ouvindo. Pressione Enter para sair.");
Console.ReadLine();
```

## 6. Scripts SQL

Os scripts para criação do banco de dados são gerenciados pelo **Entity Framework Core Migrations**. Não é necessário executar scripts SQL manualmente, a menos que seja para popular dados iniciais.

### 6.1. Script para Criação das Tabelas (Gerado pelo EF)

O SQL correspondente à criação das tabelas pode ser gerado com o seguinte comando (útil para revisão ou para ambientes sem acesso ao `dotnet ef`):

```bash
cd DDO.Web
dotnet ef migrations script > ../database_script.sql
```

## 7. Melhorias Futuras Sugeridas

- **Exportação para Excel/PDF:** Adicionar funcionalidade para exportar os dados das tabelas de presença e relatórios.
- **Notificações por E-mail:** Enviar resumos diários ou semanais de presença para os gerentes de área.
- **Integração com Active Directory:** Utilizar o AD para autenticação dos usuários internos.
- **Logs Avançados e Auditoria:** Expandir a tabela de logs para registrar mais eventos críticos do sistema.
- **Página de Perfil do Colaborador:** Uma página detalhada para cada colaborador, mostrando seu histórico completo de presenças.
- **Gamificação:** Adicionar elementos de gamificação, como medalhas ou rankings mais visuais, para incentivar a participação.
=======
    A aplicação estará disponível em `https://localhost:7252`.

### Criando o Usuário Administrador

O sistema está configurado para criar um usuário administrador padrão na primeira execução. Utilize as seguintes credenciais para o primeiro login:
- **Email:** `admin@randoncorp.com`
- **Senha:** `Admin@123456`

**É altamente recomendável alterar esta senha após o primeiro login.**

## 📡 Integração com Leitor RFID

A comunicação entre o leitor RFID e a aplicação web é feita através de um pequeno **aplicativo cliente** que deve ser executado na máquina onde o leitor está conectado.

1.  O **aplicativo cliente** (desenvolvido em .NET, Python, etc.) se conecta à porta serial/USB para ouvir os dados do leitor RFID.
2.  Quando um crachá é lido, o cliente captura o código RFID.
3.  O cliente então envia este código para o **Hub SignalR** da aplicação web (`/presencahub`) através do método `RegistrarPresencaRFID`.
4.  A aplicação web recebe o código, processa o registro e notifica todos os clientes conectados em tempo real.

## 🤝 Como Contribuir

Contribuições são o que tornam a comunidade de código aberto um lugar incrível para aprender, inspirar e criar. Qualquer contribuição que você fizer será **muito apreciada**.

1.  Faça um *Fork* do projeto
2.  Crie uma *Branch* para sua Feature (`git checkout -b feature/AmazingFeature`)
3.  Faça o *Commit* de suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4.  Faça o *Push* para a Branch (`git push origin feature/AmazingFeature`)
5.  Abra um *Pull Request*

Consulte os templates de `ISSUE_TEMPLATE` e `pull_request_template.md` na pasta `.github` para mais detalhes.
>>>>>>> b90a182 (Initial commit of DDO project)

---

*Este projeto foi desenvolvido pela Manus AI para a Randoncorp.*
*Data: 06 de outubro de 2025*
<<<<<<< HEAD

  
 
  
 
=======
>>>>>>> b90a182 (Initial commit of DDO project)
