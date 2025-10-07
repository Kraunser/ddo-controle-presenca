using DDO.Web.Components;
using DDO.Infrastructure.Data;
using DDO.Infrastructure.Repositories;
using DDO.Application.Interfaces;
using DDO.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURAÇÃO DO BANCO DE DADOS =====
// Obtém a connection string do arquivo de configuração
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Configura o Entity Framework Core com SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ===== CONFIGURAÇÃO DO ASP.NET CORE IDENTITY =====
// Configura o sistema de autenticação e autorização
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
{
    // Configurações de política de senha
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Configurações de bloqueio de conta
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Configurações de usuário
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // Configurações de confirmação (desabilitado para desenvolvimento)
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// ===== CONFIGURAÇÃO DE AUTORIZAÇÃO =====
// Define políticas de autorização baseadas em roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole", 
        policy => policy.RequireRole("Administrator"));
    
    options.AddPolicy("RequireManagerRole", 
        policy => policy.RequireRole("Manager", "Administrator"));
    
    options.AddPolicy("RequireUserRole", 
        policy => policy.RequireRole("User", "Manager", "Administrator"));
});

// ===== REGISTRO DOS REPOSITÓRIOS =====
// Implementações dos padrões Repository para acesso a dados
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IArquivoPDFRepository, ArquivoPDFRepository>();
builder.Services.AddScoped<IColaboradorRepository, ColaboradorRepository>();
builder.Services.AddScoped<IPresencaRepository, PresencaRepository>();

// ===== REGISTRO DOS SERVIÇOS DE APLICAÇÃO =====
// Serviços que contêm a lógica de negócio da aplicação
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<ColaboradorImportService>();
builder.Services.AddScoped<PresencaService>();
builder.Services.AddScoped<DashboardService>();

// ===== CONFIGURAÇÃO DO BLAZOR =====
// Adiciona suporte ao Blazor Server com componentes interativos
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

// Hub SignalR para comunicação em tempo real
app.MapHub<DDO.Web.Hubs.PresencaHub>("/presencahub");

// Componentes Blazor
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// ===== INICIALIZAÇÃO DO BANCO DE DADOS =====
// Executa na inicialização da aplicação
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        // Aplica migrações pendentes automaticamente
        await context.Database.MigrateAsync();
        
        // Cria roles e usuário administrador padrão
        await SeedDataAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro durante a inicialização do banco de dados.");
        throw;
    }
}

// Inicia a aplicação
app.Run();

/// <summary>
/// Popula o banco de dados com dados iniciais necessários
/// </summary>
/// <param name="userManager">Gerenciador de usuários do Identity</param>
/// <param name="roleManager">Gerenciador de roles do Identity</param>
static async Task SeedDataAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
{
    await CreateDefaultRolesAsync(roleManager);
    await CreateDefaultAdminAsync(userManager);
}

/// <summary>
/// Cria as roles padrão do sistema se não existirem
/// </summary>
/// <param name="roleManager">Gerenciador de roles do Identity</param>
static async Task CreateDefaultRolesAsync(RoleManager<IdentityRole> roleManager)
{
    string[] roleNames = { "Administrator", "Manager", "User" };
    
    foreach (var roleName in roleNames)
    {
        var roleExists = await roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            var role = new IdentityRole(roleName);
            await roleManager.CreateAsync(role);
        }
    }
}

/// <summary>
/// Cria o usuário administrador padrão se não existir
/// </summary>
/// <param name="userManager">Gerenciador de usuários do Identity</param>
static async Task CreateDefaultAdminAsync(UserManager<IdentityUser> userManager)
{
    const string adminEmail = "admin@randoncorp.com";
    const string adminPassword = "Admin@123456";
    
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    
    if (adminUser == null)
    {
        var newAdminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        
        var result = await userManager.CreateAsync(newAdminUser, adminPassword);
        
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdminUser, "Administrator");
        }
        else
        {
            throw new InvalidOperationException($"Falha ao criar usuário administrador: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}
