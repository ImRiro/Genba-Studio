namespace GenbaStudio.Models.Enums;

// Convierte los valores de los enums en texto para mostrar en pantalla (con tildes y espacios)
public static class Textos
{
    public static string De(EstadoProyecto e) => e switch
    {
        EstadoProyecto.Planificacion => "Planificación",
        EstadoProyecto.EnProduccion => "En producción",
        _ => "Finalizado"
    };

    public static string De(EstadoEscena e) => e switch
    {
        EstadoEscena.Pendiente => "Pendiente",
        EstadoEscena.EnProduccion => "En producción",
        _ => "Finalizada"
    };
}
