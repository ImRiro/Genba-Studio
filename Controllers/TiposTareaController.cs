using GenbaStudio.Models.Entities;
using GenbaStudio.Models.Enums;
using GenbaStudio.Models.ViewModels;
using GenbaStudio.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenbaStudio.Controllers;

[Authorize(Roles = Roles.Administrador)]
public class TiposTareaController : Controller
{
    private readonly ITipoTareaRepository _repo;

    public TiposTareaController(ITipoTareaRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> Index() => View(await _repo.GetAllAsync());

    [HttpGet]
    public IActionResult Create() => View(new TipoTareaFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TipoTareaFormViewModel vm)
    {
        if (ModelState.IsValid && await _repo.ExisteNombreAsync(vm.Nombre.Trim()))
            ModelState.AddModelError(nameof(vm.Nombre), "Ya existe un tipo de tarea con ese nombre.");
        if (!ModelState.IsValid) return View(vm);

        await _repo.AddAsync(new TipoTarea
        {
            Nombre = vm.Nombre.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(vm.Descripcion) ? null : vm.Descripcion.Trim()
        });

        TempData["Ok"] = "Tipo de tarea creado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var t = await _repo.GetByIdAsync(id);
        if (t == null) return NotFound();
        return View(new TipoTareaFormViewModel { Id = t.Id, Nombre = t.Nombre, Descripcion = t.Descripcion });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TipoTareaFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();
        if (ModelState.IsValid && await _repo.ExisteNombreAsync(vm.Nombre.Trim(), id))
            ModelState.AddModelError(nameof(vm.Nombre), "Ya existe un tipo de tarea con ese nombre.");
        if (!ModelState.IsValid) return View(vm);

        var t = await _repo.GetByIdAsync(id);
        if (t == null) return NotFound();

        t.Nombre = vm.Nombre.Trim();
        t.Descripcion = string.IsNullOrWhiteSpace(vm.Descripcion) ? null : vm.Descripcion.Trim();
        await _repo.UpdateAsync(t);

        TempData["Ok"] = "Tipo de tarea actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _repo.GetByIdAsync(id);
        if (t == null) return NotFound();

        ViewData["EnUso"] = await _repo.TieneAsignacionesAsync(id);
        return View(t);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var t = await _repo.GetByIdAsync(id);
        if (t == null) return NotFound();

        if (await _repo.TieneAsignacionesAsync(id))
        {
            TempData["Error"] = "No se puede eliminar: hay asignaciones de este tipo.";
            return RedirectToAction(nameof(Index));
        }

        await _repo.DeleteAsync(t);
        TempData["Ok"] = "Tipo de tarea eliminado.";
        return RedirectToAction(nameof(Index));
    }
}
