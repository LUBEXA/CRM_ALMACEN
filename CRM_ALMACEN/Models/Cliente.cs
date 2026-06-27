using System.ComponentModel.DataAnnotations;

namespace CRM_ALMACEN.Models;

/// <summary>
/// Empresa o persona que almacena su mercancía en el almacén.
/// Cada usuario "Cliente" pertenece a un Cliente y solo ve su propia información.
/// </summary>
public class Cliente
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Rfc { get; set; }

    [MaxLength(150)]
    [EmailAddress]
    public string? Correo { get; set; }

    [MaxLength(30)]
    public string? Telefono { get; set; }

    [MaxLength(250)]
    public string? Direccion { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    // Relaciones
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    public ICollection<SolicitudPedido> Solicitudes { get; set; } = new List<SolicitudPedido>();
}
