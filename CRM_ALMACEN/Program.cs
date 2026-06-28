using CRM_ALMACEN.Components;
using CRM_ALMACEN.Components.Account;
using CRM_ALMACEN.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// PostgreSQL guarda fechas como hora local (sin zona horaria), igual que el sistema
// las maneja. Evita el requisito de UTC de Npgsql para un negocio de una sola zona.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// QuestPDF: licencia gratuita Community.
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

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

// Servicio para guardar archivos subidos (logos, constancias).
builder.Services.AddScoped<AlmacenamientoArchivos>();

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

// --- Permisos por módulo (políticas que leen los permisos guardados del usuario) ---
builder.Services.AddAuthorizationCore(options =>
{
    foreach (var (modulo, _) in Permisos.Modulos)
    {
        var m = modulo;
        options.AddPolicy($"Ver:{m}", p => p.RequireAuthenticatedUser()
            .RequireAssertion(ctx => ctx.User.PuedeVer(m)));
        options.AddPolicy($"Editar:{m}", p => p.RequireAuthenticatedUser()
            .RequireAssertion(ctx => ctx.User.PuedeEditarModulo(m)));
    }

    // Ocupación y Ubicaciones son herramientas internas del almacén: solo el personal,
    // nunca el cliente, aunque el cliente pueda ver su inventario.
    options.AddPolicy("Ver:OcupacionAlmacen", p => p.RequireAuthenticatedUser()
        .RequireAssertion(ctx => ctx.User.EsPersonal() && ctx.User.PuedeVer(ModuloApp.Inventario)));

    // Pantallas exclusivas del cliente (no del personal del almacén).
    options.AddPolicy("EsCliente", p => p.RequireAuthenticatedUser()
        .RequireAssertion(ctx => ctx.User.ClienteId() is not null));

    // El menú muestra la sección "Administración" si el usuario ve alguno de sus módulos.
    options.AddPolicy("VerAdministracion", p => p.RequireAuthenticatedUser()
        .RequireAssertion(ctx =>
            ctx.User.PuedeVer(ModuloApp.Clientes) ||
            ctx.User.PuedeVer(ModuloApp.Productos) ||
            ctx.User.PuedeVer(ModuloApp.ListaPrecios)));
});

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
