# 🤝 Contribuindo para o DDO - Controle de Presença

Obrigado por considerar contribuir para o projeto DDO! Este documento fornece diretrizes para contribuições efetivas.

## 📋 Código de Conduta

Este projeto adere a um código de conduta. Ao participar, você deve manter um ambiente respeitoso e colaborativo.

## 🚀 Como Contribuir

### **Reportando Bugs**

Antes de reportar um bug, verifique se ele já não foi reportado nas [Issues](https://github.com/Kraunser/ddo-controle-presenca/issues).

**Para reportar um bug:**
1. Use o template de bug report
2. Inclua informações do ambiente (OS, .NET version, browser)
3. Descreva os passos para reproduzir
4. Inclua screenshots se aplicável
5. Adicione logs de erro relevantes

### **Sugerindo Melhorias**

**Para sugerir uma nova funcionalidade:**
1. Verifique se já não existe uma issue similar
2. Use o template de feature request
3. Descreva claramente o problema que resolve
4. Explique a solução proposta
5. Considere implementações alternativas

### **Contribuindo com Código**

#### **Configuração do Ambiente**

1. **Fork o repositório**
2. **Clone seu fork:**
   ```bash
   git clone https://github.com/SEU-USUARIO/ddo-controle-presenca.git
   cd ddo-controle-presenca
   ```

3. **Configure o upstream:**
   ```bash
   git remote add upstream https://github.com/Kraunser/ddo-controle-presenca.git
   ```

4. **Instale dependências:**
   ```bash
   dotnet restore
   ```

5. **Configure o banco de dados:**
   ```bash
   cd DDO.Web
   dotnet ef database update
   ```

#### **Fluxo de Desenvolvimento**

1. **Crie uma branch para sua feature:**
   ```bash
   git checkout -b feature/nome-da-feature
   ```

2. **Faça suas alterações seguindo os padrões do projeto**

3. **Teste suas alterações:**
   ```bash
   dotnet build
   dotnet run
   ```

4. **Commit suas mudanças:**
   ```bash
   git add .
   git commit -m "feat: adiciona nova funcionalidade X"
   ```

5. **Push para seu fork:**
   ```bash
   git push origin feature/nome-da-feature
   ```

6. **Abra um Pull Request**

## 📝 Padrões de Código

### **Convenções de Nomenclatura**

**C# Classes e Métodos:**
```csharp
public class ColaboradorService  // PascalCase
{
    public async Task<List<Colaborador>> ObterTodosAsync()  // PascalCase
    {
        var colaboradores = await _repository.GetAllAsync();  // camelCase para variáveis
        return colaboradores;
    }
}
```

**Blazor Components:**
```razor
@* PascalCase para componentes *@
<ComponenteCustomizado Propriedade="valor" />

@* camelCase para variáveis JavaScript *@
<script>
    const minhaVariavel = 'valor';
</script>
```

### **Estrutura de Arquivos**

**Entidades (DDO.Core/Entities/):**
```csharp
public class MinhaEntidade
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    
    // Navegação
    public virtual ICollection<OutraEntidade> Itens { get; set; } = new List<OutraEntidade>();
}
```

**Repositórios (DDO.Infrastructure/Repositories/):**
```csharp
public class MinhaEntidadeRepository : IMinhaEntidadeRepository
{
    private readonly ApplicationDbContext _context;
    
    public MinhaEntidadeRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<MinhaEntidade>> GetAllAsync()
    {
        return await _context.MinhasEntidades
            .AsNoTracking()
            .ToListAsync();
    }
}
```

**Serviços (DDO.Application/Services/):**
```csharp
public class MinhaEntidadeService
{
    private readonly IMinhaEntidadeRepository _repository;
    
    public MinhaEntidadeService(IMinhaEntidadeRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<ServiceResult<MinhaEntidade>> CriarAsync(MinhaEntidade entidade)
    {
        // Validações
        // Lógica de negócio
        // Persistência
    }
}
```

### **Padrões de Commit**

Use [Conventional Commits](https://www.conventionalcommits.org/):

```
feat: adiciona nova funcionalidade de relatórios
fix: corrige erro no upload de arquivos
docs: atualiza documentação da API
style: formata código seguindo padrões
refactor: refatora serviço de presença
test: adiciona testes para colaborador service
chore: atualiza dependências do projeto
```

### **Documentação de Código**

**XML Documentation para métodos públicos:**
```csharp
/// <summary>
/// Obtém todos os colaboradores ativos do sistema
/// </summary>
/// <param name="filtro">Filtro opcional para busca</param>
/// <returns>Lista de colaboradores encontrados</returns>
/// <exception cref="ArgumentException">Quando o filtro é inválido</exception>
public async Task<List<Colaborador>> ObterColaboradoresAsync(string? filtro = null)
{
    // Implementação
}
```

## 🧪 Testes

### **Executando Testes**
```bash
dotnet test
```

### **Escrevendo Testes**
```csharp
[Test]
public async Task ObterColaboradores_DeveRetornarListaVazia_QuandoNaoHouverDados()
{
    // Arrange
    var service = new ColaboradorService(_mockRepository.Object);
    
    // Act
    var resultado = await service.ObterTodosAsync();
    
    // Assert
    Assert.That(resultado, Is.Empty);
}
```

## 📊 Performance

### **Diretrizes de Performance**

**Entity Framework:**
- Use `AsNoTracking()` para consultas read-only
- Implemente paginação para listas grandes
- Use `Include()` para eager loading quando necessário
- Evite N+1 queries

**Blazor:**
- Use `@key` para listas dinâmicas
- Implemente `ShouldRender()` quando apropriado
- Use `StateHasChanged()` com parcimônia
- Otimize re-renders desnecessários

## 🔒 Segurança

### **Práticas de Segurança**

**Validação de Entrada:**
```csharp
[Required(ErrorMessage = "Nome é obrigatório")]
[StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
public string Nome { get; set; } = string.Empty;
```

**Autorização:**
```csharp
[Authorize(Roles = "Admin,Manager")]
public async Task<IActionResult> AcaoRestrita()
{
    // Implementação
}
```

**Sanitização:**
```csharp
public string SanitizeInput(string input)
{
    return HttpUtility.HtmlEncode(input?.Trim());
}
```

## 📋 Checklist do Pull Request

Antes de submeter um PR, verifique:

- [ ] Código segue os padrões estabelecidos
- [ ] Testes foram adicionados/atualizados
- [ ] Documentação foi atualizada se necessário
- [ ] Build passa sem warnings
- [ ] Funcionalidade foi testada manualmente
- [ ] Commit messages seguem o padrão
- [ ] Branch está atualizada com main
- [ ] PR tem descrição clara do que foi alterado

## 🎯 Áreas que Precisam de Contribuição

### **Alta Prioridade**
- Testes unitários e de integração
- Documentação da API
- Melhorias de performance
- Correção de bugs reportados

### **Média Prioridade**
- Novas funcionalidades
- Melhorias na UI/UX
- Refatoração de código legado
- Otimizações de banco de dados

### **Baixa Prioridade**
- Melhorias na documentação
- Exemplos de uso
- Scripts de automação
- Configurações de deploy

## 📞 Suporte

**Dúvidas sobre contribuição:**
- Abra uma [Discussion](https://github.com/Kraunser/ddo-controle-presenca/discussions)
- Comente em issues existentes
- Entre em contato via email (se disponível)

**Problemas técnicos:**
- Verifique a documentação primeiro
- Procure em issues fechadas
- Abra uma nova issue com detalhes

---

**Obrigado por contribuir! Sua ajuda torna este projeto melhor para todos.** 🚀
