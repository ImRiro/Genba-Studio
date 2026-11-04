using GenbaStudio.Models.Dtos;
using GenbaStudio.Models.Enums;
using GenbaStudio.Repositories;
using GenbaStudio.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenbaStudio.Controllers.Api;

// Endpoints usados en los selects
[ApiController]
[Route("api/busquedas")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Roles.Director)]
public class BusquedasApiController : ControllerBase
{
    private readonly IAsignacionRepository _asignaciones;
    private readonly ITipoTareaRepository _tipos;

    public BusquedasApiController(IAsignacionRepository asignaciones, ITipoTareaRepository tipos)
    {
        _asignaciones = asignaciones;
        _tipos = tipos;
    }

    [HttpGet("animadores")]
    public async Task<IActionResult> Animadores(string? q) =>
        Ok(await _asignaciones.BuscarAnimadoresAsync(q));

    // solo trae escenas de proyectos del director logueado
    [HttpGet("escenas")]
    public async Task<IActionResult> Escenas(string? q, int? proyectoId) =>
        Ok(await _asignaciones.BuscarEscenasAsync(User.GetUserId(), q, proyectoId));

    [HttpGet("tipos-tarea")]
    public async Task<IActionResult> TiposTarea()
    {
        var tipos = await _tipos.GetAllAsync();
        return Ok(tipos.Select(t => new OpcionDto { Id = t.Id, Texto = t.Nombre, Detalle = t.Descripcion }));
    }
}
