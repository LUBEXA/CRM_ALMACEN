using System.ComponentModel.DataAnnotations;

namespace CRM_ALMACEN.Models;

/// <summary>Categoría del gasto, para agrupar en los reportes de finanzas.</summary>
public enum CategoriaGasto
{
    Renta = 0,
    Sueldos = 1,
    [Display(Name = "Servicios (luz, agua, internet)")]
    Servicios = 2,
    Mantenimiento = 3,
    Transporte = 4,
    Insumos = 5,
    Impuestos = 6,
    Otro = 7
}

/// <summary>
/// Un gasto del negocio en un periodo (mes/año). Sirve para calcular la utilidad
/// neta en Finanzas: ingresos cobrados − gastos.
/// </summary>
public class Gasto
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    /// <summary>Periodo (mes/año) al que se imputa el gasto.</summary>
    public int Anio { get; set; } = DateTime.Now.Year;

    [Range(1, 12)]
    public int Mes { get; set; } = DateTime.Now.Month;

    [Required, MaxLength(150)]
    public string Concepto { get; set; } = string.Empty;

    public CategoriaGasto Categoria { get; set; } = CategoriaGasto.Otro;

    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a cero.")]
    public decimal Monto { get; set; }

    [MaxLength(300)]
    public string? Nota { get; set; }

    [MaxLength(150)]
    public string? RegistradoPor { get; set; }

    /// <summary>Nombre del mes/año del periodo en español para mostrar.</summary>
    public string PeriodoTexto => $"{NombresMes.Get(Mes)} {Anio}";

    /// <summary>Texto de la categoría para mostrar.</summary>
    public string CategoriaTexto => CategoriaNombre(Categoria);

    public static string CategoriaNombre(CategoriaGasto c) => c switch
    {
        CategoriaGasto.Renta => "Renta",
        CategoriaGasto.Sueldos => "Sueldos",
        CategoriaGasto.Servicios => "Servicios (luz, agua, internet)",
        CategoriaGasto.Mantenimiento => "Mantenimiento",
        CategoriaGasto.Transporte => "Transporte",
        CategoriaGasto.Insumos => "Insumos",
        CategoriaGasto.Impuestos => "Impuestos",
        _ => "Otro"
    };
}
