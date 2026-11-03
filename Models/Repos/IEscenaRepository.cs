using GenbaStudio.Models.Common;
using GenbaStudio.Models.Entities;

namespace GenbaStudio.Repositories;

public interface IEscenaRepository : IRepository<Escena>
{
    Task<PagedResult<Escena>> GetPaginaAsync(int proyectoId, string? buscar, int page, int pageSize);
    Task<Escena?> GetDelDirectorAsync(int id, int directorId);
    Task<bool> ExisteNumeroAsync(int proyectoId, int numero, int? excluirId);
    Task<int> SiguienteNumeroAsync(int proyectoId);
    Task<bool> TieneAsignacionesAsync(int escenaId);
    Task<List<string>> GetRutasStoryboardAsync(int proyectoId);
}
