using CRM_ALMACEN.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CRM_ALMACEN.Data;

/// <summary>
/// Contexto de base de datos. Incluye las tablas del almacén y las de Identity
/// (usuarios y roles) heredadas de IdentityDbContext.
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Entrada> Entradas => Set<Entrada>();
    public DbSet<Salida> Salidas => Set<Salida>();
    public DbSet<SolicitudPedido> Solicitudes => Set<SolicitudPedido>();
    public DbSet<DetalleSolicitud> DetallesSolicitud => Set<DetalleSolicitud>();
    public DbSet<Cargo> Cargos => Set<Cargo>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<CostoServicio> CostosServicio => Set<CostoServicio>();
    public DbSet<Servicio> Servicios => Set<Servicio>();
    public DbSet<Ubicacion> Ubicaciones => Set<Ubicacion>();
    public DbSet<Gasto> Gastos => Set<Gasto>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Un producto es único por (Cliente + Código)
        builder.Entity<Producto>()
            .HasIndex(p => new { p.ClienteId, p.Codigo })
            .IsUnique();

        // Código de ubicación único
        builder.Entity<Ubicacion>()
            .HasIndex(u => u.Codigo)
            .IsUnique();

        // Entrada -> Ubicación: al borrar la ubicación, la entrada queda sin rack asignado
        builder.Entity<Entrada>()
            .HasOne(e => e.Ubicacion)
            .WithMany(u => u.Entradas)
            .HasForeignKey(e => e.UbicacionId)
            .OnDelete(DeleteBehavior.SetNull);

        // Evitar borrados en cascada que disparen ciclos en SQL Server
        builder.Entity<Entrada>()
            .HasOne(e => e.Producto)
            .WithMany()
            .HasForeignKey(e => e.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Salida>()
            .HasOne(s => s.Producto)
            .WithMany()
            .HasForeignKey(s => s.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Salida>()
            .HasOne(s => s.SolicitudPedido)
            .WithMany()
            .HasForeignKey(s => s.SolicitudPedidoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<DetalleSolicitud>()
            .HasOne(d => d.Producto)
            .WithMany()
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Montos con 2 decimales
        builder.Entity<Cargo>().Property(c => c.Monto).HasPrecision(12, 2);
        builder.Entity<Pago>().Property(p => p.Monto).HasPrecision(12, 2);

        builder.Entity<Cargo>()
            .HasOne(c => c.Cliente).WithMany()
            .HasForeignKey(c => c.ClienteId).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Cargo>()
            .HasOne(c => c.SolicitudPedido).WithMany()
            .HasForeignKey(c => c.SolicitudPedidoId).OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Pago>()
            .HasOne(p => p.Cliente).WithMany()
            .HasForeignKey(p => p.ClienteId).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CostoServicio>().Property(c => c.ImporteNeto).HasPrecision(12, 2);

        builder.Entity<CostoServicio>()
            .HasOne(c => c.Cliente).WithMany()
            .HasForeignKey(c => c.ClienteId).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Servicio>().Property(s => s.ImporteNeto).HasPrecision(12, 2);

        builder.Entity<Gasto>().Property(g => g.Monto).HasPrecision(12, 2);

        // Vincular usuario -> cliente
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Cliente)
            .WithMany()
            .HasForeignKey(u => u.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
