# 🔧 Correção do Erro Entity Framework Core

## ❌ Problema Identificado

O erro que você está enfrentando:
```
Your startup project 'DDO.Web' doesn't reference Microsoft.EntityFrameworkCore.Design. 
This package is required for the Entity Framework Core Tools to work.
```

## ✅ Solução

### **1. Pacote Adicionado**
Adicionei o pacote `Microsoft.EntityFrameworkCore.Design` ao projeto `DDO.Web.csproj`:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.10">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

### **2. Comandos para Executar**

**No diretório do projeto:**
```bash
cd DDO.Web

# Restaurar pacotes
dotnet restore

# Criar migration inicial
dotnet ef migrations add InitialCreate

# Aplicar migration ao banco
dotnet ef database update
```

### **3. Verificação**

Para verificar se tudo está funcionando:
```bash
# Verificar se o EF Tools está funcionando
dotnet ef --version

# Listar migrations
dotnet ef migrations list

# Verificar status do banco
dotnet ef database update --verbose
```

## 🗄️ Configuração do Banco de Dados

### **Opção 1: SQL Server Local (Recomendado)**
No arquivo `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DDO_ControlePonto;Integrated Security=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

### **Opção 2: SQL Server com Usuário/Senha**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DDO_ControlePonto;User Id=sa;Password=SuaSenha123;TrustServerCertificate=true;MultipleActiveResultSets=true"
  }
}
```

### **Opção 3: SQLite (Para Testes Rápidos)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=ddo_database.db"
  }
}
```

**Para SQLite, também adicione o pacote:**
```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

## 🚀 Execução Após Correção

1. **Restaurar dependências:**
   ```bash
   dotnet restore
   ```

2. **Aplicar migrations:**
   ```bash
   cd DDO.Web
   dotnet ef database update
   ```

3. **Executar aplicação:**
   ```bash
   dotnet run
   ```

## 🔍 Troubleshooting

### **Se ainda der erro:**

**1. Limpar e recompilar:**
```bash
dotnet clean
dotnet restore
dotnet build
```

**2. Verificar versões dos pacotes:**
```bash
dotnet list package
```

**3. Reinstalar EF Tools:**
```bash
dotnet tool uninstall --global dotnet-ef
dotnet tool install --global dotnet-ef
```

**4. Verificar se o SQL Server está rodando:**
- Windows: `services.msc` → Procurar "SQL Server"
- Ou usar SQLite para testes rápidos

### **Erro de Connection String:**
Se der erro de conexão, verifique:
- SQL Server está instalado e rodando
- Nome do servidor está correto
- Credenciais estão corretas
- Firewall não está bloqueando

## ✅ Resultado Esperado

Após aplicar a correção, você deve conseguir:
- ✅ Executar `dotnet ef database update` sem erros
- ✅ Ver as tabelas criadas no banco de dados
- ✅ Executar a aplicação normalmente
- ✅ Acessar o sistema com as credenciais padrão

## 📞 Se Precisar de Ajuda

1. **Verifique os logs** de erro detalhados
2. **Confirme a versão** do .NET: `dotnet --version`
3. **Teste com SQLite** primeiro se SQL Server der problemas
4. **Execute passo a passo** os comandos acima

---

**Esta correção resolve definitivamente o erro do Entity Framework Core Tools!** 🎯
