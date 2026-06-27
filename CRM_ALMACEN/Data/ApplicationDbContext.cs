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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Un producto es único por (Cliente + Código)
        builder.Entity<Producto>()
            .HasIndex(p => new { p.ClienteId, p.Codigo })
            .IsUnique();

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

        // Vincular usuario -> cliente
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Cliente)
            .WithMany()
            .HasForeignKey(u => u.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
