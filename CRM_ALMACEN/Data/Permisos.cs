using System.Security.Claims;

namespace CRM_ALMACEN.Data;

/// <summary>Cada "espacio" (pantalla) del sistema cuyo acceso se controla por usuario.</summary>
public enum ModuloApp
{
    Inventario,
    Entradas,
    Salidas,
    Solicitudes,
    Pagos,
    Clientes,
    Productos,
    ListaPrecios,
    Gastos
}

/// <summary>Nivel de acceso a un módulo. El orden importa: mayor número = más permiso.</summary>
public enum NivelAcceso
{
    SinAcceso = 0,
    SoloVer = 1,
    Editar = 2
}

/// <summary>
/// Permisos por usuario. Se guardan como "claims" de la cuenta (tabla de Identity),
/// así viajan en la sesión sin consultar la BD en cada pantalla. Los roles funcionan
/// como plantillas que llenan estos permisos, pero lo que manda es el permiso guardado.
/// </summary>
public static class Permisos
{
    public const string PrefijoClaim = "perm:";

    public static string ClaimType(ModuloApp m) => PrefijoClaim + m;

    /// <summary>Módulos con su nombre legible (para la tabla del editor y el menú).</summary>
    public static readonly IReadOnlyList<(ModuloApp Modulo, string Nombre)> Modulos =
    [
        (ModuloApp.Inventario, "Inventario"),
        (ModuloApp.Entradas, "Entradas"),
        (ModuloApp.Salidas, "Salidas"),
        (ModuloApp.Solicitudes, "Solicitudes de pedido"),
        (ModuloApp.Pagos, "Pagos y Facturación"),
        (ModuloApp.Clientes, "Clientes"),
        (ModuloApp.Productos, "Productos"),
        (ModuloApp.ListaPrecios, "Lista de precios"),
        (ModuloApp.Gastos, "Gastos")
    ];

    public static string NombreNivel(NivelAcceso n) => n switch
    {
        NivelAcceso.Editar => "Editar",
        NivelAcceso.SoloVer => "Solo ver",
        _ => "Sin acceso"
    };

    private static Dictionary<ModuloApp, NivelAcceso> Todos(NivelAcceso n) =>
        Modulos.ToDictionary(x => x.Modulo, _ => n);

    /// <summary>Plantilla de permisos sugerida para un rol (preset que llena las palomitas).</summary>
    public static IReadOnlyDictionary<ModuloApp, NivelAcceso> Plantilla(string rol)
    {
        var d = Todos(NivelAcceso.SinAcceso);
        switch (rol)
        {
            case DbSeeder.RolAdmin:
                return Todos(NivelAcceso.Editar);

            case DbSeeder.RolAlmacenista:
                d[ModuloApp.Inventario] = NivelAcceso.Editar;
                d[ModuloApp.Entradas] = NivelAcceso.Editar;
                d[ModuloApp.Salidas] = NivelAcceso.Editar;
                d[ModuloApp.Productos] = NivelAcceso.Editar;
                d[ModuloApp.Solicitudes] = NivelAcceso.Editar;
                break;

            case DbSeeder.RolCobranza:
                d[ModuloApp.Pagos] = NivelAcceso.Editar;
                d[ModuloApp.ListaPrecios] = NivelAcceso.Editar;
                d[ModuloApp.Gastos] = NivelAcceso.Editar;
                d[ModuloApp.Clientes] = NivelAcceso.SoloVer;
                break;

            case DbSeeder.RolAtencion:
                d[ModuloApp.Clientes] = NivelAcceso.Editar;
                d[ModuloApp.Solicitudes] = NivelAcceso.Editar;
                d[ModuloApp.Inventario] = NivelAcceso.SoloVer;
                break;

            case DbSeeder.RolSupervisor:
                return Todos(NivelAcceso.SoloVer);
        }
        return d;
    }

    /// <summary>Permisos fijos del usuario tipo Cliente (ve lo suyo; no se personaliza).</summary>
    public static readonly IReadOnlyDictionary<ModuloApp, NivelAcceso> PlantillaCliente =
        new Dictionary<ModuloApp, NivelAcceso>
        {
            [ModuloApp.Inventario] = NivelAcceso.SoloVer,
            [ModuloApp.Solicitudes] = NivelAcceso.Editar,
            [ModuloApp.Pagos] = NivelAcceso.SoloVer
        };

    /// <summary>
    /// Nivel efectivo a partir de los datos crudos (para el editor, que trabaja con el
    /// usuario en BD, no con la sesión). Si ya hay permisos personalizados guardados,
    /// manda eso; si no, usa la plantilla del rol interno del usuario.
    /// </summary>
    public static NivelAcceso NivelEfectivo(
        ModuloApp modulo, IEnumerable<Claim> claims, IEnumerable<string> roles)
    {
        if (roles.Contains(DbSeeder.RolAdmin)) return NivelAcceso.Editar;

        var lista = claims.ToList();
        if (lista.Any(c => c.Type.StartsWith(PrefijoClaim)))
        {
            var val = lista.FirstOrDefault(c => c.Type == ClaimType(modulo))?.Value;
            return int.TryParse(val, out var i) && Enum.IsDefined(typeof(NivelAcceso), i)
                ? (NivelAcceso)i : NivelAcceso.SinAcceso;
        }

        foreach (var (rol, _) in DbSeeder.RolesInternos)
            if (roles.Contains(rol))
                return Plantilla(rol).GetValueOrDefault(modulo, NivelAcceso.SinAcceso);

        return NivelAcceso.SinAcceso;
    }
}

/// <summary>Lectura de permisos desde la sesión del usuario (ClaimsPrincipal).</summary>
public static class PermisosClaimsExtensions
{
    public static NivelAcceso NivelDe(this ClaimsPrincipal user, ModuloApp m)
    {
        if (user.EsAdmin()) return NivelAcceso.Editar;

        // Cliente: permisos fijos de su tipo.
        if (user.ClienteId() is not null)
            return Permisos.PlantillaCliente.GetValueOrDefault(m, NivelAcceso.SinAcceso);

        // Personal: usa sus permisos personalizados; si no tiene, la plantilla de su rol.
        var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value);
        return Permisos.NivelEfectivo(m, user.Claims, roles);
    }

    public static bool PuedeVer(this ClaimsPrincipal user, ModuloApp m) =>
        user.NivelDe(m) >= NivelAcceso.SoloVer;

    public static bool PuedeEditarModulo(this ClaimsPrincipal user, ModuloApp m) =>
        user.NivelDe(m) == NivelAcceso.Editar;
}
