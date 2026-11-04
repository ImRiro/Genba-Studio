using GenbaStudio.Models.Entities;

namespace GenbaStudio.Services;

public interface IJwtService
{
    (string Token, DateTime Expira) Generar(Usuario usuario, string rol);
}
