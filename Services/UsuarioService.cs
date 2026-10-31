using GenbaStudio.Models.Common;
using GenbaStudio.Models.Entities;
using GenbaStudio.Models.Enums;
using GenbaStudio.Models.ViewModels;
using GenbaStudio.Repositories;
using Microsoft.AspNetCore.Identity;

namespace GenbaStudio.Services;

// Servicio usado para la logica de negocio
// separada del repositorio para no depender de la cookie de sesion
// ademas de evitar que el controlador repita la misma logica en distintos metodos en un futuro

public interface IUsuarioService
{
    Task<UsuarioFormViewModel?> ObtenerParaEditarAsync(int id);
    Task<UsuarioDeleteViewModel?> ObtenerParaEliminarAsync(int id);
    Task<ServiceResult> CrearAsync(UsuarioFormViewModel vm);
    Task<ServiceResult> EditarAsync(UsuarioFormViewModel vm, int usuarioActualId);
    Task<ServiceResult> EliminarAsync(int id, int usuarioActualId);
}

public class UsuarioService : IUsuarioService
{
    private readonly UserManager<Usuario> _users;
    private readonly SignInManager<Usuario> _signIn;
    private readonly IUsuarioRepository _repo;
    private readonly IFileService _files;

    public UsuarioService(UserManager<Usuario> users, SignInManager<Usuario> signIn,
        IUsuarioRepository repo, IFileService files)
    {
        _users = users;
        _signIn = signIn;
        _repo = repo;
        _files = files;
    }

    public async Task<UsuarioFormViewModel?> ObtenerParaEditarAsync(int id)
    {
        var u = await _users.FindByIdAsync(id.ToString());
        if (u == null) return null;

        var roles = await _users.GetRolesAsync(u);
        return new UsuarioFormViewModel
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            Email = u.Email ?? string.Empty,
            Rol = roles.FirstOrDefault() ?? string.Empty,
            Especialidad = u.Especialidad
        };
    }

    public async Task<UsuarioDeleteViewModel?> ObtenerParaEliminarAsync(int id)
    {
        var u = await _users.FindByIdAsync(id.ToString());
        if (u == null) return null;

        var roles = await _users.GetRolesAsync(u);
        return new UsuarioDeleteViewModel
        {
            Id = u.Id,
            NombreCompleto = u.NombreCompleto,
            Email = u.Email ?? string.Empty,
            Rol = roles.FirstOrDefault(),
            TieneRelaciones = await _repo.TieneRelacionesAsync(u.Id)
        };
    }

    public async Task<ServiceResult> CrearAsync(UsuarioFormViewModel vm)
    {
        if (!Roles.Todos.Contains(vm.Rol))
            return ServiceResult.Falla("El rol seleccionado no es válido.");
        if (string.IsNullOrWhiteSpace(vm.Password))
            return ServiceResult.Falla("La contraseña es obligatoria al crear un usuario.");

        var email = vm.Email.Trim();
        var usuario = new Usuario
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            Nombre = vm.Nombre.Trim(),
            Apellido = vm.Apellido.Trim(),
            Especialidad = EspecialidadSegunRol(vm)
        };

        var creado = await _users.CreateAsync(usuario, vm.Password);
        if (!creado.Succeeded) return Falla(creado);

        var conRol = await _users.AddToRoleAsync(usuario, vm.Rol);
        if (!conRol.Succeeded)
        {
            await _users.DeleteAsync(usuario);
            return Falla(conRol);
        }

        return ServiceResult.Exito();
    }

    public async Task<ServiceResult> EditarAsync(UsuarioFormViewModel vm, int usuarioActualId)
    {
        if (!Roles.Todos.Contains(vm.Rol))
            return ServiceResult.Falla("El rol seleccionado no es válido.");

        var u = await _users.FindByIdAsync(vm.Id.ToString());
        if (u == null) return ServiceResult.Falla("El usuario no existe.");

        var rolActual = (await _users.GetRolesAsync(u)).FirstOrDefault();
        var cambiaRol = rolActual != vm.Rol;

        if (cambiaRol)
        {
            // Evita quedarse sin administradores
            if (u.Id == usuarioActualId && rolActual == Roles.Administrador)
                return ServiceResult.Falla("No podés quitarte tu propio rol de Administrador.");

            // Un Director con proyectos o un Animador con asignaciones no puede cambiar de rol
            if (await _repo.TieneRelacionesAsync(u.Id))
                return ServiceResult.Falla("No se puede cambiar el rol de un usuario con proyectos o asignaciones.");
        }

        var cambiaClave = !string.IsNullOrWhiteSpace(vm.Password);
        if (cambiaClave)
        {
            foreach (var validador in _users.PasswordValidators)
            {
                var r = await validador.ValidateAsync(_users, u, vm.Password!);
                if (!r.Succeeded) return Falla(r);
            }
        }

        var email = vm.Email.Trim();
        u.Nombre = vm.Nombre.Trim();
        u.Apellido = vm.Apellido.Trim();
        u.Email = email;
        u.UserName = email;
        u.Especialidad = EspecialidadSegunRol(vm);

        var actualizado = await _users.UpdateAsync(u);
        if (!actualizado.Succeeded) return Falla(actualizado);

        if (cambiaRol)
        {
            if (rolActual != null) await _users.RemoveFromRoleAsync(u, rolActual);
            var conRol = await _users.AddToRoleAsync(u, vm.Rol);
            if (!conRol.Succeeded) return Falla(conRol);
        }

        if (cambiaClave)
        {
            var token = await _users.GeneratePasswordResetTokenAsync(u);
            var reset = await _users.ResetPasswordAsync(u, token, vm.Password!);
            if (!reset.Succeeded) return Falla(reset);
        }

        if (u.Id == usuarioActualId)
            await _signIn.RefreshSignInAsync(u);

        return ServiceResult.Exito();
    }

    public async Task<ServiceResult> EliminarAsync(int id, int usuarioActualId)
    {
        if (id == usuarioActualId)
            return ServiceResult.Falla("No podés eliminar tu propia cuenta.");

        var u = await _users.FindByIdAsync(id.ToString());
        if (u == null) return ServiceResult.Falla("El usuario no existe.");

        // No se elimina un Usuario con proyectos o asignaciones asociadas
        if (await _repo.TieneRelacionesAsync(id))
            return ServiceResult.Falla("No se puede eliminar: el usuario tiene proyectos o asignaciones asociadas.");

        var avatar = u.Avatar;
        var r = await _users.DeleteAsync(u);
        if (!r.Succeeded) return Falla(r);

        _files.EliminarPublico(avatar);
        return ServiceResult.Exito();
    }

    // La especialidad solo tiene sentido para animadores
    private static string? EspecialidadSegunRol(UsuarioFormViewModel vm) =>
        vm.Rol == Roles.Animador && !string.IsNullOrWhiteSpace(vm.Especialidad)
            ? vm.Especialidad.Trim()
            : null;

    private static ServiceResult Falla(IdentityResult r) =>
        ServiceResult.Falla(r.Errors.Select(e => e.Description).Distinct().ToArray());
}
