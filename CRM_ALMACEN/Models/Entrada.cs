using System.ComponentModel.DataAnnotations;

namespace CRM_ALMACEN.Models;

/// <summary>
/// Movimiento de ENTRADA: mercancía que llega al almacén y suma existencia.
/// </summary>
public class Entrada
{
    public int Id { get; set; }

    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    public int Cantidad { get; set; }

    /// <summary>Rack/posición del almacén donde se colocó esta mercancía.</summary>
    public int? UbicacionId { get; set; }
    public Ubicacion? Ubicacion { get; set; }

    /// <summary>Cuántas posiciones/tarimas ocupa esta entrada en el rack (mín. 1).</summary>
    public int Posiciones { get; set; } = 1;

    public DateTime Fecha { get; set; } = DateTime.Now;

    [MaxLength(80)]
    public string? Referencia { get; set; }   // # de remisión, factura, orden, etc.

    [MaxLength(250)]
    public string? Observaciones { get; set; }

    /// <summary>Usuario (correo) que registró el movimiento.</summary>
    [MaxLength(150)]
    public string? RegistradoPor { get; set; }
}
