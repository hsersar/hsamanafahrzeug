using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FahrzeugZulassung.Domain.Entities;

namespace FahrzeugZulassung.Infrastructure.Persistence.Configurations;

public class RechnungConfiguration : IEntityTypeConfiguration<Rechnung>
{
    public void Configure(EntityTypeBuilder<Rechnung> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RechnungNummer)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Betrag)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(Domain.Enums.RechnungStatus.Offen);

        builder.Property(r => r.Faellig)
            .IsRequired();

        builder.Property(r => r.Zahlungsmethode)
            .HasMaxLength(50);

        builder.Property(r => r.ZahlungsReferenz)
            .HasMaxLength(200);

        builder.Property(r => r.ErstelltAm)
            .IsRequired();

        builder.HasOne(r => r.Auftrag)
            .WithMany(a => a.Rechnungen)
            .HasForeignKey(r => r.AuftragId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.RechnungNummer)
            .IsUnique();

        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.AuftragId);
        builder.HasIndex(r => r.Faellig);
        builder.HasIndex(r => r.BezahltAm);
    }
}
