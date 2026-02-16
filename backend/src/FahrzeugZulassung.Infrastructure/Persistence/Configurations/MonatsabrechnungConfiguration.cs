using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FahrzeugZulassung.Domain.Entities;

namespace FahrzeugZulassung.Infrastructure.Persistence.Configurations;

public class MonatsabrechnungConfiguration : IEntityTypeConfiguration<Monatsabrechnung>
{
    public void Configure(EntityTypeBuilder<Monatsabrechnung> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.AbrechnungsNummer)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(m => m.GesamtUmsatz)
            .HasPrecision(18, 2);
        
        builder.Property(m => m.Grundgebuehr)
            .HasPrecision(18, 2);
        
        builder.Property(m => m.ProvisionsProzentsatz)
            .HasPrecision(18, 4);
        
        builder.Property(m => m.ProvisionsBetrag)
            .HasPrecision(18, 2);
        
        builder.Property(m => m.Nettobetrag)
            .HasPrecision(18, 2);
        
        builder.Property(m => m.Steuersatz)
            .HasPrecision(18, 2);
        
        builder.Property(m => m.Steuerbetrag)
            .HasPrecision(18, 2);
        
        builder.Property(m => m.Bruttobetrag)
            .HasPrecision(18, 2);
        
        builder.Property(m => m.ErstelltVon)
            .HasMaxLength(200);
        
        builder.Property(m => m.ZahlungsReferenz)
            .HasMaxLength(200);
        
        // Unique Index für Standort + Jahr + Monat
        builder.HasIndex(m => new { m.StandortId, m.Jahr, m.Monat })
            .IsUnique();
        
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => m.AbrechnungsNummer);
        
        // Navigation
        builder.HasOne(m => m.Standort)
            .WithMany(s => s.Monatsabrechnungen)
            .HasForeignKey(m => m.StandortId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(m => m.Positionen)
            .WithOne(p => p.Monatsabrechnung)
            .HasForeignKey(p => p.MonatsabrechnungId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
