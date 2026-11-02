using GenbaStudio.Data;
using GenbaStudio.Models.Common;
using GenbaStudio.Models.Entities;
using Microsoft.EntityFrameworkCore;

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

public class EscenaRepository : Repository<Escena>, IEscenaRepository
{
    public EscenaRepository(AppDbContext context) : base(context) { }

    public async Task<PagedResult<Escena>> GetPaginaAsync(int proyectoId, string? buscar, int page, int pageSize)
    {

        var query = Context.Escenas.AsNoTracking().Where(e => e.ProyectoId == proyectoId);

        if (!string.IsNullOrWhiteSpace(buscar))
            query = query.Where(e => e.Descripcion != null && e.Descripcion.Contains(buscar));

        var total = await query.CountAsync();

        var totalPaginas = (int)Math.Ceiling(total / (double)pageSize);
        if (page > totalPaginas) page = Math.Max(1, totalPaginas);
        if (page < 1) page = 1;

        var items = await query
            .OrderBy(e => e.Numero)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Escena> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    }

    // La escena solo se devuelve si su proyecto pertenece a ese director
    public Task<Escena?> GetDelDirectorAsync(int id, int directorId) =>
        Context.Escenas
            .Include(e => e.Proyecto)
            .FirstOrDefaultAsync(e => e.Id == id && e.Proyecto.DirectorId == directorId);

    // No puede haber dos escenas con el mismo número dentro de un proyecto.
    public Task<bool> ExisteNumeroAsync(int proyectoId, int numero, int? excluirId) =>
        Context.Escenas.AnyAsync(e =>
            e.ProyectoId == proyectoId && e.Numero == numero && (excluirId == null || e.Id != excluirId));

    public async Task<int> SiguienteNumeroAsync(int proyectoId)
    {
        var maximo = await Context.Escenas
            .Where(e => e.ProyectoId == proyectoId)
            .MaxAsync(e => (int?)e.Numero);
        return (maximo ?? 0) + 1;
    }

    public Task<bool> TieneAsignacionesAsync(int escenaId) =>
        Context.Asignaciones.AnyAsync(a => a.EscenaId == escenaId);

    public Task<List<string>> GetRutasStoryboardAsync(int proyectoId) =>
        Context.Escenas
            .Where(e => e.ProyectoId == proyectoId && e.RutaStoryboard != null)
            .Select(e => e.RutaStoryboard!)
            .ToListAsync();
}
