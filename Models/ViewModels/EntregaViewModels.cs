using GenbaStudio.Models.Dtos;
using GenbaStudio.Models.Enums;

namespace GenbaStudio.Models.ViewModels;

public class EntregaItemViewModel
{
    public int Id { get; set; }
    public int AsignacionId { get; set; }
    public int Version { get; set; }
    public string NombreOriginal { get; set; } = string.Empty;
    public DateTime FechaSubida { get; set; }
    public EstadoRevision EstadoRevision { get; set; }
    public string? ComentarioAnimador { get; set; }
    public string? FeedbackDirector { get; set; }

    public string Proyecto { get; set; } = string.Empty;
    public int EscenaNumero { get; set; }
    public string TipoTarea { get; set; } = string.Empty;
    public string Animador { get; set; } = string.Empty;
}

public class DetalleAnimadorViewModel
{
    public AsignacionDto Asignacion { get; set; } = null!;
    public List<EntregaItemViewModel> Historial { get; set; } = new();
    public bool TieneStoryboard { get; set; }

    public bool PuedeIniciar =>
        Asignacion.Estado is EstadoAsignacion.Pendiente or EstadoAsignacion.ConCorrecciones;

    public bool PuedeSubir =>
        Asignacion.Estado is EstadoAsignacion.EnProgreso or EstadoAsignacion.ConCorrecciones;
}

public class RevisarEntregaViewModel
{
    public int EntregaId { get; set; }
    public AsignacionDto Asignacion { get; set; } = null!;
    public List<EntregaItemViewModel> Historial { get; set; } = new();
    public bool PuedeRevisar { get; set; }
}
