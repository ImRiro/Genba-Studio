using GenbaStudio.Models.Enums;

namespace GenbaStudio.Models.Entities;

public class Escena
{
    public int Id { get; set; }
    public int ProyectoId { get; set; }
    public int Numero { get; set; }
    public string? Descripcion { get; set; }
    public int DuracionSegundos { get; set; }
    public EstadoEscena Estado { get; set; } = EstadoEscena.Pendiente;
    public string? RutaStoryboard { get; set; }

    public Proyecto Proyecto { get; set; } = null!;
    public ICollection<Asignacion> Asignaciones { get; set; } = new List<Asignacion>();
}
