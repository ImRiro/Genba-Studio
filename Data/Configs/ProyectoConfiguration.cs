using GenbaStudio.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaStudio.Data.Configurations;

public class ProyectoConfiguration : IEntityTypeConfiguration<Proyecto>
{
    public void Configure(EntityTypeBuilder<Proyecto> b)
    {
        b.ToTable("Proyectos");
        b.Property(p => p.Nombre).IsRequired().HasMaxLength(150);
        b.Property(p => p.Descripcion).HasMaxLength(1000);
        b.Property(p => p.Estado).HasConversion<string>().HasMaxLength(20);

        // "No se elimina un Usuario con proyectos asociados"
        b.HasOne(p => p.Director)
         .WithMany(u => u.ProyectosDirigidos)
         .HasForeignKey(p => p.DirectorId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
