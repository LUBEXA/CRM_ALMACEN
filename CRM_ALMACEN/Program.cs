using CRM_ALMACEN.Components;
using CRM_ALMACEN.Components.Account;
using CRM_ALMACEN.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// PostgreSQL guarda fechas como hora local (sin zona horaria), igual que el sistema
// las maneja. Evita el requisito de UTC de Npgsql para un negocio de una sola zona.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// --- Blazor (componentes interactivos del lado servidor) ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// --- Estado de autenticación en cascada para toda la app ---
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// --- Cookies de Identity ---
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
    })
    .AddIdentityCookies();

// --- Base de datos SQL Server ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Falta la cadena de conexión 'DefaultConnection'.");
// Fábrica de contextos: cada pantalla Blazor crea su propio contexto de corta vida.
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
// Contexto con ámbito (scoped) que Identity necesita, creado desde la fábrica.
builder.Services.AddScoped<ApplicationDbContext>(sp =>
    sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

// SignInManager necesita acceso al HttpContext.
builder.Services.AddHttpContextAccessor();

// --- Identity (usuarios y roles) ---
builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddClaimsPrincipalFactory<AdditionalUserClaimsPrincipalFactory>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// --- Pipeline HTTP ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// --- Endpoints de cuenta (login / logout que requieren HttpContext) ---
app.MapAccountEndpoints();

// --- Crear/actualizar la BD y sembrar datos al arrancar ---
await DbSeeder.SeedAsync(app.Services);

app.Run();
