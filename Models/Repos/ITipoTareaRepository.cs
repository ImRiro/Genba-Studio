using GenbaStudio.Models.Entities;

namespace GenbaStudio.Repositories;

public interface ITipoTareaRepository : IRepository<TipoTarea>
{
    Task<List<TipoTarea>> GetAllAsync();
    Task<bool> ExisteNombreAsync(string nombre, int? excluirId = null);
    Task<bool> TieneAsignacionesAsync(int id);
}
