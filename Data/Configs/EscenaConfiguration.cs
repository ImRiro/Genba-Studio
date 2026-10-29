using GenbaStudio.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaStudio.Data.Configurations;

public class EscenaConfiguration : IEntityTypeConfiguration<Escena>
{
    public void Configure(EntityTypeBuilder<Escena> b)
    {
        b.ToTable("Escenas");
        b.Property(e => e.Descripcion).HasMaxLength(1000);
        b.Property(e => e.RutaStoryboard).HasMaxLength(255);
        b.Property(e => e.Estado).HasConversion<string>().HasMaxLength(20);

        // "No hay dos escenas con el mismo Numero dentro de un Proyecto"
        b.HasIndex(e => new { e.ProyectoId, e.Numero }).IsUnique();

        b.HasOne(e => e.Proyecto)
         .WithMany(p => p.Escenas)
         .HasForeignKey(e => e.ProyectoId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
