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
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager) =>
        {
            // El checkbox solo se envía cuando está marcado; si no viene, es false.
            var result = await signInManager.PasswordSignInAsync(
                email, password, rememberMe ?? false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Si la contraseña es temporal, obligar a cambiarla antes de continuar.
                var user = await userManager.FindByEmailAsync(email);
                if (user is not null && user.RequiereCambioPassword)
                    return Results.LocalRedirect("/Account/CambiarPassword");

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

        // Cambiar contraseña (primer ingreso con temporal o cambio voluntario)
        group.MapPost("/CambiarPassword", async (
            [FromForm] string actual,
            [FromForm] string password,
            [FromForm] string confirmar,
            HttpContext http,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager) =>
        {
            var user = await userManager.GetUserAsync(http.User);
            if (user is null)
                return Results.LocalRedirect("/Account/Login");

            if (string.IsNullOrWhiteSpace(password) || password != confirmar)
                return Results.Redirect("/Account/CambiarPassword?error=match");

            // Verifica la contraseña actual y aplica la nueva.
            var cambio = await userManager.ChangePasswordAsync(user, actual ?? "", password);
            if (!cambio.Succeeded)
            {
                var codigo = cambio.Errors.Any(e => e.Code == "PasswordMismatch") ? "actual" : "policy";
                return Results.Redirect($"/Account/CambiarPassword?error={codigo}");
            }

            user.RequiereCambioPassword = false;
            await userManager.UpdateAsync(user);

            // Refrescar la sesión para que el claim de contraseña temporal desaparezca.
            await signInManager.RefreshSignInAsync(user);
            return Results.LocalRedirect("/");
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
