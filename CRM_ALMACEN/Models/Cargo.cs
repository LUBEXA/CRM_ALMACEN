using System.ComponentModel.DataAnnotations;

namespace CRM_ALMACEN.Models;

/// <summary>
/// Un concepto cobrado a un cliente en un periodo (mes/año).
/// Ej.: "Renta de tarima mediana", cantidad 15, monto $8,004.00.
/// </summary>
public class Cargo
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public int Anio { get; set; } = DateTime.Now.Year;

    [Range(1, 12)]
    public int Mes { get; set; } = DateTime.Now.Month;

    [Required, MaxLength(150)]
    public string Concepto { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Cantidad { get; set; } = 1;

    [Range(0, double.MaxValue)]
    public decimal Monto { get; set; }

    public bool Pagado { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    /// <summary>Nombre del mes en español para mostrar.</summary>
    public string PeriodoTexto => $"{NombresMes.Get(Mes)} {Anio}";
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
