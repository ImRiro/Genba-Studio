using GenbaStudio.Models.Common;
using GenbaStudio.Models.Entities;
using GenbaStudio.Models.Enums;
using GenbaStudio.Models.ViewModels;

namespace GenbaStudio.Repositories;

public interface IEntregaRepository : IRepository<Entrega>
{
    Task<PagedResult<EntregaItemViewModel>> GetPendientesDelDirectorAsync(int directorId, int page, int pageSize);
    Task<List<EntregaItemViewModel>> GetHistorialAsync(int asignacionId);
    Task<Entrega?> GetConAsignacionAsync(int id);
    Task AgregarConVersionAsync(Entrega entrega);
    Task RegistrarRevisionAsync(Entrega entrega, EstadoRevision decision, string? feedback);
}
