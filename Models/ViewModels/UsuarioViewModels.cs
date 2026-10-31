using System.ComponentModel.DataAnnotations;
using GenbaStudio.Models.Common;
using GenbaStudio.Repositories;

namespace GenbaStudio.Models.ViewModels;

public class UsuarioFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100)]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no es válido.")]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Elegí un rol.")]
    public string Rol { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Especialidad { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contraseña")]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    public string? ConfirmarPassword { get; set; }
}

public class UsuarioDeleteViewModel
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Rol { get; set; }
    public bool TieneRelaciones { get; set; }
}

public class UsuariosIndexViewModel
{
    public PagedResult<UsuarioListItem> Resultado { get; set; } = new();
    public string? Q { get; set; }
    public string? Rol { get; set; }
}
