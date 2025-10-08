@echo off
echo 🚀 Iniciando DDO - Controle de Presença...
echo.

REM Verificar se o .NET está instalado
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ .NET não encontrado. Instale o .NET 8 SDK primeiro.
    pause
    exit /b 1
)

echo ✅ .NET encontrado
for /f "tokens=*" %%i in ('dotnet --version') do echo    Versão: %%i

REM Navegar para o diretório do projeto
cd DDO.Web

echo.
echo 📦 Restaurando dependências...
dotnet restore

echo.
echo 🔨 Compilando projeto...
dotnet build

if %errorlevel% equ 0 (
    echo.
    echo 🎉 Compilação bem-sucedida!
    echo.
    echo 🌐 Iniciando aplicação web...
    echo    - URL: http://localhost:5000
    echo    - HTTPS: https://localhost:7252
    echo.
    echo 👤 Credenciais padrão:
    echo    - Email: admin@randoncorp.com
    echo    - Senha: Admin@123456
    echo.
    echo ⚠️  IMPORTANTE: Configure o banco de dados no appsettings.json antes de usar!
    echo.
    echo 🛑 Para parar a aplicação, pressione Ctrl+C
    echo.
    
    REM Executar a aplicação
    dotnet run --urls="http://localhost:5000;https://localhost:7252"
) else (
    echo.
    echo ❌ Erro na compilação. Verifique os erros acima.
    pause
    exit /b 1
)

pause
