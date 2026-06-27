using System.Security.Claims;

namespace CRM_ALMACEN.Data;

public static class ClaimsPrincipalExtensions
{
    public static bool EsAdmin(this ClaimsPrincipal user) =>
        user.IsInRole(DbSeeder.RolAdmin);

    /// <summary>Id del cliente al que pertenece el usuario, o null si es Admin.</summary>
    public static int? ClienteId(this ClaimsPrincipal user)
    {
        var valor = user.FindFirstValue(AdditionalUserClaimsPrincipalFactory.ClienteIdClaim);
        return int.TryParse(valor, out var id) ? id : null;
    }

    /// <summary>True si el usuario debe cambiar su contraseña temporal.</summary>
    public static bool RequiereCambioPassword(this ClaimsPrincipal user) =>
        user.HasClaim(AdditionalUserClaimsPrincipalFactory.RequiereCambioPasswordClaim, "true");

    public static string NombreParaMostrar(this ClaimsPrincipal user) =>
        user.FindFirstValue("NombreCompleto")
        ?? user.Identity?.Name
        ?? "Usuario";
}
