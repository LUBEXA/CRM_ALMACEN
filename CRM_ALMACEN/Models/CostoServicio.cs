using System.ComponentModel.DataAnnotations;

namespace CRM_ALMACEN.Models;

/// <summary>
/// Concepto de costo del servicio que se ofrece a un cliente (tarifario).
/// Es informativo: el administrador lo define y el cliente lo ve en su perfil.
/// </summary>
public class CostoServicio
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    [Required, MaxLength(200)]
    public string Concepto { get; set; } = string.Empty;

    /// <summary>Importe neto del servicio (sin impuestos).</summary>
    public decimal ImporteNeto { get; set; }
}
