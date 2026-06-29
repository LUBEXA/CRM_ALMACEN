namespace CRM_ALMACEN.Models;

/// <summary>
/// Corte de facturación de un cliente en un periodo (mes/año).
/// El administrador establece la fecha de corte y, cuando todo está revisado,
/// "libera" el estado de cuenta para que el cliente pueda verlo en su plataforma.
/// Mientras no esté liberado, el cliente no ve los cargos del periodo.
/// </summary>
public class CorteFacturacion
{
    public int Id { get; set; }

    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public int Anio { get; set; } = DateTime.Now.Year;
    public int Mes { get; set; } = DateTime.Now.Month;

    /// <summary>Fecha de corte: hasta cuándo se acumulan los cobros del periodo.</summary>
    public DateTime? FechaCorte { get; set; }

    /// <summary>True cuando el administrador libera el estado de cuenta al cliente.</summary>
    public bool Liberado { get; set; }

    /// <summary>Momento en que se liberó al cliente.</summary>
    public DateTime? LiberadoEn { get; set; }

    /// <summary>Usuario que liberó el estado de cuenta.</summary>
    public string? LiberadoPor { get; set; }
}
