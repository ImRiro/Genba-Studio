using GenbaStudio.Data;
using GenbaStudio.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace GenbaStudio.Repositories;

public class UsuarioListItem
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Especialidad { get; set; }
    public string? Avatar { get; set; }
    public string? Rol { get; set; }
}

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _ctx;

    public UsuarioRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<PagedResult<UsuarioListItem>> GetPagedAsync(string? buscar, string? rol, int page, int pageSize)
    {
        var query = _ctx.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(rol))
        {
            var roleId = await _ctx.Roles
                .Where(r => r.Name == rol)
                .Select(r => (int?)r.Id)
                .FirstOrDefaultAsync();

            if (roleId == null)
                return new PagedResult<UsuarioListItem> { Page = 1, PageSize = pageSize };

            query = query.Where(u => _ctx.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == roleId));
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var b = buscar.Trim();
            query = query.Where(u =>
                u.Nombre.Contains(b) ||
                u.Apellido.Contains(b) ||
                (u.Email != null && u.Email.Contains(b)) ||
                (u.Especialidad != null && u.Especialidad.Contains(b)));
        }

        var proyectado = query
            .OrderBy(u => u.Apellido).ThenBy(u => u.Nombre)
            .Select(u => new UsuarioListItem
            {
                Id = u.Id,
                NombreCompleto = u.Nombre + " " + u.Apellido,
                Email = u.Email ?? "",
                Especialidad = u.Especialidad,
                Avatar = u.Avatar,
                Rol = _ctx.UserRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Join(_ctx.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .FirstOrDefault()
            });

        var total = await proyectado.CountAsync();

        var items = await proyectado
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<UsuarioListItem>
        {
            Items = items, Page = page, PageSize = pageSize, TotalCount = total
        };
    }

    public async Task<bool> TieneRelacionesAsync(int usuarioId) =>
        await _ctx.Proyectos.AnyAsync(p => p.DirectorId == usuarioId) ||
        await _ctx.Asignaciones.AnyAsync(a => a.AnimadorId == usuarioId);
}
