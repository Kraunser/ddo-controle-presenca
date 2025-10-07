using DDO.Web.Components;
using DDO.Infrastructure.Data;
using DDO.Infrastructure.Repositories;
using DDO.Application.Interfaces;
using DDO.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

<<<<<<< HEAD
// Configuração do banco de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=DDO_ControlePonto;Trusted_Connection=true;MultipleActiveResultSets=true";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configuração do ASP.NET Core Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
{
    // Configurações de senha
=======
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
>>>>>>> b90a182 (Initial commit of DDO project)
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
<<<<<<< HEAD
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Configurações de lockout
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
=======
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Configurações de bloqueio de conta
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
>>>>>>> b90a182 (Initial commit of DDO project)
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Configurações de usuário
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

<<<<<<< HEAD
    // Configurações de confirmação de email (desabilitado para desenvolvimento)
=======
    // Configurações de confirmação (desabilitado para desenvolvimento)
>>>>>>> b90a182 (Initial commit of DDO project)
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

<<<<<<< HEAD
// Configuração de autorização
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("RequireManagerRole", policy => policy.RequireRole("Manager", "Administrator"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Manager", "Administrator"));
});

// Registro dos repositórios
=======
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
>>>>>>> b90a182 (Initial commit of DDO project)
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IArquivoPDFRepository, ArquivoPDFRepository>();
builder.Services.AddScoped<IColaboradorRepository, ColaboradorRepository>();
builder.Services.AddScoped<IPresencaRepository, PresencaRepository>();

<<<<<<< HEAD
// Registro dos serviços
=======
// ===== REGISTRO DOS SERVIÇOS DE APLICAÇÃO =====
// Serviços que contêm a lógica de negócio da aplicação
>>>>>>> b90a182 (Initial commit of DDO project)
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<ColaboradorImportService>();
builder.Services.AddScoped<PresencaService>();
builder.Services.AddScoped<DashboardService>();

<<<<<<< HEAD
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configuração do SignalR para comunicação em tempo real (RFID)
builder.Services.AddSignalR();

// Adicionar controllers para API
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Middleware de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

// Mapeamento das páginas do Identity
app.MapRazorPages();

// Mapeamento dos controllers da API
app.MapControllers();

// Mapeamento do hub SignalR
app.MapHub<DDO.Web.Hubs.PresencaHub>("/presencahub");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Inicialização do banco de dados
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    // Aplicar migrações pendentes
    context.Database.EnsureCreated();
    
    // Criar roles padrão
    await CreateDefaultRoles(roleManager);
    
    // Criar usuário administrador padrão
    await CreateDefaultAdmin(userManager);
}

app.Run();

/// <summary>
/// Cria as roles padrão do sistema
/// </summary>
static async Task CreateDefaultRoles(RoleManager<IdentityRole> roleManager)
=======
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
>>>>>>> b90a182 (Initial commit of DDO project)
{
    string[] roleNames = { "Administrator", "Manager", "User" };
    
    foreach (var roleName in roleNames)
    {
<<<<<<< HEAD
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
=======
        var roleExists = await roleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            var role = new IdentityRole(roleName);
            await roleManager.CreateAsync(role);
>>>>>>> b90a182 (Initial commit of DDO project)
        }
    }
}

/// <summary>
<<<<<<< HEAD
/// Cria o usuário administrador padrão
/// </summary>
static async Task CreateDefaultAdmin(UserManager<IdentityUser> userManager)
{
    var adminEmail = "admin@randoncorp.com";
=======
/// Cria o usuário administrador padrão se não existir
/// </summary>
/// <param name="userManager">Gerenciador de usuários do Identity</param>
static async Task CreateDefaultAdminAsync(UserManager<IdentityUser> userManager)
{
    const string adminEmail = "admin@randoncorp.com";
    const string adminPassword = "Admin@123456";
    
>>>>>>> b90a182 (Initial commit of DDO project)
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    
    if (adminUser == null)
    {
        var newAdminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        
<<<<<<< HEAD
        var result = await userManager.CreateAsync(newAdminUser, "Admin@123");
=======
        var result = await userManager.CreateAsync(newAdminUser, adminPassword);
>>>>>>> b90a182 (Initial commit of DDO project)
        
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdminUser, "Administrator");
        }
<<<<<<< HEAD
=======
        else
        {
            throw new InvalidOperationException($"Falha ao criar usuário administrador: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
>>>>>>> b90a182 (Initial commit of DDO project)
    }
}
