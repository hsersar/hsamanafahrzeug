using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FahrzeugZulassung.Domain.Entities;

namespace FahrzeugZulassung.Infrastructure.Persistence.Configurations;

public class ProvisionsStaffelConfiguration : IEntityTypeConfiguration<ProvisionsStaffel>
{
    public void Configure(EntityTypeBuilder<ProvisionsStaffel> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.AbUmsatz)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(s => s.BisUmsatz)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.Property(s => s.Prozentsatz)
            .HasPrecision(18, 4)
            .IsRequired();
        
        builder.HasOne(s => s.ProvisionsModell)
            .WithMany(p => p.Staffeln)
            .HasForeignKey(s => s.ProvisionsModellId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
