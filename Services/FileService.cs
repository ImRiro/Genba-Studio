using Microsoft.AspNetCore.Http;

namespace GenbaStudio.Services;

public class ArchivoInvalidoException : Exception
{
    public ArchivoInvalidoException(string mensaje) : base(mensaje) { }
}

public record FileRules(string[] Extensiones, long MaxBytes, bool EsImagen = false)
{
    public static readonly FileRules Imagen =
        new(new[] { ".jpg", ".jpeg", ".png", ".webp" }, 2 * 1024 * 1024, EsImagen: true);

    public static readonly FileRules Storyboard =
        new(new[] { ".jpg", ".jpeg", ".png", ".webp", ".pdf" }, 10 * 1024 * 1024);
}

public interface IFileService
{
    Task<string> GuardarPublicoAsync(IFormFile file, string carpeta, FileRules reglas);

    Task<string> GuardarPrivadoAsync(IFormFile file, string carpeta, FileRules reglas);

    Stream? AbrirPrivado(string? rutaRelativa);
    void EliminarPublico(string? rutaWeb);
    void EliminarPrivado(string? rutaRelativa);
}

public class FileService : IFileService
{
    private const string PrefijoWeb = "/uploads/";
    private readonly string _uploadsRoot;
    private readonly string _privateRoot;

    public FileService(IWebHostEnvironment env)
    {
        _uploadsRoot = Path.Combine(env.WebRootPath, "uploads");
        _privateRoot = Path.Combine(env.ContentRootPath, "App_Data", "private");
    }

    public async Task<string> GuardarPublicoAsync(IFormFile file, string carpeta, FileRules reglas)
    {
        var nombre = await GuardarAsync(file, Path.Combine(_uploadsRoot, carpeta), reglas);
        return $"{PrefijoWeb}{carpeta}/{nombre}";
    }

    public async Task<string> GuardarPrivadoAsync(IFormFile file, string carpeta, FileRules reglas)
    {
        var nombre = await GuardarAsync(file, Path.Combine(_privateRoot, carpeta), reglas);
        return $"{carpeta}/{nombre}";
    }

    public Stream? AbrirPrivado(string? rutaRelativa)
    {
        if (string.IsNullOrWhiteSpace(rutaRelativa)) return null;
        var abs = Resolver(_privateRoot, rutaRelativa);
        if (abs == null || !File.Exists(abs)) return null;
        return new FileStream(abs, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public void EliminarPublico(string? rutaWeb)
    {
        if (string.IsNullOrWhiteSpace(rutaWeb) || !rutaWeb.StartsWith(PrefijoWeb)) return;
        var abs = Resolver(_uploadsRoot, rutaWeb[PrefijoWeb.Length..]);
        if (abs != null && File.Exists(abs)) File.Delete(abs);
    }

    public void EliminarPrivado(string? rutaRelativa)
    {
        if (string.IsNullOrWhiteSpace(rutaRelativa)) return;
        var abs = Resolver(_privateRoot, rutaRelativa);
        if (abs != null && File.Exists(abs)) File.Delete(abs);
    }

    private static async Task<string> GuardarAsync(IFormFile file, string carpetaAbsoluta, FileRules reglas)
    {
        await ValidarAsync(file, reglas);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var nombre = $"{Guid.NewGuid():N}{ext}";

        Directory.CreateDirectory(carpetaAbsoluta);
        await using var fs = new FileStream(Path.Combine(carpetaAbsoluta, nombre), FileMode.CreateNew);
        await file.CopyToAsync(fs);
        return nombre;
    }

    private static async Task ValidarAsync(IFormFile? file, FileRules reglas)
    {
        if (file == null || file.Length == 0)
            throw new ArchivoInvalidoException("No se recibió ningún archivo.");

        if (file.Length > reglas.MaxBytes)
            throw new ArchivoInvalidoException(
                $"El archivo supera el máximo permitido ({reglas.MaxBytes / (1024 * 1024)} MB).");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!reglas.Extensiones.Contains(ext))
            throw new ArchivoInvalidoException(
                $"Formato no permitido. Se aceptan: {string.Join(", ", reglas.Extensiones)}.");

        if (reglas.EsImagen && !await FirmaImagenValidaAsync(file, ext))
            throw new ArchivoInvalidoException("El archivo no es una imagen válida.");
    }

    // Verifica los primeros bytes (firma) para que un .exe renombrado a .png no pase. (Idea no mia, sacada de stackoverflow al investigar y agregada como chiche)
    private static async Task<bool> FirmaImagenValidaAsync(IFormFile file, string ext)
    {
        var h = new byte[12];
        await using var s = file.OpenReadStream();
        var n = await s.ReadAsync(h.AsMemory(0, h.Length));
        if (n < h.Length) return false;

        return ext switch
        {
            ".jpg" or ".jpeg" => h[0] == 0xFF && h[1] == 0xD8 && h[2] == 0xFF,
            ".png" => h[0] == 0x89 && h[1] == 0x50 && h[2] == 0x4E && h[3] == 0x47,
            ".webp" => h[0] == 'R' && h[1] == 'I' && h[2] == 'F' && h[3] == 'F'
                       && h[8] == 'W' && h[9] == 'E' && h[10] == 'B' && h[11] == 'P',
            _ => true
        };
    }

    private static string? Resolver(string raiz, string relativa)
    {
        var rootFull = Path.GetFullPath(raiz).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var full = Path.GetFullPath(Path.Combine(rootFull, relativa));
        return full.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase) ? full : null;
    }
}
