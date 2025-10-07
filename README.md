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

| Categoria             | Tecnologia/Framework                               |
| --------------------- | -------------------------------------------------- |
| **Backend**           | ASP.NET Core 8                                     |
| **Frontend**          | Blazor Web App (Server-side)                       |
| **Banco de Dados**    | SQL Server (via Entity Framework Core 8)           |
| **Comunicação Real-Time** | SignalR                                            |
| **Autenticação**      | ASP.NET Core Identity                              |
| **Gráficos**          | Chart.js                                           |
| **Design/UI**         | Bootstrap 5, CSS customizado (Tema Randoncorp)     |

## 🏁 Como Executar

Siga os passos abaixo para configurar e executar o projeto em seu ambiente local.

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (ou SQL Server Express)
- Um editor de código, como [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/).

### Passos para Instalação

1.  **Clone o repositório:**
    ```bash
    git clone https://github.com/seu-usuario/DDO-Controle-Presenca.git
    cd DDO-Controle-Presenca
    ```

2.  **Configure a Connection String:**
    Renomeie o arquivo `appsettings.Development.example.json` para `appsettings.Development.json` e altere a `DefaultConnection` para apontar para a sua instância do SQL Server.

3.  **Aplique as Migrations do Entity Framework:**
    As *migrations* são responsáveis por criar a estrutura do banco de dados. Execute os comandos abaixo no terminal, a partir da pasta `DDO.Web`:
    ```bash
    dotnet tool install --global dotnet-ef
    dotnet ef database update
    ```
    Isso criará o banco de dados `DDO.ControlePonto` e todas as tabelas necessárias.

4.  **Execute a Aplicação:**
    Ainda no terminal, na pasta `DDO.Web`, execute o comando:
    ```bash
    dotnet run
    ```
    A aplicação estará disponível em `https://localhost:7252`.

### Criando o Usuário Administrador

O sistema está configurado para criar um usuário administrador padrão na primeira execução. Utilize as seguintes credenciais para o primeiro login:
- **Email:** `admin@randoncorp.com`
- **Senha:** `Admin@123456`

**É altamente recomendável alterar esta senha após o primeiro login.**

### Dados de Teste

O sistema inclui um **seeder automático** que popula o banco de dados com dados de teste quando executado em ambiente de desenvolvimento. Os dados incluem:

- **6 áreas** (TI, RH, Engenharia, Vendas, Marketing, Financeiro)
- **24 colaboradores** distribuídos entre as áreas
- **Registros de presença** dos últimos 30 dias úteis
- **Arquivos PDF** de exemplo para cada área

#### Alternativas para Popular Dados

1. **Automático (Recomendado):** Os dados são inseridos automaticamente na primeira execução em modo Development.

2. **Manual via SQL:** Execute o script `Scripts/seed-data.sql` diretamente no SQL Server Management Studio.

3. **Importação CSV:** Use o arquivo `Scripts/colaboradores-exemplo.csv` para testar a funcionalidade de importação de colaboradores.

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

---

*Este projeto foi desenvolvido pela Manus AI para a Randoncorp.*
*Data: 06 de outubro de 2025*
