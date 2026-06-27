using CRM_ALMACEN.Models;
using Microsoft.AspNetCore.Identity;

namespace CRM_ALMACEN.Data;

/// <summary>
/// Usuario que inicia sesión. Extiende el usuario de ASP.NET Identity
/// para agregar el nombre a mostrar y, si es usuario de tipo Cliente,
/// el Cliente al que pertenece.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string? NombreCompleto { get; set; }

    /// <summary>
    /// Null cuando es personal del almacén (Admin).
    /// Tiene valor cuando el usuario representa a un Cliente.
    /// </summary>
    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }
}
