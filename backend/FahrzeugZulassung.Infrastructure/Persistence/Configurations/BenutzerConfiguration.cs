using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FahrzeugZulassung.Domain.Entities;

namespace FahrzeugZulassung.Infrastructure.Persistence.Configurations;

public class BenutzerConfiguration : IEntityTypeConfiguration<Benutzer>
{
    public void Configure(EntityTypeBuilder<Benutzer> builder)
    {
        builder.Property(b => b.Vorname)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Nachname)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Rolle)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(b => b.IstAktiv)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(b => b.FehlgeschlageneLoginVersuche)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(b => b.ErstelltAm)
            .IsRequired();

        builder.HasOne(b => b.Standort)
            .WithMany(s => s.Mitarbeiter)
            .HasForeignKey(b => b.StandortId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(b => b.ErstellteAuftraege)
            .WithOne(a => a.ErstelltVon)
            .HasForeignKey(a => a.ErstelltVonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(b => b.AuditLogs)
            .WithOne(a => a.Benutzer)
            .HasForeignKey(a => a.BenutzerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(b => b.Email)
            .IsUnique();

        builder.HasIndex(b => b.StandortId);
    }
}
