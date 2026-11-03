using GenbaStudio.Models.Common;
using GenbaStudio.Models.Entities;
using GenbaStudio.Models.Enums;

namespace GenbaStudio.Repositories;

public interface IProyectoRepository : IRepository<Proyecto>
{
    Task<PagedResult<Proyecto>> GetPaginaAsync(int directorId, string? buscar, EstadoProyecto? estado, int page, int pageSize);
    Task<Proyecto?> GetDelDirectorAsync(int id, int directorId);
    Task<bool> TieneAsignacionesAsync(int proyectoId);
}
