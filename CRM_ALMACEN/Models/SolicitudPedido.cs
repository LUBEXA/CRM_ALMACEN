using System.ComponentModel.DataAnnotations;

namespace CRM_ALMACEN.Models;

/// <summary>Estados por los que pasa una solicitud de pedido.</summary>
public enum EstadoSolicitud
{
    Pendiente = 0,
    Aprobada = 1,
    Surtida = 2,
    Rechazada = 3
}

/// <summary>
/// Solicitud de pedido que hace un cliente para que el almacén le surta
/// (saque) cierta mercancía. Al surtirse genera Salidas.
/// </summary>
public class SolicitudPedido
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    public EstadoSolicitud Estado { get; set; } = EstadoSolicitud.Pendiente;

    [MaxLength(300)]
    public string? Notas { get; set; }

    /// <summary>Usuario (correo) que creó la solicitud.</summary>
    [MaxLength(150)]
    public string? SolicitadoPor { get; set; }

    public DateTime? FechaAtencion { get; set; }

    [MaxLength(150)]
    public string? AtendidoPor { get; set; }

    public ICollection<DetalleSolicitud> Detalles { get; set; } = new List<DetalleSolicitud>();
}

/// <summary>Renglón de una solicitud: un producto y la cantidad pedida.</summary>
public class DetalleSolicitud
{
    public int Id { get; set; }

    public int SolicitudPedidoId { get; set; }
    public SolicitudPedido? SolicitudPedido { get; set; }

    public int ProductoId { get; set; }
    public Producto? Producto { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
    public int Cantidad { get; set; }
}
