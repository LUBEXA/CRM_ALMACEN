using System.ComponentModel.DataAnnotations;

namespace CRM_ALMACEN.Models;

/// <summary>Tipo de renglón en el estado de cuenta.</summary>
public enum TipoCargo
{
    Servicio = 0,
    Descuento = 1,
    Ajuste = 2,
    CobroPendiente = 3
}

/// <summary>
/// Un concepto cobrado a un cliente en un periodo (mes/año).
/// Ej.: "Renta de tarima mediana", cantidad 15, precio unitario $533.60.
/// El importe del renglón es Cantidad × PrecioUnitario.
/// </summary>
public class Cargo
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    /// <summary>Solicitud de pedido que originó este cobro, si aplica.</summary>
    public int? SolicitudPedidoId { get; set; }
    public SolicitudPedido? SolicitudPedido { get; set; }

    public int Anio { get; set; } = DateTime.Now.Year;

    [Range(1, 12)]
    public int Mes { get; set; } = DateTime.Now.Month;

    [Required, MaxLength(150)]
    public string Concepto { get; set; } = string.Empty;

    public TipoCargo Tipo { get; set; } = TipoCargo.Servicio;

    public int Cantidad { get; set; } = 1;

    /// <summary>Precio por unidad del concepto.</summary>
    public decimal PrecioUnitario { get; set; }

    /// <summary>Importe del renglón (Cantidad × PrecioUnitario; negativo en descuentos).</summary>
    public decimal Monto { get; set; }

    public bool Pagado { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    /// <summary>Nombre del mes en español para mostrar.</summary>
    public string PeriodoTexto => $"{NombresMes.Get(Mes)} {Anio}";

    /// <summary>Texto del tipo de renglón para mostrar.</summary>
    public string TipoTexto => Tipo switch
    {
        TipoCargo.Descuento => "Descuento",
        TipoCargo.Ajuste => "Ajuste",
        TipoCargo.CobroPendiente => "Cobro pendiente",
        _ => "Servicio"
    };
}

/// <summary>Nombres de mes en español sin depender de la cultura del servidor.</summary>
public static class NombresMes
{
    private static readonly string[] Meses =
    [
        "", "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
        "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
    ];

    public static string Get(int mes) => mes is >= 1 and <= 12 ? Meses[mes] : "";
}
