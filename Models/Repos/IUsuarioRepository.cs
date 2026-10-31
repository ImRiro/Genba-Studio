using GenbaStudio.Models.Common;

namespace GenbaStudio.Repositories;

public interface IUsuarioRepository
{
    Task<PagedResult<UsuarioListItem>> GetPagedAsync(string? buscar, string? rol, int page, int pageSize);
    Task<bool> TieneRelacionesAsync(int usuarioId);
}
