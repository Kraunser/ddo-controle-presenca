# 📁 Estrutura do Projeto DDO - Controle de Presença

## 🏗️ Arquitetura Clean Architecture

O projeto segue os princípios da **Clean Architecture**, organizando o código em camadas bem definidas:

```
DDO-Controle-Presenca/
├── 📋 Documentação
│   ├── README.md                      # Documentação principal
│   ├── COMO_RODAR_E_TESTAR.md        # Guia de execução e testes
│   ├── ESTRUTURA_PROJETO.md          # Este arquivo
│   └── randoncorp-brand-analysis.md   # Análise da identidade visual
│
├── 🚀 Scripts de Execução
│   ├── run-app.sh                     # Script para Linux/Mac
│   └── run-app.bat                    # Script para Windows
│
├── ⚙️ Configuração
│   ├── .env.example                   # Exemplo de variáveis de ambiente
│   ├── .gitignore                     # Arquivos ignorados pelo Git
│   ├── DDO.ControlePonto.sln          # Solução do Visual Studio
│   └── .github/                       # Configurações do GitHub
│       └── workflows/
│           └── ci.yml                 # Pipeline CI/CD
│
├── 🎯 DDO.Core (Domínio)
│   ├── Entities/                      # Entidades do domínio
│   │   ├── Area.cs                    # Áreas da empresa
│   │   ├── ArquivoPDF.cs             # Arquivos PDF do DDO
│   │   ├── Colaborador.cs            # Dados dos colaboradores
│   │   ├── LogSistema.cs             # Logs de auditoria
│   │   └── Presenca.cs               # Registros de presença
│   └── Enums/                         # Enumerações
│       ├── MetodoRegistro.cs         # RFID, Manual, etc.
│       ├── NivelLog.cs               # Info, Warning, Error
│       └── TipoRegistroPresenca.cs   # Entrada, Saída, etc.
│
├── 🔧 DDO.Application (Casos de Uso)
│   ├── Interfaces/                    # Contratos dos repositórios
│   │   ├── IAreaRepository.cs
│   │   ├── IArquivoPDFRepository.cs
│   │   ├── IColaboradorRepository.cs
│   │   └── IPresencaRepository.cs
│   └── Services/                      # Serviços de aplicação
│       ├── ColaboradorImportService.cs # Importação via CSV
│       ├── DashboardService.cs        # Estatísticas e gráficos
│       ├── FileUploadService.cs       # Upload de arquivos
│       └── PresencaService.cs         # Lógica de presença
│
├── 🗄️ DDO.Infrastructure (Infraestrutura)
│   ├── Data/                          # Contexto do banco de dados
│   │   └── ApplicationDbContext.cs    # Entity Framework Context
│   └── Repositories/                  # Implementações dos repositórios
│       ├── AreaRepository.cs
│       ├── ArquivoPDFRepository.cs
│       ├── ColaboradorRepository.cs
│       └── PresencaRepository.cs
│
└── 🌐 DDO.Web (Apresentação)
    ├── Components/                    # Componentes Blazor
    │   ├── Layout/                    # Layouts da aplicação
    │   │   ├── MainLayout.razor       # Layout principal
    │   │   └── NavMenu.razor          # Menu de navegação
    │   ├── Pages/                     # Páginas da aplicação
    │   │   ├── Arquivos/              # Gestão de PDFs
    │   │   │   ├── Listar.razor       # Listagem de arquivos
    │   │   │   └── Upload.razor       # Upload de arquivos
    │   │   ├── Colaboradores/         # Gestão de colaboradores
    │   │   │   ├── Index.razor        # Listagem
    │   │   │   └── Importar.razor     # Importação CSV
    │   │   ├── Presenca/              # Controle de presença
    │   │   │   └── Monitor.razor      # Monitor em tempo real
    │   │   ├── Counter.razor          # Página de exemplo
    │   │   ├── Error.razor            # Página de erro
    │   │   ├── Home.razor             # Dashboard principal
    │   │   └── Weather.razor          # Página de exemplo
    │   ├── App.razor                  # Componente raiz
    │   ├── Routes.razor               # Configuração de rotas
    │   └── _Imports.razor             # Imports globais
    ├── Controllers/                   # Controllers da API
    │   └── ArquivosController.cs      # API para arquivos
    ├── Hubs/                          # SignalR Hubs
    │   └── PresencaHub.cs            # Hub para RFID em tempo real
    ├── Models/                        # Models específicos da Web
    │   └── BrowserFileAsFormFile.cs   # Adapter para upload
    ├── wwwroot/                       # Arquivos estáticos
    │   ├── css/                       # Estilos CSS
    │   │   └── randoncorp-theme.css   # Tema Randoncorp
    │   ├── js/                        # JavaScript
    │   │   └── charts.js              # Gráficos Chart.js
    │   ├── images/                    # Imagens
    │   │   └── randoncorp-logo.svg    # Logo da empresa
    │   ├── bootstrap/                 # Framework CSS
    │   ├── app.css                    # Estilos principais
    │   └── favicon.png                # Ícone do site
    ├── Properties/
    │   └── launchSettings.json        # Configurações de execução
    ├── Program.cs                     # Ponto de entrada da aplicação
    ├── appsettings.json              # Configurações principais
    └── appsettings.Production.example.json # Exemplo para produção
```

## 🎯 Responsabilidades das Camadas

### **DDO.Core (Domínio)**
- **Responsabilidade:** Regras de negócio e entidades
- **Dependências:** Nenhuma (camada mais interna)
- **Contém:** Entidades, Value Objects, Enums, Interfaces de domínio

### **DDO.Application (Casos de Uso)**
- **Responsabilidade:** Orquestração da lógica de aplicação
- **Dependências:** DDO.Core
- **Contém:** Services, Interfaces de repositório, DTOs, Validators

### **DDO.Infrastructure (Infraestrutura)**
- **Responsabilidade:** Acesso a dados e serviços externos
- **Dependências:** DDO.Core, DDO.Application
- **Contém:** Repositórios, DbContext, Configurações de banco

### **DDO.Web (Apresentação)**
- **Responsabilidade:** Interface do usuário e APIs
- **Dependências:** Todas as outras camadas
- **Contém:** Controllers, Views, Components, Middlewares

## 🔄 Fluxo de Dados

```
[Usuário] → [DDO.Web] → [DDO.Application] → [DDO.Infrastructure] → [Banco de Dados]
    ↑                                                                        ↓
    └── [Resposta] ← [View/Component] ← [Service] ← [Repository] ← [DbContext]
```

## 📦 Tecnologias Utilizadas

### **Backend**
- **ASP.NET Core 8** - Framework web
- **Entity Framework Core** - ORM
- **SignalR** - Comunicação em tempo real
- **SQL Server** - Banco de dados principal

### **Frontend**
- **Blazor Server** - Framework de UI
- **Bootstrap 5** - Framework CSS
- **Chart.js** - Gráficos interativos
- **JavaScript** - Interações do cliente

### **DevOps**
- **GitHub Actions** - CI/CD
- **Docker** - Containerização (opcional)
- **IIS** - Hospedagem em produção

## 🚀 Padrões Implementados

### **Arquiteturais**
- ✅ **Clean Architecture** - Separação de responsabilidades
- ✅ **Repository Pattern** - Abstração do acesso a dados
- ✅ **Dependency Injection** - Inversão de controle
- ✅ **CQRS** (parcial) - Separação de comandos e consultas

### **Desenvolvimento**
- ✅ **Single Responsibility** - Uma responsabilidade por classe
- ✅ **Open/Closed** - Aberto para extensão, fechado para modificação
- ✅ **Interface Segregation** - Interfaces específicas
- ✅ **Dependency Inversion** - Depender de abstrações

## 📊 Métricas do Projeto

- **Linhas de Código:** ~3.500 linhas
- **Arquivos C#:** 25 arquivos
- **Componentes Blazor:** 8 componentes
- **Entidades:** 5 entidades principais
- **Repositórios:** 4 repositórios
- **Serviços:** 4 serviços de aplicação

## 🔧 Configurações Importantes

### **Banco de Dados**
- **Connection String:** `appsettings.json`
- **Migrations:** Entity Framework Core
- **Seeding:** Dados iniciais automáticos

### **Autenticação**
- **ASP.NET Identity** - Sistema de usuários
- **Roles:** Admin, Manager, User
- **Cookies** - Autenticação baseada em cookies

### **Upload de Arquivos**
- **Tamanho máximo:** 10MB
- **Formatos aceitos:** PDF
- **Armazenamento:** Sistema de arquivos local

### **RFID Integration**
- **SignalR Hub:** Comunicação em tempo real
- **Timeout:** 30 segundos
- **Duplicação:** Bloqueio de 5 minutos

## 📈 Próximas Melhorias

### **Funcionalidades**
- [ ] Projeto de testes unitários
- [ ] Relatórios em PDF/Excel
- [ ] Notificações por email
- [ ] API REST completa
- [ ] Aplicativo mobile

### **Técnicas**
- [ ] Cache Redis
- [ ] Logs estruturados (Serilog)
- [ ] Health checks
- [ ] Rate limiting
- [ ] Documentação OpenAPI/Swagger

---

*Esta estrutura garante um código limpo, testável e facilmente extensível para futuras funcionalidades.*
