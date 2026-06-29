using CRM_ALMACEN.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CRM_ALMACEN.Data;

/// <summary>
/// Crea (la primera vez) los roles, el usuario administrador y datos de ejemplo.
/// Se ejecuta al arrancar la aplicación.
/// </summary>
public static class DbSeeder
{
    public const string RolAdmin = "Admin";
    public const string RolCliente = "Cliente";

    // Roles del personal del almacén (colaboradores internos, no clientes).
    public const string RolAlmacenista = "Almacenista";
    public const string RolCobranza = "Cobranza";
    public const string RolAtencion = "AtencionClientes";
    public const string RolSupervisor = "Supervisor";

    /// <summary>Todos los roles que existen en el sistema.</summary>
    public static readonly string[] TodosLosRoles =
        [RolAdmin, RolCliente, RolAlmacenista, RolCobranza, RolAtencion, RolSupervisor];

    /// <summary>
    /// Roles internos que el administrador puede asignar al personal, con su
    /// nombre legible para mostrar en pantalla.
    /// </summary>
    public static readonly IReadOnlyList<(string Rol, string Nombre)> RolesInternos =
    [
        (RolAdmin, "Administrador (acceso total)"),
        (RolAlmacenista, "Almacenista / Operador"),
        (RolCobranza, "Cobranza / Facturación"),
        (RolAtencion, "Atención a clientes"),
        (RolSupervisor, "Supervisor (solo lectura)")
    ];

    /// <summary>Nombre legible de un rol; si no se conoce, devuelve el mismo código.</summary>
    public static string NombreRol(string rol) =>
        RolesInternos.FirstOrDefault(r => r.Rol == rol).Nombre ?? rol;

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        var db = sp.GetRequiredService<ApplicationDbContext>();
        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();

        // 1. Aplicar migraciones pendientes (crea la BD si no existe)
        await db.Database.MigrateAsync();

        // 2. Roles
        foreach (var rol in TodosLosRoles)
        {
            if (!await roleManager.RoleExistsAsync(rol))
                await roleManager.CreateAsync(new IdentityRole(rol));
        }

        // 3. Usuario administrador
        const string adminEmail = "admin@almacen.com";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                NombreCompleto = "Administrador del Almacén"
            };
            await userManager.CreateAsync(admin, "Admin123$");
            await userManager.AddToRoleAsync(admin, RolAdmin);
        }

        // 3.5 Mapa del almacén: ubicaciones (racks/niveles/posiciones).
        // Se crea la primera vez si todavía no hay ninguna. Layout de ejemplo:
        // 4 caras de rack (A, B = Pasillo 1; C, D = Pasillo 2), 3 niveles, 50 posiciones.
        if (!await db.Ubicaciones.AnyAsync())
        {
            var racks = new[]
            {
                (Rack: "A", Zona: "Pasillo 1"),
                (Rack: "B", Zona: "Pasillo 1"),
                (Rack: "C", Zona: "Pasillo 2"),
                (Rack: "D", Zona: "Pasillo 2"),
            };
            const int niveles = 3;
            const int posiciones = 50;

            foreach (var (rack, zona) in racks)
            {
                for (var nivel = 1; nivel <= niveles; nivel++)
                {
                    for (var pos = 1; pos <= posiciones; pos++)
                    {
                        db.Ubicaciones.Add(new Ubicacion
                        {
                            Zona = zona,
                            Rack = rack,
                            Nivel = nivel,
                            Posicion = pos,
                            Codigo = Ubicacion.ArmarCodigo(rack, nivel, pos),
                            Capacidad = 1,
                            Activo = true
                        });
                    }
                }
            }
            await db.SaveChangesAsync();
        }

        // Datos de ejemplo (cliente demo, pagos, pedido) DESACTIVADOS para operar
        // con datos reales. Cambia a true si quieres volver a sembrar el demo.
        var sembrarDatosDemo = false;
        if (!sembrarDatosDemo) return;

        // 4. Cliente de ejemplo + su usuario + productos
        if (!await db.Clientes.AnyAsync())
        {
            var cliente = new Cliente
            {
                Nombre = "Distribuidora Demo S.A. de C.V.",
                Rfc = "DDE010101AAA",
                Correo = "contacto@demo.com",
                Telefono = "55-1234-5678"
            };
            db.Clientes.Add(cliente);
            await db.SaveChangesAsync();

            db.Productos.AddRange(
                new Producto { ClienteId = cliente.Id, Codigo = "SKU-001", Nombre = "Caja de tornillos 1\"", UnidadMedida = "CAJA", CantidadDisponible = 120, StockMinimo = 20 },
                new Producto { ClienteId = cliente.Id, Codigo = "SKU-002", Nombre = "Rollo de cable calibre 12", UnidadMedida = "ROLLO", CantidadDisponible = 15, StockMinimo = 10 },
                new Producto { ClienteId = cliente.Id, Codigo = "SKU-003", Nombre = "Casco de seguridad amarillo", UnidadMedida = "PZA", CantidadDisponible = 8, StockMinimo = 25 }
            );
            await db.SaveChangesAsync();

            const string clienteEmail = "cliente@demo.com";
            if (await userManager.FindByEmailAsync(clienteEmail) is null)
            {
                var userCliente = new ApplicationUser
                {
                    UserName = clienteEmail,
                    Email = clienteEmail,
                    EmailConfirmed = true,
                    NombreCompleto = "Usuario Distribuidora Demo",
                    ClienteId = cliente.Id
                };
                await userManager.CreateAsync(userCliente, "Cliente123$");
                await userManager.AddToRoleAsync(userCliente, RolCliente);
            }
        }

        // 5. Cargos del periodo actual y pagos de ejemplo (cliente demo)
        if (!await db.Cargos.AnyAsync())
        {
            var cliente = await db.Clientes.OrderBy(c => c.Id).FirstOrDefaultAsync();
            if (cliente is not null)
            {
                var hoy = DateTime.Now;
                db.Cargos.AddRange(
                    new Cargo { ClienteId = cliente.Id, Anio = hoy.Year, Mes = hoy.Month, Concepto = "Renta de tarima mediana", Cantidad = 15, Monto = 8004.00m },
                    new Cargo { ClienteId = cliente.Id, Anio = hoy.Year, Mes = hoy.Month, Concepto = "Oficinas", Cantidad = 1, Monto = 4060.00m },
                    new Cargo { ClienteId = cliente.Id, Anio = hoy.Year, Mes = hoy.Month, Concepto = "Crossdock - Tarima mediana", Cantidad = 8, Monto = 1856.00m }
                );

                db.Pagos.AddRange(
                    new Pago { ClienteId = cliente.Id, Fecha = hoy.AddDays(-40), Monto = 10622.44m, MetodoPago = MetodoPago.Transferencia, Referencia = "TRX-001" },
                    new Pago { ClienteId = cliente.Id, Fecha = hoy.AddDays(-25), Monto = 1450.00m, MetodoPago = MetodoPago.Efectivo },
                    new Pago { ClienteId = cliente.Id, Fecha = hoy.AddDays(-10), Monto = 12683.44m, MetodoPago = MetodoPago.Transferencia, Referencia = "TRX-002" }
                );
                await db.SaveChangesAsync();
            }
        }

        // 6. Pedido (solicitud) de ejemplo para el tablero
        if (!await db.Solicitudes.AnyAsync())
        {
            var cliente = await db.Clientes.OrderBy(c => c.Id).FirstOrDefaultAsync();
            var productos = await db.Productos.OrderBy(p => p.Id).Take(2).ToListAsync();
            if (cliente is not null && productos.Count >= 2)
            {
                db.Solicitudes.Add(new SolicitudPedido
                {
                    ClienteId = cliente.Id,
                    Fecha = DateTime.Now.AddHours(-3),
                    Estado = EstadoSolicitud.Pendiente,
                    Notas = "Pedido de ejemplo para el tablero.",
                    SolicitadoPor = "cliente@demo.com",
                    Detalles =
                    [
                        new DetalleSolicitud { ProductoId = productos[0].Id, Cantidad = 5 },
                        new DetalleSolicitud { ProductoId = productos[1].Id, Cantidad = 3 }
                    ]
                });
                await db.SaveChangesAsync();
            }
        }
    }
}
