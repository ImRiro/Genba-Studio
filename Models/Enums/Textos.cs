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

    public static string De(EstadoAsignacion e) => e switch
    {
        EstadoAsignacion.Pendiente => "Pendiente",
        EstadoAsignacion.EnProgreso => "En progreso",
        EstadoAsignacion.EnRevision => "En revisión",
        EstadoAsignacion.Aprobada => "Aprobada",
        _ => "Con correcciones"
    };

    public static string De(EstadoRevision e) => e switch
    {
        EstadoRevision.Pendiente => "Pendiente de revisión",
        EstadoRevision.Aprobada => "Aprobada",
        _ => "Con correcciones"
    };

    public static string De(Prioridad p) => p switch
    {
        Prioridad.Baja => "Baja",
        Prioridad.Media => "Media",
        _ => "Alta"
    };

    // Bootstrap generalizado
    public static string Clase(EstadoAsignacion e) => e switch
    {
        EstadoAsignacion.Pendiente => "bg-secondary",
        EstadoAsignacion.EnProgreso => "bg-primary",
        EstadoAsignacion.EnRevision => "bg-info text-dark",
        EstadoAsignacion.Aprobada => "bg-success",
        _ => "bg-warning text-dark"
    };

    public static string Clase(EstadoRevision e) => e switch
    {
        EstadoRevision.Pendiente => "bg-info text-dark",
        EstadoRevision.Aprobada => "bg-success",
        _ => "bg-warning text-dark"
    };
}
