using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace CRM_ALMACEN.Data;

/// <summary>
/// Agrega a la sesión del usuario claims extra: el Id del Cliente al que pertenece
/// y su nombre para mostrar. Así las pantallas filtran sin consultar la BD cada vez.
/// </summary>
public class AdditionalUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> options)
    : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>(userManager, roleManager, options)
{
    public const string ClienteIdClaim = "ClienteId";

    public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
    {
        var principal = await base.CreateAsync(user);
        var identity = (ClaimsIdentity)principal.Identity!;

        if (user.ClienteId is int clienteId)
            identity.AddClaim(new Claim(ClienteIdClaim, clienteId.ToString()));

        if (!string.IsNullOrWhiteSpace(user.NombreCompleto))
            identity.AddClaim(new Claim("NombreCompleto", user.NombreCompleto));

        return principal;
    }
}
