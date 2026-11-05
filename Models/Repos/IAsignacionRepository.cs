using GenbaStudio.Models.Common;
using GenbaStudio.Models.Dtos;
using GenbaStudio.Models.Entities;
using GenbaStudio.Models.Enums;

namespace GenbaStudio.Repositories;

public interface IAsignacionRepository : IRepository<Asignacion>
{
    Task<PagedResult<AsignacionDto>> GetPaginaAsync(int? directorId, int? animadorId, string? buscar, EstadoAsignacion? estado, int page, int pageSize);
    Task<AsignacionDto?> GetDtoAsync(int id, int? directorId, int? animadorId);
    Task<Asignacion?> GetDelDirectorAsync(int id, int directorId);
    Task<bool> ExisteActivaAsync(int escenaId, int tipoTareaId, int? excluirId);
    Task<bool> TieneEntregasAsync(int asignacionId);
    Task<bool> ExisteAnimadorAsync(int usuarioId);
    Task<List<OpcionDto>> BuscarAnimadoresAsync(string? q);
    Task<List<OpcionDto>> BuscarEscenasAsync(int directorId, string? q, int? proyectoId);
    Task<Asignacion?> GetDelAnimadorAsync(int id, int animadorId);
    Task<bool> AnimadorTieneEscenaAsync(int animadorId, int escenaId);
}
