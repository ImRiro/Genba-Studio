using GenbaStudio.Models.Entities;
using GenbaStudio.Models.Enums;
using GenbaStudio.Models.ViewModels;
using GenbaStudio.Repositories;
using GenbaStudio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace GenbaStudio.Controllers;

[Authorize(Roles = Roles.Director)]
public class EscenasController : Controller
{
    private const int PageSize = 10;

    private readonly IProyectoRepository _proyectos;
    private readonly IEscenaRepository _escenas;
    private readonly IFileService _files;

    public EscenasController(IProyectoRepository proyectos, IEscenaRepository escenas, IFileService files)
    {
        _proyectos = proyectos;
        _escenas = escenas;
        _files = files;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int proyectoId, string? q, int page = 1)
    {
        var proyecto = await _proyectos.GetDelDirectorAsync(proyectoId, User.GetUserId());
        if (proyecto == null) return NotFound();

        var resultado = await _escenas.GetPaginaAsync(proyectoId, q, page, PageSize);

        ViewData["Proyecto"] = proyecto;
        ViewData["q"] = q;
        return View(resultado);
    }

    [HttpGet]
    public async Task<IActionResult> Nueva(int proyectoId)
    {
        var proyecto = await _proyectos.GetDelDirectorAsync(proyectoId, User.GetUserId());
        if (proyecto == null) return NotFound();

        return View("Form", new EscenaFormViewModel
        {
            ProyectoId = proyectoId,
            ProyectoNombre = proyecto.Nombre,
            Numero = await _escenas.SiguienteNumeroAsync(proyectoId)
        });
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var e = await _escenas.GetDelDirectorAsync(id, User.GetUserId());
        if (e == null) return NotFound();

        return View("Form", new EscenaFormViewModel
        {
            Id = e.Id,
            ProyectoId = e.ProyectoId,
            ProyectoNombre = e.Proyecto.Nombre,
            Numero = e.Numero,
            Descripcion = e.Descripcion,
            DuracionSegundos = e.DuracionSegundos,
            Estado = e.Estado,
            TieneStoryboard = e.RutaStoryboard != null
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(EscenaFormViewModel vm)
    {
        var directorId = User.GetUserId();

        var proyecto = await _proyectos.GetDelDirectorAsync(vm.ProyectoId, directorId);
        if (proyecto == null) return NotFound();
        vm.ProyectoNombre = proyecto.Nombre;

        if (await _escenas.ExisteNumeroAsync(vm.ProyectoId, vm.Numero, vm.Id == 0 ? null : vm.Id))
            ModelState.AddModelError(nameof(vm.Numero), "Ya existe una escena con ese número en este proyecto.");

        string? rutaNueva = null;
        if (ModelState.IsValid && vm.Storyboard is { Length: > 0 })
        {
            try
            {
                rutaNueva = await _files.GuardarPrivadoAsync(vm.Storyboard, "storyboards", FileRules.Storyboard);
            }
            catch (ArchivoInvalidoException ex)
            {
                ModelState.AddModelError(nameof(vm.Storyboard), ex.Message);
            }
        }

        if (!ModelState.IsValid) return View("Form", vm);

        if (vm.Id == 0)
        {
            await _escenas.AddAsync(new Escena
            {
                ProyectoId = vm.ProyectoId,
                Numero = vm.Numero,
                Descripcion = string.IsNullOrWhiteSpace(vm.Descripcion) ? null : vm.Descripcion.Trim(),
                DuracionSegundos = vm.DuracionSegundos,
                Estado = vm.Estado,
                RutaStoryboard = rutaNueva
            });
            TempData["Ok"] = "Escena creada.";
        }
        else
        {
            var e = await _escenas.GetDelDirectorAsync(vm.Id, directorId);
            if (e == null) return NotFound();

            var rutaVieja = e.RutaStoryboard;
            e.Numero = vm.Numero;
            e.Descripcion = string.IsNullOrWhiteSpace(vm.Descripcion) ? null : vm.Descripcion.Trim();
            e.DuracionSegundos = vm.DuracionSegundos;
            e.Estado = vm.Estado;
            if (rutaNueva != null) e.RutaStoryboard = rutaNueva;
            await _escenas.UpdateAsync(e);

            if (rutaNueva != null) _files.EliminarPrivado(rutaVieja);   // se reemplaza el archivo anterior
            TempData["Ok"] = "Escena actualizada.";
        }

        return RedirectToAction(nameof(Index), new { proyectoId = vm.ProyectoId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var e = await _escenas.GetDelDirectorAsync(id, User.GetUserId());
        if (e == null) return NotFound();

        var proyectoId = e.ProyectoId;

        if (await _escenas.TieneAsignacionesAsync(id))
        {
            TempData["Error"] = "No se puede eliminar: la escena tiene asignaciones.";
            return RedirectToAction(nameof(Index), new { proyectoId });
        }

        var ruta = e.RutaStoryboard;
        await _escenas.DeleteAsync(e);
        _files.EliminarPrivado(ruta);

        TempData["Ok"] = "Escena eliminada.";
        return RedirectToAction(nameof(Index), new { proyectoId });
    }

    [HttpGet]
    public async Task<IActionResult> Storyboard(int id)
    {
        var e = await _escenas.GetDelDirectorAsync(id, User.GetUserId());
        if (e?.RutaStoryboard == null) return NotFound();

        var stream = _files.AbrirPrivado(e.RutaStoryboard);
        if (stream == null) return NotFound();

        new FileExtensionContentTypeProvider().TryGetContentType(e.RutaStoryboard, out var tipo);
        return File(stream, tipo ?? "application/octet-stream");
    }
}
