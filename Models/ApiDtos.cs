using System.ComponentModel.DataAnnotations;
using GenbaStudio.Models.Enums;

namespace GenbaStudio.Models.Dtos;

public class LoginApiRequest
{
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no es válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Password { get; set; } = string.Empty;
}

public class AsignacionDto
{
    public int Id { get; set; }
    public int EscenaId { get; set; }
    public string Proyecto { get; set; } = string.Empty;
    public int EscenaNumero { get; set; }
    public int TipoTareaId { get; set; }
    public string TipoTarea { get; set; } = string.Empty;
    public int AnimadorId { get; set; }
    public string Animador { get; set; } = string.Empty;
    public DateTime FechaAsignacion { get; set; }
    public DateTime FechaLimite { get; set; }
    public Prioridad Prioridad { get; set; }
    public EstadoAsignacion Estado { get; set; }
    public string? Observaciones { get; set; }

    public bool Vencida => FechaLimite.Date < DateTime.Today && Estado != EstadoAsignacion.Aprobada;
}

// Lo que recibe la API al crear o editar
public class AsignacionGuardarDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Elegí una escena.")]
    public int EscenaId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Elegí un tipo de tarea.")]
    public int TipoTareaId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Elegí un animador.")]
    public int AnimadorId { get; set; }

    public DateTime FechaLimite { get; set; }

    public Prioridad Prioridad { get; set; } = Prioridad.Media;

    [StringLength(1000)]
    public string? Observaciones { get; set; }
}

// para los selects con búsqueda
public class OpcionDto
{
    public int Id { get; set; }
    public string Texto { get; set; } = string.Empty;
    public string? Detalle { get; set; }
}
