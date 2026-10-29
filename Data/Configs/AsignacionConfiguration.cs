using GenbaStudio.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaStudio.Data.Configurations;

public class AsignacionConfiguration : IEntityTypeConfiguration<Asignacion>
{
    public void Configure(EntityTypeBuilder<Asignacion> b)
    {
        b.ToTable("Asignaciones");
        b.Property(a => a.Observaciones).HasMaxLength(1000);
        b.Property(a => a.Prioridad).HasConversion<string>().HasMaxLength(10);
        b.Property(a => a.Estado).HasConversion<string>().HasMaxLength(20);

        b.HasOne(a => a.Escena)
         .WithMany(e => e.Asignaciones)
         .HasForeignKey(a => a.EscenaId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(a => a.TipoTarea)
         .WithMany(t => t.Asignaciones)
         .HasForeignKey(a => a.TipoTareaId)
         .OnDelete(DeleteBehavior.Restrict);

        // "No se elimina un Usuario con asignaciones asociadas"
        b.HasOne(a => a.Animador)
         .WithMany(u => u.Asignaciones)
         .HasForeignKey(a => a.AnimadorId)
         .OnDelete(DeleteBehavior.Restrict);

        // "Una escena no puede tener dos asignaciones activas del mismo tipo"
        b.HasIndex(a => new { a.EscenaId, a.TipoTareaId });
        b.HasIndex(a => new { a.AnimadorId, a.Estado });
    }
}
