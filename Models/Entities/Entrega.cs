using GenbaStudio.Models.Enums;

namespace GenbaStudio.Models.Entities;

public class Entrega
{
    public int Id { get; set; }
    public int AsignacionId { get; set; }
    public string RutaArchivo { get; set; } = string.Empty;
    public string NombreOriginal { get; set; } = string.Empty;
    public int Version { get; set; }
    public string? ComentarioAnimador { get; set; }
    public DateTime FechaSubida { get; set; }
    public EstadoRevision EstadoRevision { get; set; } = EstadoRevision.Pendiente;
    public string? FeedbackDirector { get; set; }

    public Asignacion Asignacion { get; set; } = null!;
}
