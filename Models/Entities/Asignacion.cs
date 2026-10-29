using GenbaStudio.Models.Enums;

namespace GenbaStudio.Models.Entities;

public class Asignacion
{
    public int Id { get; set; }
    public int EscenaId { get; set; }
    public int TipoTareaId { get; set; }
    public int AnimadorId { get; set; }
    public DateTime FechaAsignacion { get; set; }
    public DateTime FechaLimite { get; set; }
    public Prioridad Prioridad { get; set; } = Prioridad.Media;
    public EstadoAsignacion Estado { get; set; } = EstadoAsignacion.Pendiente;
    public string? Observaciones { get; set; }

    public Escena Escena { get; set; } = null!;
    public TipoTarea TipoTarea { get; set; } = null!;
    public Usuario Animador { get; set; } = null!;
    public ICollection<Entrega> Entregas { get; set; } = new List<Entrega>();
}
