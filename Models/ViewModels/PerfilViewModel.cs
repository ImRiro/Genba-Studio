using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GenbaStudio.Models.ViewModels;

public class PerfilViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(100)]
    public string Apellido { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Especialidad { get; set; }

    // Solo lectura
    public string? Email { get; set; }
    public string? Rol { get; set; }
    public string? Avatar { get; set; }
    public bool EsAnimador { get; set; }

    [Display(Name = "Nuevo avatar")]
    public IFormFile? AvatarFile { get; set; }

    [Display(Name = "Quitar avatar actual")]
    public bool QuitarAvatar { get; set; }
}
