using GenbaStudio.Data;
using GenbaStudio.Models.Common;
using GenbaStudio.Models.Dtos;
using GenbaStudio.Models.Entities;
using GenbaStudio.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace GenbaStudio.Repositories;

public class AsignacionRepository : Repository<Asignacion>, IAsignacionRepository
{
    public AsignacionRepository(AppDbContext context) : base(context) { }

    private IQueryable<Asignacion> ConsultaVisible(int? directorId, int? animadorId)
    {
        if (directorId == null && animadorId == null)
            throw new InvalidOperationException("Hay que indicar un director o un animador.");

        var query = Context.Asignaciones.AsNoTracking();
        if (directorId != null)
            query = query.Where(a => a.Escena.Proyecto.DirectorId == directorId);
        if (animadorId != null)
            query = query.Where(a => a.AnimadorId == animadorId);
        return query;
    }

    // Pasa de entidad a DTO
    private static IQueryable<AsignacionDto> ADto(IQueryable<Asignacion> query) =>
        query.Select(a => new AsignacionDto
        {
            Id = a.Id,
            EscenaId = a.EscenaId,
            Proyecto = a.Escena.Proyecto.Nombre,
            EscenaNumero = a.Escena.Numero,
            TipoTareaId = a.TipoTareaId,
            TipoTarea = a.TipoTarea.Nombre,
            AnimadorId = a.AnimadorId,
            Animador = a.Animador.Nombre + " " + a.Animador.Apellido,
            FechaAsignacion = a.FechaAsignacion,
            FechaLimite = a.FechaLimite,
            Prioridad = a.Prioridad,
            Estado = a.Estado,
            Observaciones = a.Observaciones
        });

    public async Task<PagedResult<AsignacionDto>> GetPaginaAsync(
        int? directorId, int? animadorId, string? buscar, EstadoAsignacion? estado, int page, int pageSize)
    {
        if (page < 1) page = 1;

        var query = ConsultaVisible(directorId, animadorId);

        if (estado != null)
            query = query.Where(a => a.Estado == estado);

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var b = buscar.Trim();
            query = query.Where(a =>
                a.Escena.Proyecto.Nombre.Contains(b) ||
                a.TipoTarea.Nombre.Contains(b) ||
                a.Animador.Nombre.Contains(b) ||
                a.Animador.Apellido.Contains(b));
        }

        var total = await query.CountAsync();

        var paginaDeAsignaciones = query
            .OrderBy(a => a.FechaLimite).ThenBy(a => a.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        var items = await ADto(paginaDeAsignaciones).ToListAsync();

        return new PagedResult<AsignacionDto> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    }

    public Task<AsignacionDto?> GetDtoAsync(int id, int? directorId, int? animadorId) =>
        ADto(ConsultaVisible(directorId, animadorId).Where(a => a.Id == id)).FirstOrDefaultAsync();

    public Task<Asignacion?> GetDelDirectorAsync(int id, int directorId) =>
        Context.Asignaciones.FirstOrDefaultAsync(a => a.Id == id && a.Escena.Proyecto.DirectorId == directorId);

    // Una escena no puede tener dos asignaciones ACTIVAS del mismo tipo de tarea
    // Activa = todavía no aprobada
    public Task<bool> ExisteActivaAsync(int escenaId, int tipoTareaId, int? excluirId) =>
        Context.Asignaciones.AnyAsync(a =>
            a.EscenaId == escenaId &&
            a.TipoTareaId == tipoTareaId &&
            a.Estado != EstadoAsignacion.Aprobada &&
            (excluirId == null || a.Id != excluirId));

    public Task<bool> TieneEntregasAsync(int asignacionId) =>
        Context.Entregas.AnyAsync(e => e.AsignacionId == asignacionId);

    public Task<bool> ExisteAnimadorAsync(int usuarioId) =>
        (from ur in Context.UserRoles
         join r in Context.Roles on ur.RoleId equals r.Id
         where ur.UserId == usuarioId && r.Name == Roles.Animador
         select ur).AnyAsync();

    // Búsquedas para los selects
    public async Task<List<OpcionDto>> BuscarAnimadoresAsync(string? q)
    {
        var query =
            from u in Context.Users
            join ur in Context.UserRoles on u.Id equals ur.UserId
            join r in Context.Roles on ur.RoleId equals r.Id
            where r.Name == Roles.Animador
            select u;

        if (!string.IsNullOrWhiteSpace(q))
        {
            var b = q.Trim();
            query = query.Where(u =>
                u.Nombre.Contains(b) ||
                u.Apellido.Contains(b) ||
                (u.Especialidad != null && u.Especialidad.Contains(b)));
        }

        var datos = await query
            .OrderBy(u => u.Apellido).ThenBy(u => u.Nombre)
            .Take(10)
            .Select(u => new { u.Id, u.Nombre, u.Apellido, u.Especialidad })
            .ToListAsync();

        return datos.Select(d => new OpcionDto
        {
            Id = d.Id,
            Texto = $"{d.Nombre} {d.Apellido}",
            Detalle = d.Especialidad
        }).ToList();
    }

    public async Task<List<OpcionDto>> BuscarEscenasAsync(int directorId, string? q, int? proyectoId)
    {
        var query = Context.Escenas.Where(e => e.Proyecto.DirectorId == directorId);

        if (proyectoId != null)
            query = query.Where(e => e.ProyectoId == proyectoId);

        if (!string.IsNullOrWhiteSpace(q))
        {
            var b = q.Trim();
            var esNumero = int.TryParse(b, out var numero);
            query = query.Where(e =>
                e.Proyecto.Nombre.Contains(b) ||
                (e.Descripcion != null && e.Descripcion.Contains(b)) ||
                (esNumero && e.Numero == numero));
        }

        var datos = await query
            .OrderBy(e => e.Proyecto.Nombre).ThenBy(e => e.Numero)
            .Take(10)
            .Select(e => new { e.Id, Proyecto = e.Proyecto.Nombre, e.Numero, e.Descripcion })
            .ToListAsync();

        return datos.Select(d => new OpcionDto
        {
            Id = d.Id,
            Texto = $"{d.Proyecto} — Escena {d.Numero}",
            Detalle = d.Descripcion
        }).ToList();
    }
}
