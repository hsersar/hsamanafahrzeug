using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FahrzeugZulassung.Domain.Entities;

namespace FahrzeugZulassung.Infrastructure.Persistence.Configurations;

public class MonatsabrechnungPositionConfiguration : IEntityTypeConfiguration<MonatsabrechnungPosition>
{
    public void Configure(EntityTypeBuilder<MonatsabrechnungPosition> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Beschreibung)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(p => p.Einheit)
            .HasMaxLength(20);
        
        builder.Property(p => p.Menge)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.Einzelpreis)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.Nettobetrag)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.Steuersatz)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.Steuerbetrag)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.Bruttobetrag)
            .HasPrecision(18, 2);
        
        builder.HasOne(p => p.Monatsabrechnung)
            .WithMany(m => m.Positionen)
            .HasForeignKey(p => p.MonatsabrechnungId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
