#!/bin/bash

echo "🚀 Iniciando DDO - Controle de Presença..."
echo ""

# Verificar se o .NET está instalado
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET não encontrado. Instale o .NET 8 SDK primeiro."
    exit 1
fi

echo "✅ .NET encontrado: $(dotnet --version)"

# Navegar para o diretório do projeto
cd DDO.Web

echo "📦 Restaurando dependências..."
dotnet restore

echo "🔨 Compilando projeto..."
dotnet build

if [ $? -eq 0 ]; then
    echo ""
    echo "🎉 Compilação bem-sucedida!"
    echo ""
    echo "🌐 Iniciando aplicação web..."
    echo "   - URL: http://localhost:5000"
    echo "   - HTTPS: https://localhost:7252"
    echo ""
    echo "👤 Credenciais padrão:"
    echo "   - Email: admin@randoncorp.com"
    echo "   - Senha: Admin@123456"
    echo ""
    echo "⚠️  IMPORTANTE: Configure o banco de dados no appsettings.json antes de usar!"
    echo ""
    echo "🛑 Para parar a aplicação, pressione Ctrl+C"
    echo ""
    
    # Executar a aplicação
    dotnet run --urls="http://localhost:5000;https://localhost:7252"
else
    echo "❌ Erro na compilação. Verifique os erros acima."
    exit 1
fi
