using GenbaStudio.Models.Enums;
using GenbaStudio.Models.ViewModels;
using GenbaStudio.Repositories;
using GenbaStudio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenbaStudio.Controllers;

[Authorize(Roles = Roles.Administrador)]
public class UsuariosController : Controller
{
    private const int PageSize = 10;

    private readonly IUsuarioRepository _repo;
    private readonly IUsuarioService _service;

    public UsuariosController(IUsuarioRepository repo, IUsuarioService service)
    {
        _repo = repo;
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? q, string? rol, int page = 1)
    {
        var vm = new UsuariosIndexViewModel
        {
            Q = q,
            Rol = rol,
            Resultado = await _repo.GetPagedAsync(q, rol, page, PageSize)
        };
        return View(vm);
    }

    [HttpGet]
    public IActionResult Create() => View(new UsuarioFormViewModel { Rol = Roles.Animador });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioFormViewModel vm)
    {
        if (string.IsNullOrWhiteSpace(vm.Password))
            ModelState.AddModelError(nameof(vm.Password), "La contraseña es obligatoria.");
        if (!ModelState.IsValid) return View(vm);

        var result = await _service.CrearAsync(vm);
        if (!result.Ok)
        {
            foreach (var e in result.Errores) ModelState.AddModelError("", e);
            return View(vm);
        }

        TempData["Ok"] = $"Usuario {vm.Email} creado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var vm = await _service.ObtenerParaEditarAsync(id);
        return vm == null ? NotFound() : View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UsuarioFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        var result = await _service.EditarAsync(vm, User.GetUserId());
        if (!result.Ok)
        {
            foreach (var e in result.Errores) ModelState.AddModelError("", e);
            return View(vm);
        }

        TempData["Ok"] = "Usuario actualizado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var vm = await _service.ObtenerParaEliminarAsync(id);
        return vm == null ? NotFound() : View(vm);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _service.EliminarAsync(id, User.GetUserId());
        if (result.Ok) TempData["Ok"] = "Usuario eliminado.";
        else TempData["Error"] = string.Join(" ", result.Errores);
        return RedirectToAction(nameof(Index));
    }
}
