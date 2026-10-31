namespace GenbaStudio.Models.Common;

public class ServiceResult
{
    public List<string> Errores { get; } = new();
    public bool Ok => Errores.Count == 0;

    public static ServiceResult Exito() => new();

    public static ServiceResult Falla(params string[] errores)
    {
        var r = new ServiceResult();
        r.Errores.AddRange(errores);
        return r;
    }
}
