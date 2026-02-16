using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FahrzeugZulassung.Domain.Entities;

namespace FahrzeugZulassung.Infrastructure.Persistence.Configurations;

public class AuftragConfiguration : IEntityTypeConfiguration<Auftrag>
{
    public void Configure(EntityTypeBuilder<Auftrag> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.AuftragNummer)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Typ)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(Domain.Enums.AuftragStatus.Entwurf);

        builder.Property(a => a.Step1Abgeschlossen)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.Step2Abgeschlossen)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.Step3Abgeschlossen)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.AGBAkzeptiert)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.UnterschriftVorhanden)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.IKfzReferenz)
            .HasMaxLength(100);

        builder.Property(a => a.IKfzAntwort)
            .HasColumnType("text");

        builder.Property(a => a.Bemerkungen)
            .HasColumnType("text");

        builder.Property(a => a.ErstelltAm)
            .IsRequired();

        builder.HasOne(a => a.Kunde)
            .WithMany(k => k.Auftraege)
            .HasForeignKey(a => a.KundeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Fahrzeug)
            .WithMany(f => f.Auftraege)
            .HasForeignKey(a => a.FahrzeugId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Standort)
            .WithMany(s => s.Auftraege)
            .HasForeignKey(a => a.StandortId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.ErstelltVon)
            .WithMany(b => b.ErstellteAuftraege)
            .HasForeignKey(a => a.ErstelltVonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Dokumente)
            .WithOne(d => d.Auftrag)
            .HasForeignKey(d => d.AuftragId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.Rechnungen)
            .WithOne(r => r.Auftrag)
            .HasForeignKey(r => r.AuftragId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.AuftragNummer)
            .IsUnique();

        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.KundeId);
        builder.HasIndex(a => a.FahrzeugId);
        builder.HasIndex(a => a.StandortId);
        builder.HasIndex(a => a.ErstelltVonId);
        builder.HasIndex(a => a.ErstelltAm);
        builder.HasIndex(a => a.IKfzReferenz);
    }
}
