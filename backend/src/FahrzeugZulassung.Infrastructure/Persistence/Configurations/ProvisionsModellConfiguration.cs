using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FahrzeugZulassung.Domain.Entities;

namespace FahrzeugZulassung.Infrastructure.Persistence.Configurations;

public class ProvisionsModellConfiguration : IEntityTypeConfiguration<ProvisionsModell>
{
    public void Configure(EntityTypeBuilder<ProvisionsModell> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.MonatlicheGrundgebuehr)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(p => p.ProvisionsProzentsatz)
            .HasPrecision(18, 4)
            .IsRequired();
        
        builder.Property(p => p.MinProvisionProRechnung)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.MaxProvisionProRechnung)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.ErstelltVon)
            .HasMaxLength(200);
        
        builder.Property(p => p.AktualisiertVon)
            .HasMaxLength(200);
        
        builder.Property(p => p.Aenderungsgrund)
            .HasMaxLength(1000);
        
        // Indizes
        builder.HasIndex(p => p.StandortId);
        builder.HasIndex(p => p.IstStandard);
        builder.HasIndex(p => p.GueltigAb);
        
        // Navigation
        builder.HasOne(p => p.Standort)
            .WithMany(s => s.ProvisionsModelle)
            .HasForeignKey(p => p.StandortId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(p => p.Staffeln)
            .WithOne(s => s.ProvisionsModell)
            .HasForeignKey(s => s.ProvisionsModellId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
