using GenbaStudio.Models.Enums;
using GenbaStudio.Models.ViewModels;
using GenbaStudio.Repositories;
using GenbaStudio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenbaStudio.Controllers;

[Authorize(Roles = Roles.Director)]
public class RevisionController : Controller
{
    private const int PageSize = 10;

    private readonly IEntregaRepository _entregas;
    private readonly IAsignacionRepository _asignaciones;

    public RevisionController(IEntregaRepository entregas, IAsignacionRepository asignaciones)
    {
        _entregas = entregas;
        _asignaciones = asignaciones;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1)
    {
        var resultado = await _entregas.GetPendientesDelDirectorAsync(User.GetUserId(), page, PageSize);
        return View(resultado);
    }

    [HttpGet]
    public async Task<IActionResult> Revisar(int id)
    {
        var directorId = User.GetUserId();

        var entrega = await _entregas.GetConAsignacionAsync(id);
        if (entrega == null || entrega.Asignacion.Escena.Proyecto.DirectorId != directorId)
            return NotFound();

        var dto = await _asignaciones.GetDtoAsync(entrega.AsignacionId, directorId, null);
        if (dto == null) return NotFound();

        var vm = new RevisarEntregaViewModel
        {
            EntregaId = id,
            Asignacion = dto,
            Historial = await _entregas.GetHistorialAsync(entrega.AsignacionId),
            PuedeRevisar = entrega.EstadoRevision == EstadoRevision.Pendiente
                           && entrega.Asignacion.Estado == EstadoAsignacion.EnRevision
        };
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Revisar(int id, string decision, string? feedback)
    {
        var entrega = await _entregas.GetConAsignacionAsync(id);
        if (entrega == null || entrega.Asignacion.Escena.Proyecto.DirectorId != User.GetUserId())
            return NotFound();

        if (entrega.EstadoRevision != EstadoRevision.Pendiente || entrega.Asignacion.Estado != EstadoAsignacion.EnRevision)
        {
            TempData["Error"] = "Esta entrega ya fue revisada.";
            return RedirectToAction(nameof(Revisar), new { id });
        }

        feedback = string.IsNullOrWhiteSpace(feedback) ? null : feedback.Trim();
        if (feedback != null && feedback.Length > 2000)
        {
            TempData["Error"] = "El feedback no puede superar los 2000 caracteres.";
            TempData["FeedbackBorrador"] = feedback;
            return RedirectToAction(nameof(Revisar), new { id });
        }

        if (decision == "correcciones")
        {
            if (feedback == null)
            {
                TempData["Error"] = "Para pedir correcciones tenés que escribir un feedback.";
                return RedirectToAction(nameof(Revisar), new { id });
            }
            await _entregas.RegistrarRevisionAsync(entrega, EstadoRevision.ConCorrecciones, feedback);
            TempData["Ok"] = "Se pidieron correcciones al animador.";
        }
        else if (decision == "aprobar")
        {
            await _entregas.RegistrarRevisionAsync(entrega, EstadoRevision.Aprobada, feedback);
            TempData["Ok"] = "Entrega aprobada.";
        }
        else
        {
            return BadRequest();
        }

        return RedirectToAction(nameof(Index));
    }
}
