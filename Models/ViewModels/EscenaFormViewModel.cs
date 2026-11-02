using System.ComponentModel.DataAnnotations;
using GenbaStudio.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace GenbaStudio.Models.ViewModels;

public class EscenaFormViewModel
{
    public int Id { get; set; }
    public int ProyectoId { get; set; }

    [Display(Name = "Número")]
    [Range(1, 9999, ErrorMessage = "El número debe estar entre 1 y 9999.")]
    public int Numero { get; set; }

    [Display(Name = "Descripción")]
    [StringLength(1000)]
    public string? Descripcion { get; set; }

    [Display(Name = "Duración (segundos)")]
    [Range(1, 3600, ErrorMessage = "La duración debe estar entre 1 y 3600 segundos.")]
    public int DuracionSegundos { get; set; } = 5;

    public EstadoEscena Estado { get; set; } = EstadoEscena.Pendiente;

    [Display(Name = "Storyboard")]
    public IFormFile? Storyboard { get; set; }

    // Solo lectura
    public string? ProyectoNombre { get; set; }
    public bool TieneStoryboard { get; set; }
}
