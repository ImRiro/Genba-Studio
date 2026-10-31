using System.ComponentModel.DataAnnotations;

namespace GenbaStudio.Models.ViewModels;

public class TipoTareaFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(60)]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Descripción")]
    [StringLength(300)]
    public string? Descripcion { get; set; }
}
