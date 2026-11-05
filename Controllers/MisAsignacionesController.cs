using GenbaStudio.Models.Entities;
using GenbaStudio.Models.Enums;
using GenbaStudio.Models.ViewModels;
using GenbaStudio.Repositories;
using GenbaStudio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GenbaStudio.Controllers;

[Authorize(Roles = Roles.Animador)]
public class MisAsignacionesController : Controller
{
    private const int PageSize = 10;

    private const long LimiteRequest = 110L * 1024 * 1024;

    private readonly IAsignacionRepository _asignaciones;
    private readonly IEntregaRepository _entregas;
    private readonly IEscenaRepository _escenas;
    private readonly IFileService _files;

    public MisAsignacionesController(IAsignacionRepository asignaciones, IEntregaRepository entregas,
        IEscenaRepository escenas, IFileService files)
    {
        _asignaciones = asignaciones;
        _entregas = entregas;
        _escenas = escenas;
        _files = files;
    }

    [HttpGet]
    public async Task<IActionResult> Index(EstadoAsignacion? estado, int page = 1)
    {
        var resultado = await _asignaciones.GetPaginaAsync(null, User.GetUserId(), null, estado, page, PageSize);
        ViewData["estado"] = estado;
        return View(resultado);
    }

    [HttpGet]
    public async Task<IActionResult> Detalle(int id)
    {
        var animadorId = User.GetUserId();

        var dto = await _asignaciones.GetDtoAsync(id, null, animadorId);
        if (dto == null) return NotFound();

        var escena = await _escenas.GetByIdAsync(dto.EscenaId);

        var vm = new DetalleAnimadorViewModel
        {
            Asignacion = dto,
            Historial = await _entregas.GetHistorialAsync(id),
            TieneStoryboard = escena?.RutaStoryboard != null
        };
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Iniciar(int id)
    {
        var asignacion = await _asignaciones.GetDelAnimadorAsync(id, User.GetUserId());
        if (asignacion == null) return NotFound();

        if (asignacion.Estado != EstadoAsignacion.Pendiente && asignacion.Estado != EstadoAsignacion.ConCorrecciones)
        {
            TempData["Error"] = "Desde el estado actual no se puede iniciar el trabajo.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        asignacion.Estado = EstadoAsignacion.EnProgreso;
        await _asignaciones.UpdateAsync(asignacion);

        TempData["Ok"] = "Asignación en progreso.";
        return RedirectToAction(nameof(Detalle), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequestSizeLimit(LimiteRequest)]
    [RequestFormLimits(MultipartBodyLengthLimit = LimiteRequest)]
    public async Task<IActionResult> SubirEntrega(int id, IFormFile? archivo, string? comentario)
    {
        var asignacion = await _asignaciones.GetDelAnimadorAsync(id, User.GetUserId());
        if (asignacion == null) return NotFound();

        if (asignacion.Estado != EstadoAsignacion.EnProgreso && asignacion.Estado != EstadoAsignacion.ConCorrecciones)
        {
            TempData["Error"] = "Solo se puede subir una entrega cuando la asignación está En progreso o Con correcciones.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        if (archivo == null || archivo.Length == 0)
        {
            TempData["Error"] = "Elegí un archivo para subir.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        comentario = string.IsNullOrWhiteSpace(comentario) ? null : comentario.Trim();
        if (comentario != null && comentario.Length > 1000)
        {
            TempData["Error"] = "El comentario no puede superar los 1000 caracteres.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        string ruta;
        try
        {
            ruta = await _files.GuardarPrivadoAsync(archivo, "entregas", FileRules.Entrega);
        }
        catch (ArchivoInvalidoException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Detalle), new { id });
        }

        var nombreOriginal = Path.GetFileName(archivo.FileName);
        if (nombreOriginal.Length > 255) nombreOriginal = nombreOriginal[^255..];

        var entrega = new Entrega
        {
            AsignacionId = id,
            RutaArchivo = ruta,
            NombreOriginal = nombreOriginal,
            ComentarioAnimador = comentario,
            FechaSubida = DateTime.Now,
            EstadoRevision = EstadoRevision.Pendiente
        };

        try
        {
            await _entregas.AgregarConVersionAsync(entrega);
        }
        catch (DbUpdateException)
        {
            _files.EliminarPrivado(ruta);
            TempData["Error"] = "No se pudo registrar la entrega (puede haber otra subida en curso). Intentá de nuevo.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        TempData["Ok"] = $"Entrega subida (versión {entrega.Version}). Quedó en revisión.";
        return RedirectToAction(nameof(Detalle), new { id });
    }
}
