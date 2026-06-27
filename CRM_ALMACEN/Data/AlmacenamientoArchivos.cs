using Microsoft.AspNetCore.Components.Forms;

namespace CRM_ALMACEN.Data;

/// <summary>
/// Guarda archivos subidos (logos, constancias) dentro de wwwroot/uploads
/// y devuelve la ruta web relativa para mostrarlos o enlazarlos.
/// </summary>
public class AlmacenamientoArchivos(IWebHostEnvironment env)
{
    private const long MaxBytes = 5 * 1024 * 1024; // 5 MB

    /// <summary>
    /// Guarda el archivo en uploads/{subcarpeta} y regresa la ruta web relativa
    /// (ej. "uploads/clientes/3/logo_abc.png"). Borra el archivo anterior si se indica.
    /// </summary>
    public async Task<string> GuardarAsync(IBrowserFile archivo, string subcarpeta, string prefijo, string? anterior = null)
    {
        var raiz = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var carpetaDestino = Path.Combine(raiz, "uploads", subcarpeta);
        Directory.CreateDirectory(carpetaDestino);

        var extension = Path.GetExtension(archivo.Name);
        var nombre = $"{prefijo}_{Guid.NewGuid():N}{extension}";
        var rutaFisica = Path.Combine(carpetaDestino, nombre);

        await using (var destino = File.Create(rutaFisica))
        await using (var origen = archivo.OpenReadStream(MaxBytes))
        {
            await origen.CopyToAsync(destino);
        }

        // Borrar el archivo anterior si existía
        if (!string.IsNullOrEmpty(anterior))
        {
            var rutaAnterior = Path.Combine(raiz, anterior.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(rutaAnterior))
            {
                try { File.Delete(rutaAnterior); } catch { /* ignorar */ }
            }
        }

        // Ruta web relativa (con / para el navegador)
        return $"uploads/{subcarpeta}/{nombre}";
    }
}
