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
        foreach (var rol in new[] { RolAdmin, RolCliente })
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
    }
}
