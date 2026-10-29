using GenbaStudio.Models.Enums;

namespace GenbaStudio.Models.Entities;

public class Proyecto
{
    public int Id { get; set; }
    public int DirectorId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaEntrega { get; set; }
    public EstadoProyecto Estado { get; set; } = EstadoProyecto.Planificacion;

    public Usuario Director { get; set; } = null!;
    public ICollection<Escena> Escenas { get; set; } = new List<Escena>();
}
