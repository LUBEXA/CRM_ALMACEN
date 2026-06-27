using System.ComponentModel.DataAnnotations;

namespace CRM_ALMACEN.Models;

/// <summary>
/// Servicio del catálogo general (lista de precios). El administrador lo define
/// una sola vez con su precio sugerido; luego se asigna a los clientes desde la
/// pestaña "Costos del servicio", donde el importe puede ajustarse por cliente.
/// </summary>
public class Servicio
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Nombre { get; set; } = string.Empty;

    /// <summary>Precio sugerido (neto, sin impuestos) que se usa al asignarlo a un cliente.</summary>
    public decimal ImporteNeto { get; set; }

    /// <summary>Servicios inactivos no aparecen al asignar a clientes, pero se conservan.</summary>
    public bool Activo { get; set; } = true;
}
