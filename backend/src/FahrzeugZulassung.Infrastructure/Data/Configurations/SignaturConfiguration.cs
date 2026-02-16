using FahrzeugZulassung.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FahrzeugZulassung.Infrastructure.Data.Configurations;

public class SignaturConfiguration : IEntityTypeConfiguration<Signatur>
{
    public void Configure(EntityTypeBuilder<Signatur> builder)
    {
        builder.HasKey(s => s.Id);
        
        // Indizes für Performance
        builder.HasIndex(s => s.ProviderSessionId);
        builder.HasIndex(s => s.ProviderTransaktionId);
        builder.HasIndex(s => s.AuftragId);
        builder.HasIndex(s => s.Status);
        
        // Beziehungen
        builder.HasOne(s => s.Auftrag)
            .WithMany(a => a.Signaturen)
            .HasForeignKey(s => s.AuftragId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(s => s.Kunde)
            .WithMany()
            .HasForeignKey(s => s.KundeId)
            .OnDelete(DeleteBehavior.SetNull);
        
        // String-Längen
        builder.Property(s => s.DokumentName).HasMaxLength(500);
        builder.Property(s => s.DokumentHash).HasMaxLength(128);
        builder.Property(s => s.DokumentHashAlgorithmus).HasMaxLength(50);
        builder.Property(s => s.Provider).HasMaxLength(100);
        builder.Property(s => s.ProviderSessionId).HasMaxLength(200);
        builder.Property(s => s.ProviderTransaktionId).HasMaxLength(200);
        builder.Property(s => s.ZertifikatSubject).HasMaxLength(500);
        builder.Property(s => s.ZertifikatIssuer).HasMaxLength(500);
        builder.Property(s => s.ZertifikatSeriennummer).HasMaxLength(100);
        builder.Property(s => s.SigniererName).HasMaxLength(200);
        builder.Property(s => s.SigniererEmail).HasMaxLength(200);
        
        // Text-Felder (unbegrenzt)
        builder.Property(s => s.SignaturWert).HasColumnType("text");
        builder.Property(s => s.ProviderAntwortJson).HasColumnType("text");
        builder.Property(s => s.FehlerNachricht).HasColumnType("text");
        builder.Property(s => s.OriginalDokumentPfad).HasMaxLength(1000);
        builder.Property(s => s.SigniertesDokumentPfad).HasMaxLength(1000);
        builder.Property(s => s.ProviderRedirectUrl).HasMaxLength(2000);
        builder.Property(s => s.ProviderCallbackUrl).HasMaxLength(2000);
        
        // Enums als Integer speichern
        builder.Property(s => s.Typ).HasConversion<int>();
        builder.Property(s => s.Status).HasConversion<int>();
        builder.Property(s => s.Level).HasConversion<int>();
        builder.Property(s => s.AuthMethode).HasConversion<int?>();
    }
}
