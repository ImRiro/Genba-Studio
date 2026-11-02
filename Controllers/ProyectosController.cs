using GenbaStudio.Models.Entities;
using GenbaStudio.Models.Enums;
using GenbaStudio.Models.ViewModels;
using GenbaStudio.Repositories;
using GenbaStudio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenbaStudio.Controllers;

[Authorize(Roles = Roles.Director)]
public class ProyectosController : Controller
{
    private const int PageSize = 10;

    private readonly IProyectoRepository _proyectos;
    private readonly IEscenaRepository _escenas;
    private readonly IFileService _files;

    public ProyectosController(IProyectoRepository proyectos, IEscenaRepository escenas, IFileService files)
    {
        _proyectos = proyectos;
        _escenas = escenas;
        _files = files;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? q, EstadoProyecto? estado, int page = 1)
    {
        var directorId = User.GetUserId();
        var resultado = await _proyectos.GetPaginaAsync(directorId, q, estado, page, PageSize);

        ViewData["q"] = q;
        ViewData["estado"] = estado;
        return View(resultado);
    }

    [HttpGet]
    public IActionResult Nuevo() =>
        View("Form", new ProyectoFormViewModel
        {
            FechaInicio = DateTime.Today,
            FechaEntrega = DateTime.Today.AddDays(30)
        });

    [HttpGet]
    public async Task<IActionResult> Editar(int id)
    {
        var p = await _proyectos.GetDelDirectorAsync(id, User.GetUserId());
        if (p == null) return NotFound();

        return View("Form", new ProyectoFormViewModel
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            FechaInicio = p.FechaInicio,
            FechaEntrega = p.FechaEntrega,
            Estado = p.Estado
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(ProyectoFormViewModel vm)
    {
        if (vm.FechaEntrega < vm.FechaInicio)
            ModelState.AddModelError(nameof(vm.FechaEntrega), "La fecha de entrega no puede ser anterior a la de inicio.");

        if (!ModelState.IsValid) return View("Form", vm);

        var directorId = User.GetUserId();

        if (vm.Id == 0)
        {
            await _proyectos.AddAsync(new Proyecto
            {
                DirectorId = directorId,
                Nombre = vm.Nombre.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(vm.Descripcion) ? null : vm.Descripcion.Trim(),
                FechaInicio = vm.FechaInicio,
                FechaEntrega = vm.FechaEntrega,
                Estado = vm.Estado
            });
            TempData["Ok"] = "Proyecto creado.";
        }
        else
        {
            var p = await _proyectos.GetDelDirectorAsync(vm.Id, directorId);
            if (p == null) return NotFound();

            p.Nombre = vm.Nombre.Trim();
            p.Descripcion = string.IsNullOrWhiteSpace(vm.Descripcion) ? null : vm.Descripcion.Trim();
            p.FechaInicio = vm.FechaInicio;
            p.FechaEntrega = vm.FechaEntrega;
            p.Estado = vm.Estado;
            await _proyectos.UpdateAsync(p);
            TempData["Ok"] = "Proyecto actualizado.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(int id)
    {
        var p = await _proyectos.GetDelDirectorAsync(id, User.GetUserId());
        if (p == null) return NotFound();

        if (await _proyectos.TieneAsignacionesAsync(id))
        {
            TempData["Error"] = "No se puede eliminar: el proyecto tiene escenas con asignaciones.";
            return RedirectToAction(nameof(Index));
        }

        var rutas = await _escenas.GetRutasStoryboardAsync(id);
        await _proyectos.DeleteAsync(p);
        foreach (var ruta in rutas) _files.EliminarPrivado(ruta);

        TempData["Ok"] = "Proyecto eliminado.";
        return RedirectToAction(nameof(Index));
    }
}
