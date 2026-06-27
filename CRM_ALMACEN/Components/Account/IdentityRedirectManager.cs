using Microsoft.AspNetCore.Components;

namespace CRM_ALMACEN.Components.Account;

/// <summary>Ayuda a redirigir desde componentes con render estático (formularios de login).</summary>
internal sealed class IdentityRedirectManager(NavigationManager navigationManager)
{
    public const string StatusCookieName = "Identity.StatusMessage";

    [DoesNotReturnAttributeShim]
    public void RedirectTo(string? uri)
    {
        // Si no hay destino, ir al inicio.
        if (string.IsNullOrEmpty(uri))
            uri = "/";

        // Evitar URLs absolutas externas (seguridad de redirección abierta)
        if (uri.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            uri.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            uri = "/";

        navigationManager.NavigateTo(uri);
        throw new InvalidOperationException($"No se puede continuar tras la redirección a '{uri}'.");
    }

    public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
    {
        var uriWithoutQuery = navigationManager.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        var newUri = navigationManager.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
        RedirectTo(newUri);
    }
}

/// <summary>Marca métodos que nunca retornan (no usamos el del framework para evitar dependencias).</summary>
[AttributeUsage(AttributeTargets.Method)]
internal sealed class DoesNotReturnAttributeShim : Attribute;
