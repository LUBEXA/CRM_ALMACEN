using System.ComponentModel.DataAnnotations;

namespace CRM_ALMACEN.Models;

/// <summary>Forma en que el cliente realizó un pago.</summary>
public enum MetodoPago
{
    [Display(Name = "Transferencia interbancaria")]
    Transferencia = 0,
    Efectivo = 1,
    Tarjeta = 2,
    Cheque = 3,
    Otro = 4
}

/// <summary>
/// Un pago realizado por un cliente, con su comprobante/factura CFDI (XML y PDF).
/// </summary>
public class Pago
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    /// <summary>Periodo (mes/año) al que se aplica el pago, para calcular el estado de cuenta.</summary>
    public int Anio { get; set; } = DateTime.Now.Year;

    [Range(1, 12)]
    public int Mes { get; set; } = DateTime.Now.Month;

    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
    public decimal Monto { get; set; }

    public MetodoPago MetodoPago { get; set; } = MetodoPago.Transferencia;

    [MaxLength(120)]
    public string? Referencia { get; set; }

    /// <summary>Archivo XML de la factura CFDI (ruta en wwwroot/uploads).</summary>
    [MaxLength(260)]
    public string? XmlArchivo { get; set; }

    /// <summary>Archivo PDF de la factura CFDI (ruta en wwwroot/uploads).</summary>
    [MaxLength(260)]
    public string? PdfArchivo { get; set; }

    [MaxLength(150)]
    public string? RegistradoPor { get; set; }

    /// <summary>Nombre del mes/año del periodo en español para mostrar.</summary>
    public string PeriodoTexto => $"{NombresMes.Get(Mes)} {Anio}";

    /// <summary>Texto del método de pago para mostrar.</summary>
    public string MetodoTexto => MetodoPago switch
    {
        MetodoPago.Transferencia => "Transferencia interbancaria",
        MetodoPago.Efectivo => "Efectivo",
        MetodoPago.Tarjeta => "Tarjeta",
        MetodoPago.Cheque => "Cheque",
        _ => "Otro"
    };
}
