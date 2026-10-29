using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GenbaStudio.Models.Entities;

// El rol se resuelve con Identity, esto difiere con el DER pero en la practica es lo mismo.
public class Usuario : IdentityUser<int>
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string? Especialidad { get; set; }

    [NotMapped]
    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();

    public ICollection<Proyecto> ProyectosDirigidos { get; set; } = new List<Proyecto>();
    public ICollection<Asignacion> Asignaciones { get; set; } = new List<Asignacion>();
}
