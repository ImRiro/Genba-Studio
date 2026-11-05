using System.IO.Compression;
using GenbaStudio.Models.Entities;
using GenbaStudio.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GenbaStudio.Data.Seed;

public static class DbSeeder
{
    private const string ProyectoMarca = "El último farol";

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

        // (Seed:DemoUsers = true) || User-secret para usuarios y datos de prueba.
        if (config.GetValue<bool>("Seed:DemoUsers"))
        {
            await CrearUsuarioAsync(users, "director@genba.test", password, "Dana", "Director", null, Roles.Director);
            await CrearUsuarioAsync(users, "director2@genba.test", password, "Rita", "Director", null, Roles.Director);
            await CrearUsuarioAsync(users, "animador1@genba.test", password, "Lucas", "Animador", "Cleanup", Roles.Animador);
            await CrearUsuarioAsync(users, "animador2@genba.test", password, "Mia", "Animador", "Color", Roles.Animador);
            await CrearUsuarioAsync(users, "animador3@genba.test", password, "Sara", "Animador", "Layout", Roles.Animador);
            await CrearUsuarioAsync(users, "animador4@genba.test", password, "Mateo", "Animador", "Key Animation", Roles.Animador);
            await CrearUsuarioAsync(users, "animador5@genba.test", password, "Alicia", "Animador", "Inbetween", Roles.Animador);
            await CrearUsuarioAsync(users, "animador6@genba.test", password, "Noah", "Animador", "Compositing", Roles.Animador);
            await CrearUsuarioAsync(users, "animador7@genba.test", password, "Elena", "Animador", "Cleanup", Roles.Animador);
            await CrearUsuarioAsync(users, "animador8@genba.test", password, "Pablo", "Animador", "Color", Roles.Animador);
            await CrearUsuarioAsync(users, "animador9@genba.test", password, "Leona", "Animador", "Layout", Roles.Animador);
            await CrearUsuarioAsync(users, "animador10@genba.test", password, "Iván", "Animador", "Key Animation", Roles.Animador);
            await CrearUsuarioAsync(users, "animador11@genba.test", password, "Carmen", "Animador", "Inbetween", Roles.Animador);

            await SembrarDatosDemoAsync(sp, users);
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

    private static async Task SembrarDatosDemoAsync(IServiceProvider sp, UserManager<Usuario> users)
    {
        var db = sp.GetRequiredService<AppDbContext>();
        var env = sp.GetRequiredService<IWebHostEnvironment>();

        if (await db.Set<Proyecto>().AnyAsync(p => p.Nombre == ProyectoMarca)) return;

        var raizPrivada = Path.Combine(env.ContentRootPath, "App_Data", "private");

        var dana = await IdDeAsync(users, "director@genba.test");
        var rita = await IdDeAsync(users, "director2@genba.test");
        var lucas = await IdDeAsync(users, "animador1@genba.test");
        var mia = await IdDeAsync(users, "animador2@genba.test");
        var sara = await IdDeAsync(users, "animador3@genba.test");
        var mateo = await IdDeAsync(users, "animador4@genba.test");
        var alicia = await IdDeAsync(users, "animador5@genba.test");
        var noah = await IdDeAsync(users, "animador6@genba.test");
        var elena = await IdDeAsync(users, "animador7@genba.test");
        var pablo = await IdDeAsync(users, "animador8@genba.test");
        var leona = await IdDeAsync(users, "animador9@genba.test");
        var ivan = await IdDeAsync(users, "animador10@genba.test");

        var tipos = await AsegurarTiposTareaAsync(db);
        var hoy = DateTime.Today;

        // ----- Proyecto 1 (Director: Dana) -----
        var farol = new Proyecto
        {
            DirectorId = dana,
            Nombre = ProyectoMarca,
            Descripcion = "Corto de 6 minutos sobre un farolero y el último faro del puerto.",
            FechaInicio = hoy.AddDays(-45),
            FechaEntrega = hoy.AddDays(60),
            Estado = EstadoProyecto.EnProduccion
        };
        var f1 = NuevaEscena(farol, 1, "Amanecer en el puerto", 8, EstadoEscena.Finalizada);
        var f2 = NuevaEscena(farol, 2, "El farolero sube la escalera", 15, EstadoEscena.EnProduccion);
        var f3 = NuevaEscena(farol, 3, "La tormenta", 20, EstadoEscena.EnProduccion);
        var f4 = NuevaEscena(farol, 4, "El farol se apaga", 10, EstadoEscena.Pendiente);
        var f5 = NuevaEscena(farol, 5, "Créditos", 6, EstadoEscena.Pendiente);

        // Asignaciones: (escena, tipo, animador, estado, asignada hace N días, límite en N días, prioridad)
        var a1 = NuevaAsignacion(f1, tipos["Layout"], sara, EstadoAsignacion.Aprobada, 30, -15, Prioridad.Media);
        var a2 = NuevaAsignacion(f1, tipos["Key Animation"], mateo, EstadoAsignacion.Aprobada, 28, -8, Prioridad.Alta,
            "Cuidar el arco de la caída del farol.");
        var a3 = NuevaAsignacion(f2, tipos["Layout"], sara, EstadoAsignacion.Aprobada, 20, -5, Prioridad.Media);
        var a4 = NuevaAsignacion(f2, tipos["Key Animation"], mateo, EstadoAsignacion.ConCorrecciones, 14, 5, Prioridad.Alta);
        var a5 = NuevaAsignacion(f2, tipos["Cleanup"], lucas, EstadoAsignacion.EnRevision, 10, 7, Prioridad.Media);
        var a6 = NuevaAsignacion(f3, tipos["Layout"], leona, EstadoAsignacion.EnRevision, 9, 6, Prioridad.Alta);
        var a7 = NuevaAsignacion(f3, tipos["Key Animation"], ivan, EstadoAsignacion.EnProgreso, 8, 10, Prioridad.Alta);
        var a8 = NuevaAsignacion(f3, tipos["Color"], mia, EstadoAsignacion.Pendiente, 3, 14, Prioridad.Baja);
        var a9 = NuevaAsignacion(f4, tipos["Layout"], sara, EstadoAsignacion.Pendiente, 12, -3, Prioridad.Alta,
            "Fecha límite vencida (para probar el aviso).");
        var a10 = NuevaAsignacion(f4, tipos["Inbetween"], alicia, EstadoAsignacion.Pendiente, 2, 20, Prioridad.Media);
        var a11 = NuevaAsignacion(f5, tipos["Compositing"], noah, EstadoAsignacion.Pendiente, 1, 30, Prioridad.Baja);
        var a12 = NuevaAsignacion(f5, tipos["Cleanup"], elena, EstadoAsignacion.EnProgreso, 4, 12, Prioridad.Media);
        var a13 = NuevaAsignacion(f5, tipos["Color"], pablo, EstadoAsignacion.EnProgreso, 15, -2, Prioridad.Media);

        var entregas = new List<Entrega>
        {
            NuevaEntrega(a1, 1, "E1_layout_v1.zip", EstadoRevision.Aprobada, 20, raizPrivada,
                "Layout de la escena 1.", "Encuadre aprobado."),

            NuevaEntrega(a2, 1, "E1_keyanim_v1.zip", EstadoRevision.ConCorrecciones, 20, raizPrivada,
                "Primera pasada de poses clave.", "Suavizar el arco de la caída del farol."),
            NuevaEntrega(a2, 2, "E1_keyanim_v2.zip", EstadoRevision.Aprobada, 12, raizPrivada,
                "Corregí el arco de la caída.", "Ahora sí, aprobada."),

            NuevaEntrega(a3, 1, "E2_layout_v1.zip", EstadoRevision.Aprobada, 10, raizPrivada,
                "Layout de la subida.", "Perfecto."),

            NuevaEntrega(a4, 1, "E2_keyanim_v1.zip", EstadoRevision.ConCorrecciones, 3, raizPrivada,
                "Poses de la subida.", "El peso del farolero no se siente en los escalones; exagerar la inclinación."),

            NuevaEntrega(a5, 1, "E2_cleanup_v1.zip", EstadoRevision.Pendiente, 1, raizPrivada,
                "Limpieza lista, falta revisar el pelo."),
            NuevaEntrega(a6, 1, "E3_layout_v1.zip", EstadoRevision.Pendiente, 2, raizPrivada,
                "Cámara con leve movimiento hacia arriba."),

            NuevaEntrega(a7, 1, "E3_keyanim_v1.zip", EstadoRevision.ConCorrecciones, 4, raizPrivada,
                "Primera versión de la tormenta.", "Las ráfagas de viento deberían afectar el abrigo.")
        };

        // ----- Proyecto 2 (Director: Dana), en planificación -----
        var marea = new Proyecto
        {
            DirectorId = dana,
            Nombre = "Marea",
            Descripcion = "Videoclip de 2 minutos, todavía en preproducción.",
            FechaInicio = hoy.AddDays(10),
            FechaEntrega = hoy.AddDays(120),
            Estado = EstadoProyecto.Planificacion
        };
        var m1 = NuevaEscena(marea, 1, "Intro", 20, EstadoEscena.Pendiente);
        var m2 = NuevaEscena(marea, 2, "Estribillo", 25, EstadoEscena.Pendiente);
        var a14 = NuevaAsignacion(m1, tipos["Layout"], leona, EstadoAsignacion.Pendiente, 1, 25, Prioridad.Media);

        // ----- Proyecto 3 (Director: Rita): sirve para probar que cada director ve SOLO lo suyo -----
        var neo = new Proyecto
        {
            DirectorId = rita,
            Nombre = "Neo Tokio, capítulo 1",
            Descripcion = "Primer capítulo de una serie de ciencia ficción.",
            FechaInicio = hoy.AddDays(-20),
            FechaEntrega = hoy.AddDays(90),
            Estado = EstadoProyecto.EnProduccion
        };
        var n1 = NuevaEscena(neo, 1, "Apertura", 30, EstadoEscena.EnProduccion);
        var a15 = NuevaAsignacion(n1, tipos["Layout"], sara, EstadoAsignacion.EnProgreso, 6, 8, Prioridad.Alta);
        var a16 = NuevaAsignacion(n1, tipos["Key Animation"], mateo, EstadoAsignacion.EnRevision, 7, 5, Prioridad.Media);
        entregas.Add(NuevaEntrega(a16, 1, "Neo_keyanim_v1.zip", EstadoRevision.Pendiente, 2, raizPrivada,
            "Poses de la apertura."));

        db.AddRange(farol, marea, neo);
        db.AddRange(f1, f2, f3, f4, f5, m1, m2, n1);
        db.AddRange(a1, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16);
        db.AddRange(entregas);
        await db.SaveChangesAsync();
    }

    private static async Task<int> IdDeAsync(UserManager<Usuario> users, string email)
    {
        var usuario = await users.FindByEmailAsync(email)
            ?? throw new InvalidOperationException($"No existe el usuario demo {email}.");
        return usuario.Id;
    }

    private static async Task<Dictionary<string, int>> AsegurarTiposTareaAsync(AppDbContext db)
    {
        var tipos = new (string Nombre, string Descripcion)[]
        {
            ("Layout", "Encuadre, cámara y posición de los personajes en la escena."),
            ("Key Animation", "Poses clave que definen el movimiento."),
            ("Cleanup", "Limpieza de línea sobre las poses clave."),
            ("Inbetween", "Dibujos intermedios entre las poses clave."),
            ("Color", "Pintado de los dibujos."),
            ("Compositing", "Unión de capas, fondos y efectos.")
        };

        foreach (var (nombre, descripcion) in tipos)
        {
            if (!await db.Set<TipoTarea>().AnyAsync(t => t.Nombre == nombre))
                db.Add(new TipoTarea { Nombre = nombre, Descripcion = descripcion });
        }
        await db.SaveChangesAsync();

        var todos = await db.Set<TipoTarea>().ToListAsync();
        return todos.ToDictionary(t => t.Nombre, t => t.Id, StringComparer.OrdinalIgnoreCase);
    }

    private static Escena NuevaEscena(Proyecto proyecto, int numero, string descripcion, int segundos, EstadoEscena estado) =>
        new()
        {
            Proyecto = proyecto,
            Numero = numero,
            Descripcion = descripcion,
            DuracionSegundos = segundos,
            Estado = estado
        };

    private static Asignacion NuevaAsignacion(Escena escena, int tipoTareaId, int animadorId, EstadoAsignacion estado,
        int asignadaHaceDias, int limiteEnDias, Prioridad prioridad, string? observaciones = null) =>
        new()
        {
            Escena = escena,
            TipoTareaId = tipoTareaId,
            AnimadorId = animadorId,
            FechaAsignacion = DateTime.Today.AddDays(-asignadaHaceDias),
            FechaLimite = DateTime.Today.AddDays(limiteEnDias),
            Prioridad = prioridad,
            Estado = estado,
            Observaciones = observaciones
        };

    private static Entrega NuevaEntrega(Asignacion asignacion, int version, string nombreOriginal, EstadoRevision estado,
        int subidaHaceDias, string raizPrivada, string? comentario = null, string? feedback = null) =>
        new()
        {
            Asignacion = asignacion,
            Version = version,
            NombreOriginal = nombreOriginal,
            RutaArchivo = CrearArchivoDemo(raizPrivada, nombreOriginal),
            ComentarioAnimador = comentario,
            FechaSubida = DateTime.Now.AddDays(-subidaHaceDias),
            EstadoRevision = estado,
            FeedbackDirector = feedback
        };

    private static string CrearArchivoDemo(string raizPrivada, string nombreOriginal)
    {
        var carpeta = Path.Combine(raizPrivada, "entregas");
        Directory.CreateDirectory(carpeta);

        var nombre = $"{Guid.NewGuid():N}.zip";
        using var fs = new FileStream(Path.Combine(carpeta, nombre), FileMode.CreateNew);
        using var zip = new ZipArchive(fs, ZipArchiveMode.Create);
        var entrada = zip.CreateEntry("LEEME.txt");
        using (var escritor = new StreamWriter(entrada.Open()))
            escritor.WriteLine($"Archivo de demostración: {nombreOriginal}");

        return $"entregas/{nombre}";
    }
}
