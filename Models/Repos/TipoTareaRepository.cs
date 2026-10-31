using GenbaStudio.Data;
using GenbaStudio.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace GenbaStudio.Repositories;

public interface ITipoTareaRepository : IRepository<TipoTarea>
{
    Task<List<TipoTarea>> GetAllAsync();
    Task<bool> ExisteNombreAsync(string nombre, int? excluirId = null);
    Task<bool> TieneAsignacionesAsync(int id);
}

public class TipoTareaRepository : Repository<TipoTarea>, ITipoTareaRepository
{
    public TipoTareaRepository(AppDbContext context) : base(context) { }

    public Task<List<TipoTarea>> GetAllAsync() =>
        Set.AsNoTracking().OrderBy(t => t.Nombre).ToListAsync();

    public Task<bool> ExisteNombreAsync(string nombre, int? excluirId = null) =>
        Set.AnyAsync(t => t.Nombre == nombre && (excluirId == null || t.Id != excluirId));

    public Task<bool> TieneAsignacionesAsync(int id) =>
        Context.Asignaciones.AnyAsync(a => a.TipoTareaId == id);
}
