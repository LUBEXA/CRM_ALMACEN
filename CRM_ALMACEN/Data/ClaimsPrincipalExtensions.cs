using System.Security.Claims;

namespace CRM_ALMACEN.Data;

public static class ClaimsPrincipalExtensions
{
    public static bool EsAdmin(this ClaimsPrincipal user) =>
        user.IsInRole(DbSeeder.RolAdmin);

    /// <summary>
    /// True si el usuario es personal del almacén (Admin o cualquier rol interno),
    /// es decir, NO representa a un cliente. El personal ve todos los clientes;
    /// el cliente solo lo suyo.
    /// </summary>
    public static bool EsPersonal(this ClaimsPrincipal user) =>
        (user.Identity?.IsAuthenticated ?? false) && user.ClienteId() is null;

    /// <summary>Id del cliente al que pertenece el usuario, o null si es Admin.</summary>
    public static int? ClienteId(this ClaimsPrincipal user)
    {
        var valor = user.FindFirstValue(AdditionalUserClaimsPrincipalFactory.ClienteIdClaim);
        return int.TryParse(valor, out var id) ? id : null;
    }

    /// <summary>
    /// True si el usuario puede gestionar pedidos (mover en el tablero, surtir, etc.).
    /// El Supervisor y Cobranza solo consultan.
    /// </summary>
    public static bool PuedeGestionarPedidos(this ClaimsPrincipal user) =>
        user.PuedeEditarModulo(ModuloApp.Solicitudes);

    /// <summary>True si el usuario debe cambiar su contraseña temporal.</summary>
    public static bool RequiereCambioPassword(this ClaimsPrincipal user) =>
        user.HasClaim(AdditionalUserClaimsPrincipalFactory.RequiereCambioPasswordClaim, "true");

    public static string NombreParaMostrar(this ClaimsPrincipal user) =>
        user.FindFirstValue("NombreCompleto")
        ?? user.Identity?.Name
        ?? "Usuario";
}
