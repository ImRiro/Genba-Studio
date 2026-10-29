namespace GenbaStudio.Models.Enums;

public static class Roles
{
    public const string Administrador = "Administrador";
    public const string Director = "Director";
    public const string Animador = "Animador";

    public static readonly string[] Todos = { Administrador, Director, Animador };
}

public enum EstadoProyecto
{
    Planificacion,
    EnProduccion,
    Finalizado
}

public enum EstadoEscena
{
    Pendiente,
    EnProduccion,
    Finalizada
}

public enum EstadoAsignacion
{
    Pendiente,
    EnProgreso,
    EnRevision,
    Aprobada,
    ConCorrecciones
}

public enum Prioridad
{
    Baja,
    Media,
    Alta
}

public enum EstadoRevision
{
    Pendiente,
    Aprobada,
    ConCorrecciones
}
