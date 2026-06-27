using System.ComponentModel.DataAnnotations;

namespace CRM_ALMACEN.Models;

/// <summary>
/// Movimiento de SALIDA: mercancía que sale del almacén y resta existencia.
/// </summary>
public class Salida
{
    public int Id { get; set; }

    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    public int Cantidad { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [MaxLength(80)]
    public string? Referencia { get; set; }

    [MaxLength(250)]
    public string? Observaciones { get; set; }

    /// <summary>Si la salida nació de una solicitud de pedido, aquí queda el vínculo.</summary>
    public int? SolicitudPedidoId { get; set; }
    public SolicitudPedido? SolicitudPedido { get; set; }

    [MaxLength(150)]
    public string? RegistradoPor { get; set; }
}
