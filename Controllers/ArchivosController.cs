using GenbaStudio.Models.Entities;
using GenbaStudio.Models.Enums;
using GenbaStudio.Repositories;
using GenbaStudio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace GenbaStudio.Controllers;

[Authorize(Roles = Roles.Director + "," + Roles.Animador)]
public class ArchivosController : Controller
{
    private readonly IEscenaRepository _escenas;
    private readonly IAsignacionRepository _asignaciones;
    private readonly IEntregaRepository _entregas;
    private readonly IFileService _files;

    public ArchivosController(IEscenaRepository escenas, IAsignacionRepository asignaciones,
        IEntregaRepository entregas, IFileService files)
    {
        _escenas = escenas;
        _asignaciones = asignaciones;
        _entregas = entregas;
        _files = files;
    }

    [HttpGet]
    public async Task<IActionResult> Storyboard(int id)
    {
        var usuarioId = User.GetUserId();

        Escena? escena = null;
        if (User.IsInRole(Roles.Director))
            escena = await _escenas.GetDelDirectorAsync(id, usuarioId);
        else if (await _asignaciones.AnimadorTieneEscenaAsync(usuarioId, id))
            escena = await _escenas.GetByIdAsync(id);

        if (escena?.RutaStoryboard == null) return NotFound();

        var stream = _files.AbrirPrivado(escena.RutaStoryboard);
        if (stream == null) return NotFound();

        new FileExtensionContentTypeProvider().TryGetContentType(escena.RutaStoryboard, out var tipo);
        return File(stream, tipo ?? "application/octet-stream");
    }

    [HttpGet]
    public async Task<IActionResult> Entrega(int id)
    {
        var usuarioId = User.GetUserId();

        var entrega = await _entregas.GetConAsignacionAsync(id);
        if (entrega == null) return NotFound();

        var esDirectorDueno = User.IsInRole(Roles.Director)
                              && entrega.Asignacion.Escena.Proyecto.DirectorId == usuarioId;
        var esAnimadorAsignado = User.IsInRole(Roles.Animador)
                                 && entrega.Asignacion.AnimadorId == usuarioId;
        if (!esDirectorDueno && !esAnimadorAsignado) return NotFound();

        var stream = _files.AbrirPrivado(entrega.RutaArchivo);
        if (stream == null) return NotFound();

        new FileExtensionContentTypeProvider().TryGetContentType(entrega.RutaArchivo, out var tipo);
        return File(stream, tipo ?? "application/octet-stream", entrega.NombreOriginal);
    }
}
