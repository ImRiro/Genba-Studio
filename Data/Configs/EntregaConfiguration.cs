using GenbaStudio.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaStudio.Data.Configurations;

public class EntregaConfiguration : IEntityTypeConfiguration<Entrega>
{
    public void Configure(EntityTypeBuilder<Entrega> b)
    {
        b.ToTable("Entregas");
        b.Property(e => e.RutaArchivo).IsRequired().HasMaxLength(255);
        b.Property(e => e.NombreOriginal).IsRequired().HasMaxLength(255);
        b.Property(e => e.ComentarioAnimador).HasMaxLength(1000);
        b.Property(e => e.FeedbackDirector).HasMaxLength(2000);
        b.Property(e => e.EstadoRevision).HasConversion<string>().HasMaxLength(20);

        // "No hay dos entregas con la misma Version para una Asignacion"
        b.HasIndex(e => new { e.AsignacionId, e.Version }).IsUnique();

        b.HasOne(e => e.Asignacion)
         .WithMany(a => a.Entregas)
         .HasForeignKey(e => e.AsignacionId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
