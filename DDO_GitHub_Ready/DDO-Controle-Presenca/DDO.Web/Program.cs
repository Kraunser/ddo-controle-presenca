using DDO.Web.Components;
using DDO.Infrastructure.Data;
using DDO.Infrastructure.Repositories;
using DDO.Application.Interfaces;
using DDO.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuração do banco de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=(localdb)\\mssqllocaldb;Database=DDO_ControlePonto;Trusted_Connection=true;MultipleActiveResultSets=true";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configuração do ASP.NET Core Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
{
    // Configurações de senha
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Configurações de lockout
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // Configurações de usuário
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // Configurações de confirmação de email (desabilitado para desenvolvimento)
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Configuração de autorização
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("RequireManagerRole", policy => policy.RequireRole("Manager", "Administrator"));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Manager", "Administrator"));
});

// Registro dos repositórios
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IArquivoPDFRepository, ArquivoPDFRepository>();
builder.Services.AddScoped<IColaboradorRepository, ColaboradorRepository>();
builder.Services.AddScoped<IPresencaRepository, PresencaRepository>();

// Registro dos serviços
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<ColaboradorImportService>();
builder.Services.AddScoped<PresencaService>();
builder.Services.AddScoped<DashboardService>();

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
{
    string[] roleNames = { "Administrator", "Manager", "User" };
    
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

/// <summary>
/// Cria o usuário administrador padrão
/// </summary>
static async Task CreateDefaultAdmin(UserManager<IdentityUser> userManager)
{
    var adminEmail = "admin@randoncorp.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    
    if (adminUser == null)
    {
        var newAdminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };
        
        var result = await userManager.CreateAsync(newAdminUser, "Admin@123");
        
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newAdminUser, "Administrator");
        }
    }
}
