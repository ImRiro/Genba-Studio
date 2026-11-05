using GenbaStudio.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GenbaStudio.Controllers;

[Authorize(Roles = Roles.Director)]
public class AsignacionesController : Controller
{
    public IActionResult Index() => View();
}
