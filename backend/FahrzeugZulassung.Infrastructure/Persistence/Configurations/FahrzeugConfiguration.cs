using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FahrzeugZulassung.Domain.Entities;

namespace FahrzeugZulassung.Infrastructure.Persistence.Configurations;

public class FahrzeugConfiguration : IEntityTypeConfiguration<Fahrzeug>
{
    public void Configure(EntityTypeBuilder<Fahrzeug> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.FIN)
            .IsRequired()
            .HasMaxLength(17);

        builder.Property(f => f.Kennzeichen)
            .HasMaxLength(15);

        builder.Property(f => f.Marke)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Modell)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Farbe)
            .HasMaxLength(50);

        builder.Property(f => f.Kraftstoffart)
            .HasMaxLength(50);

        builder.Property(f => f.ErstelltAm)
            .IsRequired();

        builder.HasMany(f => f.Auftraege)
            .WithOne(a => a.Fahrzeug)
            .HasForeignKey(a => a.FahrzeugId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(f => f.FIN)
            .IsUnique();

        builder.HasIndex(f => f.Kennzeichen);
        builder.HasIndex(f => new { f.Marke, f.Modell });
    }
}
