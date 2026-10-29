using GenbaStudio.Models.Entities;
using GenbaStudio.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace GenbaStudio.Data.Seed;

public static class DbSeeder
{

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var config = sp.GetRequiredService<IConfiguration>();
        var roles = sp.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var users = sp.GetRequiredService<UserManager<Usuario>>();

        foreach (var rol in Roles.Todos)
        {
            if (!await roles.RoleExistsAsync(rol))
                await roles.CreateAsync(new IdentityRole<int>(rol));
        }

        var password = config["Seed:Password"]
            ?? throw new InvalidOperationException("Falta la clave 'Seed:Password' (user-secrets).");
        var adminEmail = config["Seed:AdminEmail"]
            ?? throw new InvalidOperationException("Falta la clave 'Seed:AdminEmail' (user-secrets).");

        await CrearUsuarioAsync(users, adminEmail, password, "Admin", "Genba", null, Roles.Administrador);

        // (Seed:DemoUsers = true) || User-secret para usuarios de prueba.
        if (config.GetValue<bool>("Seed:DemoUsers"))
        {
            await CrearUsuarioAsync(users, "director@genba.test", password, "Dana", "Director", null, Roles.Director);
            await CrearUsuarioAsync(users, "animador1@genba.test", password, "Lucas", "Animador", "Cleanup", Roles.Animador);
            await CrearUsuarioAsync(users, "animador2@genba.test", password, "Mia", "Animador", "Color", Roles.Animador);
        }
    }

    private static async Task CrearUsuarioAsync(UserManager<Usuario> users, string email, string password,
        string nombre, string apellido, string? especialidad, string rol)
    {
        if (await users.FindByEmailAsync(email) != null) return;

        var usuario = new Usuario
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            Nombre = nombre,
            Apellido = apellido,
            Especialidad = especialidad
        };

        var result = await users.CreateAsync(usuario, password);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                $"No se pudo crear {email}: {string.Join("; ", result.Errors.Select(e => e.Description))}");

        await users.AddToRoleAsync(usuario, rol);
    }
}
