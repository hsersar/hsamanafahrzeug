using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FahrzeugZulassung.Domain.Entities;

namespace FahrzeugZulassung.Infrastructure.Persistence.Configurations;

public class KundeConfiguration : IEntityTypeConfiguration<Kunde>
{
    public void Configure(EntityTypeBuilder<Kunde> builder)
    {
        builder.HasKey(k => k.Id);

        builder.Property(k => k.Vorname)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(k => k.Nachname)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(k => k.Strasse)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(k => k.Hausnummer)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(k => k.PLZ)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(k => k.Ort)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(k => k.Telefon)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(k => k.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(k => k.DatenschutzAkzeptiert)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(k => k.ErstelltAm)
            .IsRequired();

        builder.HasOne(k => k.Benutzer)
            .WithOne()
            .HasForeignKey<Kunde>(k => k.BenutzerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(k => k.Auftraege)
            .WithOne(a => a.Kunde)
            .HasForeignKey(a => a.KundeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(k => k.BenutzerId)
            .IsUnique();

        builder.HasIndex(k => k.Email);
        builder.HasIndex(k => new { k.Nachname, k.Vorname });
    }
}
