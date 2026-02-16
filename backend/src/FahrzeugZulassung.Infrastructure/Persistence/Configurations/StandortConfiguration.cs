using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FahrzeugZulassung.Domain.Entities;

namespace FahrzeugZulassung.Infrastructure.Persistence.Configurations;

public class StandortConfiguration : IEntityTypeConfiguration<Standort>
{
    public void Configure(EntityTypeBuilder<Standort> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(s => s.Adresse)
            .HasMaxLength(500);
    }
}
