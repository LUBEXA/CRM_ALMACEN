using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM_ALMACEN.Models;

/// <summary>
/// Artículo/SKU que un cliente guarda en el almacén.
/// El "stock" (existencia actual) se mantiene en CantidadDisponible y
/// se actualiza con cada Entrada (suma) y cada Salida (resta).
/// </summary>
public class Producto
{
    public int Id { get; set; }

    /// <summary>Cliente dueño del producto.</summary>
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    [Required, MaxLength(50)]
    public string Codigo { get; set; } = string.Empty;   // SKU / código interno

    [Required, MaxLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Descripcion { get; set; }

    [MaxLength(20)]
    public string UnidadMedida { get; set; } = "PZA";    // PZA, CAJA, KG, etc.

    /// <summary>Existencia actual en almacén.</summary>
    public int CantidadDisponible { get; set; }

    /// <summary>Cantidad mínima antes de alertar reabastecimiento.</summary>
    public int StockMinimo { get; set; }

    public bool Activo { get; set; } = true;

    [NotMapped]
    public bool BajoStock => CantidadDisponible <= StockMinimo;
}
