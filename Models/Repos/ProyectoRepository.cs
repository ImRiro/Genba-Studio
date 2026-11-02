using GenbaStudio.Data;
using GenbaStudio.Models.Common;
using GenbaStudio.Models.Entities;
using GenbaStudio.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace GenbaStudio.Repositories;

public interface IProyectoRepository : IRepository<Proyecto>
{
    Task<PagedResult<Proyecto>> GetPaginaAsync(int directorId, string? buscar, EstadoProyecto? estado, int page, int pageSize);
    Task<Proyecto?> GetDelDirectorAsync(int id, int directorId);
    Task<bool> TieneAsignacionesAsync(int proyectoId);
}

public class ProyectoRepository : Repository<Proyecto>, IProyectoRepository
{
    public ProyectoRepository(AppDbContext context) : base(context) { }

    public async Task<PagedResult<Proyecto>> GetPaginaAsync(
        int directorId, string? buscar, EstadoProyecto? estado, int page, int pageSize)
    {

        var query = Context.Proyectos.AsNoTracking().Where(p => p.DirectorId == directorId);

        if (!string.IsNullOrWhiteSpace(buscar))
            query = query.Where(p => p.Nombre.Contains(buscar));
        if (estado != null)
            query = query.Where(p => p.Estado == estado);

        var total = await query.CountAsync();

        var totalPaginas = (int)Math.Ceiling(total / (double)pageSize);
        if (page > totalPaginas) page = Math.Max(1, totalPaginas);
        if (page < 1) page = 1;

        var items = await query
            .OrderByDescending(p => p.FechaInicio)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Proyecto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    }

    public Task<Proyecto?> GetDelDirectorAsync(int id, int directorId) =>
        Context.Proyectos.FirstOrDefaultAsync(p => p.Id == id && p.DirectorId == directorId);

    public Task<bool> TieneAsignacionesAsync(int proyectoId) =>
        Context.Asignaciones.AnyAsync(a => a.Escena.ProyectoId == proyectoId);
}
