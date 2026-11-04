using System.Security.Claims;

namespace GenbaStudio.Services;

public static class ClaimsPrincipalExtensions
{
    /// ID del usuario logueado (0 si no hay)
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var valor = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
        return int.TryParse(valor, out var id) ? id : 0;
    }
}
