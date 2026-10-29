using GenbaStudio.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GenbaStudio.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> b)
    {
        b.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
        b.Property(u => u.Apellido).IsRequired().HasMaxLength(100);
        b.Property(u => u.Avatar).HasMaxLength(255);
        b.Property(u => u.Especialidad).HasMaxLength(100);

        // "El Email es único"
        b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex").IsUnique();
    }
}
