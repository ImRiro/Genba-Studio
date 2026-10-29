using System.Security.Claims;
using GenbaStudio.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GenbaStudio.Services;

// claims usadas en el navbar para mostrar usuario logueado
public class UsuarioClaimsPrincipalFactory : UserClaimsPrincipalFactory<Usuario, IdentityRole<int>>
{
    public UsuarioClaimsPrincipalFactory(
        UserManager<Usuario> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IOptions<IdentityOptions> options) : base(userManager, roleManager, options) { }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(Usuario user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        identity.AddClaim(new Claim("NombreCompleto", user.NombreCompleto));
        if (!string.IsNullOrEmpty(user.Avatar))
            identity.AddClaim(new Claim("Avatar", user.Avatar));
        return identity;
    }
}
