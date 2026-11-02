using System.ComponentModel.DataAnnotations;
using GenbaStudio.Models.Enums;

namespace GenbaStudio.Models.ViewModels;

public class ProyectoFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    [StringLength(1000)]
    public string? Descripcion { get; set; }

    [Display(Name = "Fecha de inicio")]
    public DateTime FechaInicio { get; set; }

    [Display(Name = "Fecha de entrega")]
    public DateTime FechaEntrega { get; set; }

    public EstadoProyecto Estado { get; set; } = EstadoProyecto.Planificacion;
}
