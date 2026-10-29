using GenbaStudio.Models.Entities;
using GenbaStudio.Models.Enums;
using GenbaStudio.Models.ViewModels;
using GenbaStudio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GenbaStudio.Controllers;

[Authorize]
public class PerfilController : Controller
{
    private readonly UserManager<Usuario> _users;
    private readonly SignInManager<Usuario> _signIn;
    private readonly IFileService _files;

    public PerfilController(UserManager<Usuario> users, SignInManager<Usuario> signIn, IFileService files)
    {
        _users = users;
        _signIn = signIn;
        _files = files;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _users.GetUserAsync(User);
        if (user == null) return Challenge();

        var vm = new PerfilViewModel
        {
            Nombre = user.Nombre,
            Apellido = user.Apellido,
            Especialidad = user.Especialidad
        };
        await CompletarSoloLecturaAsync(vm, user);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(PerfilViewModel vm)
    {
        var user = await _users.GetUserAsync(User);
        if (user == null) return Challenge();

        var esAnimador = await _users.IsInRoleAsync(user, Roles.Animador);

        if (ModelState.IsValid)
        {
            string? avatarNuevo = null;
            if (vm.AvatarFile is { Length: > 0 })
            {
                try
                {
                    avatarNuevo = await _files.GuardarPublicoAsync(vm.AvatarFile, "avatars", FileRules.Imagen);
                }
                catch (ArchivoInvalidoException ex)
                {
                    ModelState.AddModelError(nameof(vm.AvatarFile), ex.Message);
                }
            }

            if (ModelState.IsValid)
            {
                var avatarViejo = user.Avatar;
                user.Nombre = vm.Nombre.Trim();
                user.Apellido = vm.Apellido.Trim();
                if (esAnimador)
                    user.Especialidad = string.IsNullOrWhiteSpace(vm.Especialidad) ? null : vm.Especialidad.Trim();

                if (avatarNuevo != null) user.Avatar = avatarNuevo;
                else if (vm.QuitarAvatar) user.Avatar = null;

                var result = await _users.UpdateAsync(user);
                if (result.Succeeded)
                {
                    if (avatarNuevo != null || vm.QuitarAvatar) _files.EliminarPublico(avatarViejo);
                    await _signIn.RefreshSignInAsync(user);
                    TempData["Ok"] = "Perfil actualizado.";
                    return RedirectToAction(nameof(Index));
                }

                if (avatarNuevo != null) _files.EliminarPublico(avatarNuevo);
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            }
        }

        await CompletarSoloLecturaAsync(vm, user);
        return View(vm);
    }

    private async Task CompletarSoloLecturaAsync(PerfilViewModel vm, Usuario user)
    {
        var roles = await _users.GetRolesAsync(user);
        vm.Email = user.Email;
        vm.Avatar = user.Avatar;
        vm.Rol = string.Join(", ", roles);
        vm.EsAnimador = roles.Contains(Roles.Animador);
    }
}
