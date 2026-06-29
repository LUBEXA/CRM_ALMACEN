using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM_ALMACEN.Models;

/// <summary>
/// Posición física del almacén (pasillo, rack, nivel...) con un cupo definido.
/// Es el catálogo que permite saber cuánto espacio hay y cuánto está ocupado.
/// Un producto se asigna a una ubicación (Producto.UbicacionId).
/// </summary>
public class Ubicacion
{
    public int Id { get; set; }

    /// <summary>Agrupador grande: pasillo, área o zona. Ej. "Pasillo 3".</summary>
    [MaxLength(60)]
    public string? Zona { get; set; }

    /// <summary>Letra del rack (o cara del rack). Ej. "A".</summary>
    [MaxLength(4)]
    public string? Rack { get; set; }

    /// <summary>Nivel o altura dentro del rack. 1 = piso.</summary>
    public int? Nivel { get; set; }

    /// <summary>Posición (hueco) a lo largo del nivel.</summary>
    public int? Posicion { get; set; }

    /// <summary>Identificador corto de la posición. Ej. "A-2-05". Se arma desde Rack/Nivel/Posición.</summary>
    [Required, MaxLength(40)]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>Cuántos productos/tarimas caben en esta posición.</summary>
    public int Capacidad { get; set; } = 1;

    [MaxLength(200)]
    public string? Notas { get; set; }

    public bool Activo { get; set; } = true;

    /// <summary>Entradas de mercancía colocadas en esta ubicación.</summary>
    public ICollection<Entrada> Entradas { get; set; } = new List<Entrada>();

    /// <summary>Etiqueta legible para mostrar en listas y desplegables.</summary>
    [NotMapped]
    public string Etiqueta =>
        string.IsNullOrWhiteSpace(Zona) ? Codigo : $"{Zona} · {Codigo}";

    /// <summary>
    /// Arma el código estándar a partir de rack, nivel y posición. Ej. "A-2-05".
    /// La posición se rellena a 2 dígitos para que ordene parejo (01, 02, … 10).
    /// </summary>
    public static string ArmarCodigo(string? rack, int nivel, int posicion) =>
        $"{(rack ?? "").Trim().ToUpperInvariant()}-{nivel}-{posicion:00}";
}
