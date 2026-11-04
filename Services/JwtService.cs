using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GenbaStudio.Models.Entities;
using Microsoft.IdentityModel.Tokens;

namespace GenbaStudio.Services;

public class JwtService : IJwtService
{
    private const int HorasDeVida = 2;
    private readonly IConfiguration _config;

    public JwtService(IConfiguration config) => _config = config;

    public (string Token, DateTime Expira) Generar(Usuario usuario, string rol)
    {
        // La clave secreta (Jwt:Key) está en user-secrets
        var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("sub", usuario.Id.ToString()),
            new Claim("name", usuario.NombreCompleto),
            new Claim("email", usuario.Email ?? ""),
            new Claim("role", rol)
        };

        var expira = DateTime.UtcNow.AddHours(HorasDeVida);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expira,
            signingCredentials: credenciales);

        return (new JwtSecurityTokenHandler().WriteToken(token), expira);
    }
}
