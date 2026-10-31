using System.Security.Claims;

namespace GenbaStudio.Services;

public static class ClaimsPrincipalExtensions
{
    /// ID del usuario logueado (0 si no hay)
    public static int GetUserId(this ClaimsPrincipal user) =>
        int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
