# Scripts e Dados de Teste

Esta pasta contém scripts e arquivos para popular o banco de dados com dados de teste, facilitando a demonstração e desenvolvimento do sistema DDO.

## Arquivos Disponíveis

### 📄 `seed-data.sql`
Script SQL completo para popular o banco de dados com dados de teste. Inclui:
- 6 áreas de exemplo (TI, RH, Engenharia, Vendas, Marketing, Financeiro)
- 24 colaboradores distribuídos entre as áreas
- Registros de presença dos últimos 7 dias úteis
- Arquivos PDF de exemplo

**Como usar:**
1. Certifique-se de que o banco `DDO_ControlePonto` existe
2. Execute o script no SQL Server Management Studio ou Azure Data Studio
3. O script limpa dados existentes e insere os dados de teste

### 📊 `colaboradores-exemplo.csv`
Arquivo CSV de exemplo para testar a funcionalidade de importação de colaboradores.

**Formato do CSV:**
```
Nome,Matricula,CodigoRFID,Email,Telefone,Area
```

**Como usar:**
1. Acesse o sistema DDO
2. Vá para "Colaboradores" > "Importar CSV"
3. Faça upload do arquivo `colaboradores-exemplo.csv`
4. Os colaboradores serão importados automaticamente

## Dados Incluídos

### Áreas
- **Tecnologia da Informação**: 5 colaboradores
- **Recursos Humanos**: 4 colaboradores  
- **Engenharia**: 5 colaboradores
- **Vendas**: 4 colaboradores
- **Marketing**: 3 colaboradores
- **Financeiro**: 3 colaboradores

### Colaboradores
Cada colaborador possui:
- Nome completo realista
- Matrícula única (formato RDC0001)
- Código RFID único
- Email corporativo (@randoncorp.com)
- Telefone com DDD de Porto Alegre (51)
- Status ativo (90% dos colaboradores)

### Registros de Presença
- Dados dos últimos 7 dias úteis (excluindo fins de semana)
- Simulação realista de horários de entrada, saída para almoço e saída final
- 80% de taxa de comparecimento simulada
- Diferentes tipos de registro (Entrada/Saída)
- Registros via RFID

### Arquivos PDF
- 6 arquivos PDF de exemplo
- Distribuídos entre diferentes áreas
- Metadados realistas (tamanho, data de upload, etc.)

## Seeder Automático

O sistema também inclui um seeder automático (`DatabaseSeeder.cs`) que é executado automaticamente quando:
- O ambiente é Development
- O banco de dados está vazio
- A aplicação é iniciada

Este seeder cria dados mais robustos, incluindo:
- 35 colaboradores com nomes realistas
- Registros de presença dos últimos 30 dias
- Distribuição mais realista de dados

## Observações Importantes

⚠️ **Atenção**: Os scripts SQL incluem comandos `DELETE` que removem dados existentes. Use com cuidado em ambientes que já possuem dados importantes.

✅ **Recomendação**: Use sempre em bancos de desenvolvimento ou teste.

🔄 **Regeneração**: Você pode executar os scripts quantas vezes quiser para resetar os dados de teste.

## Credenciais de Teste

Após popular os dados, use estas credenciais para login:
- **Email**: `admin@randoncorp.com`
- **Senha**: `Admin@123456`

## Suporte

Se encontrar problemas com os scripts ou dados de teste, verifique:
1. Se o banco de dados existe e está acessível
2. Se as migrations foram aplicadas corretamente
3. Se as permissões do usuário do banco estão adequadas
4. Os logs da aplicação para mensagens de erro detalhadas
