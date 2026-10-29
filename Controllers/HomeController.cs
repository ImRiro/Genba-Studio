using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Genba_Studio.Models;
using Microsoft.AspNetCore.Authorization;

namespace Genba_Studio.Controllers;

[Authorize]
public class HomeController : Controller
{
    public IActionResult Index() => View();

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}