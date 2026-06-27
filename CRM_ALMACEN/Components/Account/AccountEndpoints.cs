using CRM_ALMACEN.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CRM_ALMACEN.Components.Account;

/// <summary>
/// Endpoints HTTP de cuenta (iniciar y cerrar sesión). Se hacen como endpoints
/// normales para que la redirección use 302 directo, sin generar NavigationException.
/// </summary>
internal static class AccountEndpoints
{
    public static IEndpointConventionBuilder MapAccountEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/Account");

        // Iniciar sesión
        group.MapPost("/Login", async (
            [FromForm] string email,
            [FromForm] string password,
            [FromForm] bool? rememberMe,
            [FromForm] string? returnUrl,
            SignInManager<ApplicationUser> signInManager) =>
        {
            // El checkbox solo se envía cuando está marcado; si no viene, es false.
            var result = await signInManager.PasswordSignInAsync(
                email, password, rememberMe ?? false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var destino = string.IsNullOrEmpty(returnUrl) || !returnUrl.StartsWith('/')
                    ? "/"
                    : returnUrl;
                return Results.LocalRedirect(destino);
            }

            // Login fallido: regresar a la pantalla de login mostrando el error.
            var volver = "/Account/Login?error=1";
            if (!string.IsNullOrEmpty(returnUrl))
                volver += $"&returnUrl={Uri.EscapeDataString(returnUrl)}";
            return Results.Redirect(volver);
        });

        // Cerrar sesión
        group.MapPost("/Logout", async (SignInManager<ApplicationUser> signInManager) =>
        {
            await signInManager.SignOutAsync();
            return Results.LocalRedirect("/Account/Login");
        });

        return group;
    }
}
