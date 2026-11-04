using GenbaStudio.Models.Dtos;
using GenbaStudio.Models.Entities;
using GenbaStudio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GenbaStudio.Controllers.Api;

[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly UserManager<Usuario> _users;
    private readonly SignInManager<Usuario> _signIn;
    private readonly IJwtService _jwt;

    public AuthApiController(UserManager<Usuario> users, SignInManager<Usuario> signIn, IJwtService jwt)
    {
        _users = users;
        _signIn = signIn;
        _jwt = jwt;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginApiRequest request)
    {
        var user = await _users.FindByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized(new { mensaje = "Email o contraseña incorrectos." });

        var resultado = await _signIn.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!resultado.Succeeded)
            return Unauthorized(new { mensaje = "Email o contraseña incorrectos." });

        var rol = (await _users.GetRolesAsync(user)).FirstOrDefault() ?? "";
        var (token, expira) = _jwt.Generar(user, rol);

        return Ok(new { token, expira, rol, nombre = user.NombreCompleto });
    }

    [HttpGet("token")]
    [Authorize(AuthenticationSchemes = "Identity.Application")] 
    public async Task<IActionResult> ObtenerToken()
    {
        var user = await _users.GetUserAsync(User);
        if (user == null)
            return Unauthorized(new { mensaje = "Usuario no encontrado." });

        var rol = (await _users.GetRolesAsync(user)).FirstOrDefault() ?? "";
        var (token, expira) = _jwt.Generar(user, rol);

        return Ok(new { token, expira, rol, nombre = user.NombreCompleto });
    }
}
