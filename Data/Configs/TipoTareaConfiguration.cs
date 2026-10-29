using GenbaStudio.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaStudio.Data.Configurations;

public class TipoTareaConfiguration : IEntityTypeConfiguration<TipoTarea>
{
    public void Configure(EntityTypeBuilder<TipoTarea> b)
    {
        b.ToTable("TiposTarea");
        b.Property(t => t.Nombre).IsRequired().HasMaxLength(60);
        b.Property(t => t.Descripcion).HasMaxLength(300);
        b.HasIndex(t => t.Nombre).IsUnique();

        b.HasData(
            new TipoTarea { Id = 1, Nombre = "Layout", Descripcion = "Composición de cámara y posiciones de la escena" },
            new TipoTarea { Id = 2, Nombre = "Key animation", Descripcion = "Poses clave de la animación" },
            new TipoTarea { Id = 3, Nombre = "Cleanup", Descripcion = "Limpieza y línea final sobre las claves" },
            new TipoTarea { Id = 4, Nombre = "Inbetween", Descripcion = "Intercalado entre las poses clave" },
            new TipoTarea { Id = 5, Nombre = "Color", Descripcion = "Coloreado de los frames" },
            new TipoTarea { Id = 6, Nombre = "Compositing", Descripcion = "Composición final de capas y efectos" }
        );
    }
}
