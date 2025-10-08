using DDO.Application.Interfaces;
using DDO.Application.Services;
using DDO.Infrastructure.Data;
using DDO.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURAÇÃO DO BANCO DE DADOS =====
// Configuração do Entity Framework com SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ===== CONFIGURAÇÃO DO IDENTITY =====
// Sistema de autenticação e autorização
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
{
    // Configurações de senha
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
    
    // Configurações de conta
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
    
    // Configurações de login
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// ===== CONFIGURAÇÃO DOS SERVIÇOS DE APLICAÇÃO =====
// Registro dos repositórios (padrão Repository)
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IColaboradorRepository, ColaboradorRepository>();
builder.Services.AddScoped<IPresencaRepository, PresencaRepository>();
builder.Services.AddScoped<IArquivoPDFRepository, ArquivoPDFRepository>();

// Registro dos serviços de aplicação
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<ColaboradorImportService>();
builder.Services.AddScoped<PresencaService>();

// ===== CONFIGURAÇÃO DO BLAZOR =====
// Blazor Server com componentes interativos
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// ===== CONFIGURAÇÃO DO SIGNALR =====
// Para comunicação em tempo real (registro de presença via RFID)
builder.Services.AddSignalR();

// ===== CONFIGURAÇÃO DOS CONTROLLERS =====
// Para endpoints da API REST
builder.Services.AddControllers();

// ===== CONSTRUÇÃO DA APLICAÇÃO =====
var app = builder.Build();

// ===== CONFIGURAÇÃO DO PIPELINE HTTP =====
if (!app.Environment.IsDevelopment())
{
    // Em produção, usa página de erro personalizada
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    
    // Adiciona HSTS (HTTP Strict Transport Security)
    app.UseHsts();
}

// Força redirecionamento para HTTPS
app.UseHttpsRedirection();

// Serve arquivos estáticos (CSS, JS, imagens)
app.UseStaticFiles();

// ===== MIDDLEWARE DE AUTENTICAÇÃO E AUTORIZAÇÃO =====
app.UseAuthentication();
app.UseAuthorization();

// Proteção contra ataques CSRF
app.UseAntiforgery();

// ===== MAPEAMENTO DE ROTAS =====
// Páginas do Identity (login, registro, etc.)
app.MapRazorPages();

// Controllers da API
app.MapControllers();

// Componentes Blazor
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Hub SignalR para presença em tempo real
app.MapHub<DDO.Web.Hubs.PresencaHub>("/presencahub");

// ===== INICIALIZAÇÃO DE DADOS =====
// Popular banco de dados com dados iniciais
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    // Executar o seeder de dados
    await DDO.Infrastructure.Data.DatabaseSeeder.SeedAsync(context, userManager, roleManager);
}

// ===== EXECUÇÃO DA APLICAÇÃO =====
app.Run();
