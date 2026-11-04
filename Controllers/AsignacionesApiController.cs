using GenbaStudio.Models.Dtos;
using GenbaStudio.Models.Entities;
using GenbaStudio.Models.Enums;
using GenbaStudio.Repositories;
using GenbaStudio.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenbaStudio.Controllers.Api;

[ApiController]
[Route("api/asignaciones")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AsignacionesApiController : ControllerBase
{
    private const int PageSize = 10;

    private readonly IAsignacionRepository _asignaciones;
    private readonly IEscenaRepository _escenas;
    private readonly ITipoTareaRepository _tipos;

    public AsignacionesApiController(IAsignacionRepository asignaciones, IEscenaRepository escenas, ITipoTareaRepository tipos)
    {
        _asignaciones = asignaciones;
        _escenas = escenas;
        _tipos = tipos;
    }

    private (int? directorId, int? animadorId) Alcance()
    {
        var id = User.GetUserId();
        return User.IsInRole(Roles.Director) ? (id, null) : (null, id);
    }

    [HttpGet]
    [Authorize(Roles = Roles.Director + "," + Roles.Animador)]
    public async Task<IActionResult> Listar(string? q, EstadoAsignacion? estado, int page = 1)
    {
        var (directorId, animadorId) = Alcance();
        var resultado = await _asignaciones.GetPaginaAsync(directorId, animadorId, q, estado, page, PageSize);
        return Ok(resultado);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Director + "," + Roles.Animador)]
    public async Task<IActionResult> Obtener(int id)
    {
        var (directorId, animadorId) = Alcance();
        var dto = await _asignaciones.GetDtoAsync(id, directorId, animadorId);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Director)]
    public async Task<IActionResult> Crear(AsignacionGuardarDto dto)
    {
        var directorId = User.GetUserId();

        if (dto.FechaLimite.Date < DateTime.Today)
            ModelState.AddModelError(nameof(dto.FechaLimite), "La fecha límite no puede ser anterior a hoy.");

        await ValidarReglasAsync(dto, directorId, null);
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var asignacion = new Asignacion
        {
            EscenaId = dto.EscenaId,
            TipoTareaId = dto.TipoTareaId,
            AnimadorId = dto.AnimadorId,
            FechaAsignacion = DateTime.Now,
            FechaLimite = dto.FechaLimite.Date,
            Prioridad = dto.Prioridad,
            Estado = EstadoAsignacion.Pendiente,
            Observaciones = string.IsNullOrWhiteSpace(dto.Observaciones) ? null : dto.Observaciones.Trim()
        };
        await _asignaciones.AddAsync(asignacion);

        var creada = await _asignaciones.GetDtoAsync(asignacion.Id, directorId, null);
        return CreatedAtAction(nameof(Obtener), new { id = asignacion.Id }, creada);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.Director)]
    public async Task<IActionResult> Editar(int id, AsignacionGuardarDto dto)
    {
        var directorId = User.GetUserId();

        var asignacion = await _asignaciones.GetDelDirectorAsync(id, directorId);
        if (asignacion == null) return NotFound();

        if (asignacion.Estado == EstadoAsignacion.Aprobada)
        {
            ModelState.AddModelError("", "Una asignación aprobada no se puede modificar.");
            return ValidationProblem(ModelState);
        }

        await ValidarReglasAsync(dto, directorId, id);
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        asignacion.EscenaId = dto.EscenaId;
        asignacion.TipoTareaId = dto.TipoTareaId;
        asignacion.AnimadorId = dto.AnimadorId;
        asignacion.FechaLimite = dto.FechaLimite.Date;
        asignacion.Prioridad = dto.Prioridad;
        asignacion.Observaciones = string.IsNullOrWhiteSpace(dto.Observaciones) ? null : dto.Observaciones.Trim();
        await _asignaciones.UpdateAsync(asignacion);

        return Ok(await _asignaciones.GetDtoAsync(id, directorId, null));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Director)]
    public async Task<IActionResult> Eliminar(int id)
    {
        var asignacion = await _asignaciones.GetDelDirectorAsync(id, User.GetUserId());
        if (asignacion == null) return NotFound();

        // Si ya tiene entregas no se puede borrar.
        if (await _asignaciones.TieneEntregasAsync(id))
        {
            ModelState.AddModelError("", "No se puede eliminar: la asignación ya tiene entregas.");
            return ValidationProblem(ModelState);
        }

        await _asignaciones.DeleteAsync(asignacion);
        return NoContent();
    }

    private async Task ValidarReglasAsync(AsignacionGuardarDto dto, int directorId, int? asignacionId)
    {
        // La escena tiene que ser de un proyecto de ESTE director
        if (await _escenas.GetDelDirectorAsync(dto.EscenaId, directorId) == null)
            ModelState.AddModelError(nameof(dto.EscenaId), "La escena no existe o no pertenece a tus proyectos.");

        if (await _tipos.GetByIdAsync(dto.TipoTareaId) == null)
            ModelState.AddModelError(nameof(dto.TipoTareaId), "El tipo de tarea no existe.");

        if (!await _asignaciones.ExisteAnimadorAsync(dto.AnimadorId))
            ModelState.AddModelError(nameof(dto.AnimadorId), "El animador no existe.");

        // Una escena no puede tener dos asignaciones activas del mismo tipo
        if (ModelState.IsValid && await _asignaciones.ExisteActivaAsync(dto.EscenaId, dto.TipoTareaId, asignacionId))
            ModelState.AddModelError("", "Esa escena ya tiene una asignación activa de ese tipo de tarea.");
    }
}
