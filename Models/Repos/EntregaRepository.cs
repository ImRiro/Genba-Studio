using GenbaStudio.Data;
using GenbaStudio.Models.Common;
using GenbaStudio.Models.Entities;
using GenbaStudio.Models.Enums;
using GenbaStudio.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GenbaStudio.Repositories;

public class EntregaRepository : Repository<Entrega>, IEntregaRepository
{
    public EntregaRepository(AppDbContext context) : base(context) { }

    private static IQueryable<EntregaItemViewModel> AItem(IQueryable<Entrega> query) =>
        query.Select(e => new EntregaItemViewModel
        {
            Id = e.Id,
            AsignacionId = e.AsignacionId,
            Version = e.Version,
            NombreOriginal = e.NombreOriginal,
            FechaSubida = e.FechaSubida,
            EstadoRevision = e.EstadoRevision,
            ComentarioAnimador = e.ComentarioAnimador,
            FeedbackDirector = e.FeedbackDirector,
            Proyecto = e.Asignacion.Escena.Proyecto.Nombre,
            EscenaNumero = e.Asignacion.Escena.Numero,
            TipoTarea = e.Asignacion.TipoTarea.Nombre,
            Animador = e.Asignacion.Animador.Nombre + " " + e.Asignacion.Animador.Apellido
        });

    public async Task<PagedResult<EntregaItemViewModel>> GetPendientesDelDirectorAsync(int directorId, int page, int pageSize)
    {
        if (page < 1) page = 1;

        var query = Context.Entregas.AsNoTracking()
            .Where(e => e.EstadoRevision == EstadoRevision.Pendiente
                     && e.Asignacion.Escena.Proyecto.DirectorId == directorId);

        var total = await query.CountAsync();

        var pagina = query
            .OrderBy(e => e.FechaSubida).ThenBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        var items = await AItem(pagina).ToListAsync();

        return new PagedResult<EntregaItemViewModel> { Items = items, Page = page, PageSize = pageSize, TotalCount = total };
    }

    public Task<List<EntregaItemViewModel>> GetHistorialAsync(int asignacionId) =>
        AItem(Context.Entregas.AsNoTracking()
                .Where(e => e.AsignacionId == asignacionId)
                .OrderByDescending(e => e.Version))
            .ToListAsync();

    public Task<Entrega?> GetConAsignacionAsync(int id) =>
        Context.Entregas
            .Include(e => e.Asignacion).ThenInclude(a => a.Escena).ThenInclude(es => es.Proyecto)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task AgregarConVersionAsync(Entrega entrega)
    {
        await using var transaccion = await Context.Database.BeginTransactionAsync();

        var maxima = await Context.Entregas
            .Where(e => e.AsignacionId == entrega.AsignacionId)
            .MaxAsync(e => (int?)e.Version) ?? 0;
        entrega.Version = maxima + 1;
        Context.Entregas.Add(entrega);

        var asignacion = await Context.Asignaciones.FirstAsync(a => a.Id == entrega.AsignacionId);
        asignacion.Estado = EstadoAsignacion.EnRevision;

        await Context.SaveChangesAsync();
        await transaccion.CommitAsync();
    }

    public async Task RegistrarRevisionAsync(Entrega entrega, EstadoRevision decision, string? feedback)
    {
        entrega.EstadoRevision = decision;
        entrega.FeedbackDirector = feedback;
        entrega.Asignacion.Estado = decision == EstadoRevision.Aprobada
            ? EstadoAsignacion.Aprobada
            : EstadoAsignacion.ConCorrecciones;

        await Context.SaveChangesAsync();
    }
}
