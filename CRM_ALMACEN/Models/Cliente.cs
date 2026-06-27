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

    [MaxLength(150)]
    public string? Representante { get; set; }

    [MaxLength(20)]
    public string? Rfc { get; set; }

    [MaxLength(150)]
    [EmailAddress]
    public string? Correo { get; set; }

    [MaxLength(150)]
    [EmailAddress]
    public string? Correo2 { get; set; }

    [MaxLength(30)]
    public string? Telefono { get; set; }

    [MaxLength(30)]
    public string? Telefono2 { get; set; }

    [MaxLength(250)]
    public string? Direccion { get; set; }

    /// <summary>El cliente requiere factura CFDI de todos sus pagos.</summary>
    public bool RequiereFactura { get; set; }

    /// <summary>Nombre del archivo del logotipo (guardado en wwwroot/uploads/clientes).</summary>
    [MaxLength(260)]
    public string? LogoArchivo { get; set; }

    /// <summary>Nombre del archivo de la constancia de situación fiscal (PDF).</summary>
    [MaxLength(260)]
    public string? ConstanciaFiscalArchivo { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    // Relaciones
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
    public ICollection<SolicitudPedido> Solicitudes { get; set; } = new List<SolicitudPedido>();
}
